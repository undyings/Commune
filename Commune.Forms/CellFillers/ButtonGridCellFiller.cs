using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public class ButtonGridCellFiller : IGridCellFiller
  {
    public readonly static ButtonGridCellFiller Default =
      new ButtonGridCellFiller(GridWindowsFormsButtonCreator.Default);

    readonly IGridControlCreator buttonCreator;
    public ButtonGridCellFiller(IGridControlCreator buttonCreator)
    {
      this.buttonCreator = buttonCreator;
    }

    public Control CreateEditControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      Control button = buttonCreator.CreateControl(cellRectangle, column, row, value);

      button.Click += new EventHandler(delegate (object sender, EventArgs e)
      {
        column.SetValue(row, null);
      });
      column.SetValue(row, null);
      return button;
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      using (Control button = buttonCreator.CreateControl(cellRectangle, column, row, value))
      using (Bitmap bitmap = new Bitmap(button.Width, button.Height))
      {
        button.DrawToBitmap(bitmap, new Rectangle(0, 0, button.Width, button.Height));
        //bitmap.MakeTransparent();
        graphics.DrawImage(bitmap, cellRectangle);
      }
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
    }
  }
}
