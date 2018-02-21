using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Forms
{
  public class ComboItem<T>
  {
    public readonly T Item;
    public readonly string DisplayName;

    public ComboItem(T item, string displayName)
    {
      this.Item = item;
      this.DisplayName = displayName;
    }

    public override bool Equals(object obj)
    {
      ComboItem<T> comboItem = obj as ComboItem<T>;
      if (comboItem == null)
      {
        if (obj is T)
          return _.Equals(Item, (T)obj);
        return false;
      }
      return _.Equals(Item, comboItem.Item);
      //if (Item is IComparable || comboItem.Item is IComparable)
      //  return Comparer<T>.Default.Compare(Item, comboItem.Item) == 0;
      //return object.ReferenceEquals(Item, comboItem.Item);
    }

    public override int GetHashCode()
    {
      if (Item == null)
        return 0;
      return Item.GetHashCode();
    }

    public override string ToString()
    {
      return DisplayName;
    }
  }
}
