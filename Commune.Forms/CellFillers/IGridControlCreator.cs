using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public interface IGridControlCreator
  {
    Control CreateControl(Rectangle cellRectangle, IGridColumn column, object row, object value);
  }

  public class GridWindowsFormsButtonCreator : IGridControlCreator
  {
    public readonly static IGridControlCreator Default = new GridWindowsFormsButtonCreator();

    public Control CreateControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      Button button = new Button();
      button.Text = SynchronizerHlp.ValueToString(column, row, value);
      button.AutoSize = false;
      button.Size = cellRectangle.Size;
      button.Font = SynchronizerHlp.GetFont(column, row);

      Image image = SynchronizerHlp.GetImageForCellOrValue(column, row, column.GetValue(row));
      if (image != null)
      {
        button.Image = image;
        button.ImageAlign = ContentAlignment.MiddleCenter;
      }

      Color? backColor = SynchronizerHlp.GetBackColor(column, row);
      if (backColor != null)
        button.BackColor = backColor.Value;

      return button;
    }
  }
}
