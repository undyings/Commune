using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Windows.Forms;

namespace Commune.Forms
{
  public class ComboFiller<T>
  {
    readonly ComboBox comboBox;
    readonly Executter<T> selectedItemChanged;

    public ComboFiller(ComboBox comboBox, Executter<T> selectedItemChanged) :
      this(comboBox, false, selectedItemChanged)
    {
    }

    readonly bool onlyValueFromList;
    public ComboFiller(ComboBox comboBox, bool onlyValueFromList,
      Executter<T> selectedItemChanged)
    {
      this.comboBox = comboBox;
      this.selectedItemChanged = selectedItemChanged;
      this.comboBox.SelectedIndexChanged += SelectedIndexChanged;

      this.onlyValueFromList = onlyValueFromList;
    }

    void SelectedIndexChanged(object sender, EventArgs args)
    {
      ComboItem<T> item = comboBox.SelectedItem as ComboItem<T>;
      if (selectedItemChanged != null && item != null)
        selectedItemChanged(item.Item);
    }

    public void ComboFillItems(T first, string firstDisplay,
      bool sortNeeded, IEnumerable<T> sortableItems, Getter<string, T> displayGetter)
    {
      ComboFillItems(sortNeeded, sortableItems, displayGetter,
        new ComboItem<T>(first, firstDisplay));
    }

    public void ComboFillItems(bool sortNeeded, IEnumerable<T> sortableItems, Getter<string, T> displayGetter,
      params ComboItem<T>[] leadItems)
    {
      IList<ComboItem<T>> comboItems = ComboFromItems(sortNeeded, sortableItems, displayGetter);
      for (int i = leadItems.Length - 1; i >= 0; --i)
      {
        comboItems.Insert(0, leadItems[i]);
      }

      ComboFillItems(_.ToArray(comboItems));
    }

    public void ComboFillItems(bool sortNeeded, IEnumerable<T> items, Getter<string, T> displayGetter)
    {
      IList<ComboItem<T>> comboItems = ComboFromItems(sortNeeded, items, displayGetter);

      ComboFillItems(_.ToArray(comboItems));
    }

    public static IList<ComboItem<T>> ComboFromItems(bool sortNeeded, IEnumerable<T> items,
      Getter<string, T> displayGetter)
    {
      List<ComboItem<T>> comboItems = new List<ComboItem<T>>();
      foreach (T itemId in items)
      {
        comboItems.Add(new ComboItem<T>(itemId, displayGetter(itemId)));
      }
      if (sortNeeded)
        return _.SortBy(comboItems, delegate (ComboItem<T> comboItem) { return comboItem.DisplayName; });

      return comboItems;
    }

    public void ComboFillItems(params ComboItem<T>[] comboItems)
    {
      T selectionItem = SelectionItem;

      comboBox.Items.Clear();
      comboBox.Items.AddRange(comboItems);
      SelectionItem = selectionItem;
    }

    public T SelectionItem
    {
      get
      {
        ComboItem<T> item = comboBox.SelectedItem as ComboItem<T>;
        if (item != null)
          return item.Item;
        return default(T);
      }
      set
      {
        comboBox.SelectedIndexChanged -= SelectedIndexChanged;

        try
        {
          int index = -1;
          foreach (ComboItem<T> item in comboBox.Items)
          {
            index++;
            if (object.Equals(item.Item, value))
            {
              comboBox.SelectedIndex = index;
              return;
            }
          }

          if (onlyValueFromList)
            comboBox.SelectedIndex = 0;
          else
          {
            comboBox.SelectedIndex = -1;
            //comboBox.Text = value != null ? value.ToString() : "";
          }
        }
        finally
        {
          comboBox.SelectedIndexChanged += SelectedIndexChanged;
        }
      }
    }
  }
}
