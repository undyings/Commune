using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public class TextGridCellFiller : IGridCellFiller
  {
    public readonly static TextGridCellFiller Default = new TextGridCellFiller(null);
    public readonly static TextGridCellFiller DefaultWithLeftImage = new TextGridCellFiller(false);
    public readonly static TextGridCellFiller DefaultWithRightImage = new TextGridCellFiller(true);

    readonly bool? isRightImage;
    public TextGridCellFiller(bool? isRightImage)
    {
      this.isRightImage = isRightImage;
    }

    public Control CreateEditControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      TextBox textBox = new TextBox();
      textBox.BorderStyle = BorderStyle.Fixed3D;
      textBox.AutoSize = false;
      textBox.Visible = true;
      textBox.Size = new Size(cellRectangle.Size.Width, cellRectangle.Size.Height);
      textBox.Font = SynchronizerHlp.GetFont(column, row);
      textBox.Text = SynchronizerHlp.ValueToString(column, row, value);
      textBox.TextAlign = SynchronizerHlp.ConvertToHorizontalAlignment(SynchronizerHlp.GetAlignment(column, row));
      if (SynchronizerHlp.IsNoDelayApply(column))
        textBox.TextChanged += new EventHandler(textBox_TextChanged);

      bool isMultiline = SynchronizerHlp.IsMultilineCell(column, row);
      if (isMultiline)
      {
        textBox.Multiline = true;
        textBox.WordWrap = false;
      }
      return textBox;
    }

    void textBox_TextChanged(object sender, EventArgs e)
    {
      Control control = (Control)sender;
      GridCell gridCell = (GridCell)control.Tag;
      PushedValue(control, gridCell.Property, gridCell.DataItem);
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      Image image = SynchronizerHlp.GetImageForCellOrValue(column, row, value);

      int border = (int)SynchronizerHlp.GetBorder(column);
      if (border != 0)
        cellRectangle = new Rectangle(cellRectangle.X + 1 + border, cellRectangle.Y + border,
          cellRectangle.Width - 1 - border * 2, cellRectangle.Height - border * 2);

      SynchronizerHlp.PaintImageWithTextCell(graphics, cellRectangle, column, row, value, image, isRightImage);
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
      TextBox textBox = control as TextBox;
      if (textBox == null)
        Logger.AddMessage("TextGridCellFiller обрабатывает только TextBox'ы");
      else
        column.SetValue(row, textBox.Text);
    }
  }
}
