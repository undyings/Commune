using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;
using System.Collections;

namespace Commune.Forms
{
  public class ComboGridCellFiller : IGridCellFiller
  {
    public readonly static ComboGridCellFiller Default = new ComboGridCellFiller(SynchronizerHlp.PaintTextCell);
    public readonly static ComboGridCellFiller DefaultAsReadonly = new ComboGridCellFiller(SynchronizerHlp.PaintTextCell, true, false);
    public readonly static ComboGridCellFiller DefaultWithText = new ComboGridCellFiller(
      SynchronizerHlp.PaintTextCell,
      false, true
    );
    public readonly static ComboGridCellFiller ReadonlyWithListColor =
      new ComboGridCellFiller(
        delegate (Graphics g, Rectangle cellRect, IGridColumn column, object row, object value)
        {
          Color? color = SynchronizerHlp.GetBackColorForCellOrValue(column, row, value);
          SynchronizerHlp.PaintTextCell(g, cellRect, column, row, value, color);
        },
        true, false
      );

    readonly Executter<Graphics, Rectangle, IGridColumn, object, object> cellPainter;
    readonly bool isReadonly;
    readonly bool textValueEnabled;

    public ComboGridCellFiller(Executter<Graphics, Rectangle, IGridColumn, object, object> cellPainter) :
      this(cellPainter, false, false)
    {
    }

    public ComboGridCellFiller(Executter<Graphics, Rectangle, IGridColumn, object, object> cellPainter,
      bool isReadonly, bool textValueEnabled)
    {
      this.cellPainter = cellPainter;
      this.isReadonly = isReadonly;
      this.textValueEnabled = textValueEnabled;
    }

    public AutoCompleteMode AutoCompleteMode = AutoCompleteMode.Suggest;

    public Control CreateEditControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      ComboBox comboBox = new ComboBox();
      comboBox.DrawMode = DrawMode.OwnerDrawVariable;
      comboBox.AutoSize = false;
      comboBox.Visible = true;
      //comboBox.Location = cellRectangle.Location;
      comboBox.Size = cellRectangle.Size;
      Getter<IEnumerable, object> comboItemsGetter =
        column.GetExtended("ComboItems") as Getter<IEnumerable, object>;
      if (comboItemsGetter != null)
      {
        IEnumerable comboItems = comboItemsGetter(row);
        if (comboItems != null)
          comboBox.Items.AddRange(_.ToArray1<object>(comboItems));
      }
      comboBox.DroppedDown = true;
      if (textValueEnabled)
        //comboBox.DropDownStyle = ComboBoxStyle.DropDown;
        comboBox.DropDownStyle = ComboBoxStyle.DropDown;
      else
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      comboBox.FlatStyle = FlatStyle.Standard;
      //comboBox.FlatStyle = FlatStyle.Popup; //Кидает исключение при прибивании
      comboBox.DropDownHeight = Math.Min(12, comboBox.Items.Count + 1) * comboBox.Height;
      if (isReadonly)
      {
        if (value != null)
          this.readonlyValue = SynchronizerHlp.ValueToString(column, row, value);
      }
      else if (textValueEnabled)
      {
        comboBox.AutoCompleteMode = AutoCompleteMode;
        comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        comboBox.Text = SynchronizerHlp.ValueToString(column, row, value);
      }
      else
      {
        if (value != null)
          this.readonlyValue = SynchronizerHlp.ValueToString(column, row, value);
      }

      comboBox.MeasureItem += new MeasureItemEventHandler(comboBox_MeasureItem);
      comboBox.DrawItem += new DrawItemEventHandler(comboBox_DrawItem);
      comboBox.Disposed += new EventHandler(comboBox_Disposed);
      if (SynchronizerHlp.IsNoDelayApply(column))
        comboBox.SelectedIndexChanged += delegate (object sender, EventArgs e)
        {
          Control control = (Control)sender;
          GridCell gridCell = (GridCell)control.Tag;
          PushedValue(control, gridCell.Property, gridCell.DataItem);
        };
      return comboBox;
    }

    string readonlyValue = "";

    void comboBox_Disposed(object sender, EventArgs e)
    {
      ComboBox comboBox = (ComboBox)sender;
      comboBox.FlatStyle = FlatStyle.Standard;
    }

    void comboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      ComboBox comboBox = (ComboBox)sender;
      e.ItemHeight = comboBox.DisplayRectangle.Height;
    }

    Brush SelectedBrush = new SolidBrush(Color.FromArgb(40, Color.Indigo));

    void comboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      ComboBox comboBox = (ComboBox)sender;
      if (comboBox.Tag == null)
        return;

      bool isComboBoxEdit = ((int)e.State & (int)DrawItemState.ComboBoxEdit) == (int)DrawItemState.ComboBoxEdit;
      bool isSelected = ((int)e.State & (int)DrawItemState.Selected) == (int)DrawItemState.Selected;

      object value;
      if (isReadonly && isComboBoxEdit)
        value = readonlyValue;
      else
      {
        if (e.Index == -1)
          return;
        value = comboBox.Items[e.Index];
      }
      GridCell cell = (GridCell)comboBox.Tag;
      Rectangle rect = e.Bounds;
      int shift = (comboBox.DisplayRectangle.Height - e.Bounds.Height) / 2;
      if (shift > 0)
      {
        //rect = new Rectangle(e.Bounds.X + shift - 1, e.Bounds.Y + shift,
        //  e.Bounds.Width - shift * 2, e.Bounds.Height - shift * 2);
        rect = new Rectangle(e.Bounds.X - shift, e.Bounds.Y - shift,
          e.Bounds.Width + shift * 2, e.Bounds.Height + shift * 2);
      }

      if (isSelected && !isComboBoxEdit)
        e.Graphics.FillRectangle(SelectedBrush, rect);
      else if (!isComboBoxEdit)
        e.DrawBackground();
      cellPainter(e.Graphics, rect, cell.Property, cell.DataItem, value);
      e.DrawFocusRectangle();
    }

    void comboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComboBox control = (ComboBox)sender;
      GridCell gridCell = control.Tag as GridCell;
      if (gridCell == null)
        return;
      PushedValue(control, gridCell.Property, gridCell.DataItem);
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      //cellPainter(graphics, new Rectangle(cellRectangle.X + 3, cellRectangle.Y + 3,
      //  cellRectangle.Width - 6, cellRectangle.Height - 6), column, row, value);
      cellPainter(graphics, cellRectangle, column, row, value);
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
      ComboBox comboBox = control as ComboBox;
      if (comboBox == null)
        Logger.AddMessage("ComboGridCellFiller обрабатывает только ComboBox'ы");
      else
      {
        if (textValueEnabled)
          column.SetValue(row, comboBox.Text);
        else
          column.SetValue(row, comboBox.SelectedItem);
      }
    }
  }
}
