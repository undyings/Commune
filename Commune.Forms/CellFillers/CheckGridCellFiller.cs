using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public class CheckGridCellFiller : IGridCellFiller
  {
    public readonly static CheckGridCellFiller Default = new CheckGridCellFiller(false, false);
    public readonly static CheckGridCellFiller Immediate = new CheckGridCellFiller(true, false);

    public static CheckBox CreateCheckEditControl(Rectangle cellRectangle,
      IGridColumn column, object row, object value, bool threeState)
    {
      CheckBox checkBox = new CheckBox();
      checkBox.CheckAlign = ContentAlignment.MiddleCenter;
      checkBox.AutoSize = false;
      checkBox.Size = cellRectangle.Size;

      if (threeState)
        checkBox.ThreeState = true;

      //hack Цвет фона не поддерживается из-за хака в PaintCell
      Color? backColor = SynchronizerHlp.GetBackColor(column, row);
      if (backColor != null)
        checkBox.BackColor = backColor.Value;

      bool? isChecked = value as bool?;
      if (isChecked == null)
        checkBox.CheckState = CheckState.Indeterminate;
      else if (isChecked == true)
        checkBox.CheckState = CheckState.Checked;
      else if (isChecked == false)
        checkBox.CheckState = CheckState.Unchecked;

      return checkBox;
    }

    readonly bool isImmediate;
    readonly bool threeState;
    public CheckGridCellFiller(bool isImmediate, bool threeState)
    {
      this.isImmediate = isImmediate;
      this.threeState = threeState;
    }

    public CheckGridCellFiller()
      : this(false, false)
    {
    }

    void checkBox_CheckedChanged(object sender, EventArgs e)
    {
      Control control = (Control)sender;
      GridCell gridCell = (GridCell)control.Tag;
      PushedValue(control, gridCell.Property, gridCell.DataItem);
    }

    public Control CreateEditControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      CheckBox checkBox = CreateCheckEditControl(cellRectangle, column, row, value, threeState);

      if (isImmediate)
      {
        if (!threeState)
          checkBox.Checked = !checkBox.Checked;
        else
        {
          if (checkBox.CheckState == CheckState.Checked)
            checkBox.CheckState = CheckState.Indeterminate;
          else if (checkBox.CheckState == CheckState.Indeterminate)
            checkBox.CheckState = CheckState.Unchecked;
          else
            checkBox.CheckState = CheckState.Checked;
        }
        if (SynchronizerHlp.IsNoDelayApply(column))
          PushedValue(checkBox, column, row);
      }

      if (SynchronizerHlp.IsNoDelayApply(column))
        checkBox.CheckStateChanged += new EventHandler(checkBox_CheckedChanged);

      return checkBox;
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      using (CheckBox checkBox = CreateCheckEditControl(cellRectangle, column, row, value, threeState))
      {
        Color? backColor = SynchronizerHlp.GetBackColor(column, row);
        //Hack Для того, чтобы подложка контрола была прозрачной
        if (backColor == null)
          checkBox.BackColor = Color.DarkOrange;
        using (Bitmap bitmap = new Bitmap(checkBox.Width, checkBox.Height))
        {
          checkBox.DrawToBitmap(bitmap, new Rectangle(0, 0, checkBox.Width, checkBox.Height));
          if (backColor == null)
            bitmap.MakeTransparent(Color.DarkOrange);
          graphics.DrawImage(bitmap, cellRectangle);
        }
      }
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
      CheckBox checkBox = control as CheckBox;
      if (checkBox == null)
      {
        Logger.AddMessage("CheckGridCellFiller обрабатывает только CheckBox'ы");
        return;
      }
      if (checkBox.CheckState == CheckState.Indeterminate)
        column.SetValue(row, null);
      else
        column.SetValue(row, checkBox.Checked);
    }

  }
}
