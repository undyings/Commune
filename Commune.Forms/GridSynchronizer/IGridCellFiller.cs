using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public interface IGridCellFiller
  {
    Control CreateEditControl(Rectangle cellRectagle, IGridColumn column, object row, object value);
    void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value);
    void PushedValue(Control control, IGridColumn column, object row);
  }

  public class GridCell
  {
    public readonly IGridColumn Property;
    public readonly int ColumnIndex;
    public readonly object DataItem;
    public readonly int RowIndex;
    public string Message = null;
    public object OldValue = null;

    public GridCell(IGridColumn property, int columnIndex, object dataItem, int rowIndex)
    {
      this.Property = property;
      this.ColumnIndex = columnIndex;
      this.DataItem = dataItem;
      this.RowIndex = rowIndex;
      this.OldValue = Property.GetValue(dataItem);
    }
  }

  public interface ICompositeCellPainter
  {
    void PaintCompositeCell(VirtualGridSynchronizer gridSynch, Graphics graphics, Rectangle cellRectInCells, Rectangle cellRectangle, IGridColumn column, object row, object value);
  }

  public class EmptyGridCellFiller : IGridCellFiller
  {
    public static readonly EmptyGridCellFiller Default = new EmptyGridCellFiller();

    static EmptyGridCellFiller()
    {
    }

    public Control CreateEditControl(Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      return (Control)null;
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      Color? backColor = SynchronizerHlp.GetBackColor(column, row);
      if (!backColor.HasValue)
        return;
      graphics.FillRectangle((Brush)new SolidBrush(backColor.Value), cellRectangle);
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
    }
  }
}
