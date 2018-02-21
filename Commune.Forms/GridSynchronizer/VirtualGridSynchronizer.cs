using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Commune.Basis;
using System.Drawing.Drawing2D;

namespace Commune.Forms
{
  public class VirtualGridSynchronizer : BaseVirtualGridSynchronizer
  {
    public Getter<object> DataLink;
    public Getter<IList<IGridColumn>> ColumnsGetter;
    public Getter<object, object> RowKeyGetter;
    public Getter<object> FilterRowGetter;
    private IGridColumn defaultFilterColumn;
    private int filterSeparatorHeight;
    private IGridColumn[] tempColumns = new IGridColumn[0];
    private IList tempRows = new object[0];
    public string CaptionForEmptyFilter;
    public bool CaptionForEmptyFilterEnabled;
    public string CaptionForEmptyGrid;

    public VirtualGridDrawSettings DrawSettings
    {
      get
      {
        return this.drawSettings;
      }
    }

    public IList Rows
    {
      get
      {
        if (this.DataLink == null)
          return (IList)null;
        else
          return this.DataLink() as IList;
      }
    }

    public ICollection SelectedRows
    {
      get
      {
        List<object> list = new List<object>();
        foreach (int index in this.selectedRowIndexByKey.Values)
        {
          if (index < this.RowsCount)
            list.Add(this.Rows[index]);
        }
        return (ICollection)list.ToArray();
      }
    }

    public object SelectedRow
    {
      get
      {
        int? minIndex = null;
        try
        {
          foreach (int index in selectedRowIndexByKey.Values)
          {
            if (minIndex == null || minIndex.Value > index)
              minIndex = index;
          }
          return (minIndex != null && minIndex < tempRows.Count) ? tempRows[minIndex.Value] : null;
        }
        catch
        {
          Logger.AddMessage("MinIndex: '{0}'", minIndex);
          throw;
        }
      }
      set
      {
        if (!MultiSelect)
          ClearSelection();

        object rowKey = RowKeyGetter != null ? RowKeyGetter(value) : value;
        for (int i = 0; i < tempRows.Count; ++i)
        {
          if (_.Equals(GetRowKey(i), rowKey))
          {
            SelectRow(i);
            break;
          }
        }
        RowPanel.Invalidate();
      }
    }

    public bool FilterEnabled
    {
      get
      {
        return this.FilterRowGetter != null;
      }
    }

    public IGridColumn DefaultFilterColumn
    {
      get
      {
        if (this.defaultFilterColumn == null)
          this.defaultFilterColumn = (IGridColumn)new SimpleColumn<object>("", (Getter<object, object>)delegate
          {
            return (object)"";
          }, new ColumnExtensionAttribute[1]
          {
            GridExt.CellBackColor(Color.LightGray)
          });
        return this.defaultFilterColumn;
      }
      set
      {
        if (value == null)
          return;
        this.defaultFilterColumn = value;
      }
    }

    public override int HeaderAndFilterHeight
    {
      get
      {
        int height = this.HeaderPanel.Height;
        if (this.FilterEnabled)
          height += this.RowHeight + this.filterSeparatorHeight;
        return height;
      }
    }

    public override int RowsCount
    {
      get
      {
        return this.tempRows.Count;
      }
    }

    public override IGridColumn[] Columns
    {
      get
      {
        return this.tempColumns;
      }
    }

    public VirtualGridSynchronizer(Panel rowPanel)
      : this(rowPanel, (IVirtualGridControls)new WindowsFormsGridHeader(1), (Getter<object>)null, (Getter<IList<IGridColumn>>)null)
    {
      this.HeaderPanel.Height = this.RowHeight + 2;
    }

    public VirtualGridSynchronizer(Panel rowPanel, IVirtualGridControls gridControls)
      : this(rowPanel, gridControls, (Getter<object>)null, (Getter<IList<IGridColumn>>)null)
    {
    }

    public VirtualGridSynchronizer(Panel rowPanel, IVirtualGridControls gridControls, Getter<object> dataLink, Getter<IList<IGridColumn>> columnsGetter)
      : base(rowPanel, gridControls, gridControls.DrawSettings)
    {
      this.DataLink = dataLink;
      this.ColumnsGetter = columnsGetter;
      if (this.HeaderPanel.Height == 0)
        this.HeaderPanel.Height = this.RowHeight;
      this.RowPanel.MouseMove += new MouseEventHandler(this.panel_MouseMove);
      this.RowPanel.Paint += new PaintEventHandler(this.panel_Paint);
    }

    public bool SelectAndMakeVisibleRow(object row)
    {
      this.SelectedRow = row;
      if (!this.SelectedRowIndex.HasValue)
        return false;
      this.MakeVisibleCell(0, this.SelectedRowIndex.Value);
      return true;
    }

    public object GetRowAt(int index)
    {
      if (this.tempRows != null && 0 <= index && index < this.tempRows.Count)
        return this.tempRows[index];
      else
        return (object)null;
    }

    protected override int GetColumnIndex(int propertyIndex, int itemIndex)
    {
      return propertyIndex;
    }

    protected override int GetRowIndex(int propertyIndex, int itemIndex)
    {
      return itemIndex;
    }

    protected override void GoToNextCell(GridCell editCell)
    {
      for (int index = editCell.RowIndex; index < this.RowsCount + editCell.RowIndex + 1; ++index)
      {
        int rowIndex = index % this.RowsCount;
        object obj = this.Rows[rowIndex];
        int num1 = index == editCell.RowIndex ? editCell.ColumnIndex + 1 : 0;
        int num2 = index == this.RowsCount + editCell.RowIndex + 1 ? editCell.ColumnIndex : this.ColumnsCount;
        for (int columnIndex = num1; columnIndex < num2; ++columnIndex)
        {
          IGridColumn gridColumn = this.Columns[columnIndex];
          if (!SynchronizerHlp.IsReadOnlyCell(gridColumn, obj))
          {
            this.FinishEditing(false, new GridCell(gridColumn, columnIndex, obj, rowIndex));
            if (!this.IsVisibleCell(columnIndex, rowIndex))
              this.MakeVisibleCell(columnIndex, rowIndex);
            this.SelectedRow = obj;
            return;
          }
        }
      }
    }

    protected override object GetRowKey(int rowIndex)
    {
      if (rowIndex < 0 || rowIndex >= this.tempRows.Count)
        return (object)null;
      object obj = this.tempRows[rowIndex];
      if (this.RowKeyGetter != null)
        return this.RowKeyGetter(obj);
      else
        return obj;
    }

    public IGridColumn GetColumnAt(int index)
    {
      if (this.tempColumns != null && 0 <= index && index < this.tempColumns.Length)
        return this.tempColumns[index];
      else
        return (IGridColumn)null;
    }

    private IGridColumn GetFilterColumn(IGridColumn column)
    {
      return SynchronizerHlp.GetFilter(column) ?? this.DefaultFilterColumn;
    }

    protected override GridCell GetGridCell(int columnIndex, int rowIndex)
    {
      if (rowIndex < -1 || rowIndex >= this.RowsCount || (columnIndex < 0 || columnIndex >= this.Columns.Length))
        return (GridCell)null;
      IGridColumn gridColumn = this.tempColumns[columnIndex];
      object dataItem;
      if (rowIndex == -1)
      {
        gridColumn = this.GetFilterColumn(gridColumn);
        dataItem = this.FilterRowGetter();
      }
      else
        dataItem = this.tempRows[rowIndex];
      return new GridCell(gridColumn, columnIndex, dataItem, rowIndex);
    }

    protected override int GetPropertyIndex(string propertyName)
    {
      int num = -1;
      foreach (IGridColumn gridColumn in this.tempColumns)
      {
        ++num;
        if (gridColumn.Name == propertyName)
          return num;
      }
      return -1;
    }

    protected override int GetDataItemIndex(object dataItem)
    {
      return this.tempRows.IndexOf(dataItem);
    }

    protected override void Synchronize(bool requieredInvalidate)
    {
      if (this.DataLink == null || this.ColumnsGetter == null)
        return;
      IList list1 = this.DataLink() as IList;
      IList<IGridColumn> list2 = this.ColumnsGetter();
      if (list1 == null || list2 == null)
        return;
      IList<IGridColumn> currentColumns = (IList<IGridColumn>)new List<IGridColumn>();
      foreach (IGridColumn column in (IEnumerable<IGridColumn>)list2)
      {
        if (SynchronizerHlp.IsVisible(column))
          currentColumns.Add(column);
      }
      if (this.CheckObsoleteColumns(currentColumns))
      {
        requieredInvalidate = true;
        this.tempColumns = CollectionHlp.ToArray<IGridColumn>((ICollection<IGridColumn>)currentColumns);
      }
      this.tempRows = list1;
      if (this.SyncView())
        requieredInvalidate = true;
      this.CheckAndUpdateSelectedRows();
      int displayRowsCount = this.DisplayedRowCount(true);
      if (!this.gridCache.IsValid((IList<IGridColumn>)this.tempColumns, this.tempRows, this.FirstDisplayedScrollingRowIndex, displayRowsCount))
      {
        this.gridCache.Refresh((IList<IGridColumn>)this.tempColumns, this.tempRows, this.FirstDisplayedScrollingRowIndex, displayRowsCount);
        requieredInvalidate = true;
      }
      if (!requieredInvalidate)
        return;
      this.RowPanel.Invalidate();
    }

    private void panel_MouseMove(object sender, MouseEventArgs e)
    {
      Point? cell = this.GetCellByClientCoord(this.ReferencePoint(), e.Location);
      if (cell.HasValue && (cell.Value.Y == -1 && this.FilterRowGetter() == null && (this.EditingControl == null || ((GridCell)this.EditingControl.Tag).RowIndex != -1)))
        cell = new Point?();
      bool flag = false;
      if (cell.HasValue)
      {
        GridCell gridCell = this.GetGridCell(cell.Value.X, cell.Value.Y);
        Rectangle cellRectangle = this.GetCellRectangle(this.ReferencePoint(), cell.Value.X, cell.Value.Y);
        string toolTipCaption = SynchronizerHlp.GetCellDescription(gridCell.Property, gridCell.DataItem);
        string text = SynchronizerHlp.GetValue(gridCell.Property, gridCell.DataItem) as string ?? "";
        Font font = SynchronizerHlp.GetFont(gridCell.Property, gridCell.DataItem);
        if ((double)this.graphics.MeasureString(text, font).Width >= (double)(cellRectangle.Width - 4))
        {
          if (!StringHlp.IsEmpty(toolTipCaption))
            toolTipCaption = string.Format("{0}\n{1}", text, toolTipCaption);
          else
            toolTipCaption = text;
        }
        if (toolTipCaption != null && toolTipCaption != "")
        {
          Point point = this.RowPanel.PointToScreen(new Point(0, 0));
          this.toolTipShower.TakeUpEvent((object)new ToolTipArgs(new Point(point.X + e.X, point.Y + e.Y), new Point(4, 44), toolTipCaption));
          flag = true;
        }
      }
      if (flag || !this.toolTip.Active)
        return;
      this.toolTip.Active = false;
    }

    private void panel_Paint(object sender, PaintEventArgs e)
    {
      try
      {
        e.Graphics.SmoothingMode = SmoothingMode.None;
        Graphics graphics = e.Graphics;
        Point point = this.ReferencePoint();
        Rectangle visibleDataRegion = this.VisibleDataRegion;
        Rectangle visibleTableRectangle = this.VisibleTableRectangle;
        if (this.tempRows != null && this.tempRows.Count != 0)
        {
          int x = -1;
          int y = this.FirstDisplayedScrollingRowIndex;
          int displayRowCount = this.DisplayedRowCount(true);

          foreach (IGridColumn column in this.tempColumns)
          {
            ++x;
            Rectangle rawCellRectangle = this.GetRawCellRectangle(point, x, 0);
            if (this.CompositeCellEnabled || rawCellRectangle.X + rawCellRectangle.Width >= 0)
            {
              if (rawCellRectangle.X <= visibleDataRegion.X + visibleDataRegion.Width)
              {
                if (this.CompositeCellEnabled)
                {
                  if (displayRowCount > 0)
                  {
                    try
                    {
                      y = this.GetRealCell(new Point(x, y)).Value.Y;
                    }
                    catch
                    {
                      Logger.AddMessage("Cell.Error: {0}, {1}", (object)x, (object)y);
                      throw;
                    }
                  }
                }
                if (y != 0)
                {
                  int rowIndex = y - 1;
                  object row = this.tempRows[rowIndex];
                  Rectangle cellRectangle = this.GetCellRectangle(point, x, rowIndex);
                  if (cellRectangle.Width != 0)
                  {
                    Pen pen = SynchronizerHlp.GetHMarkingPen(column, row) ?? this.DrawSettings.GridMarkingPen;
                    graphics.DrawLine(pen, cellRectangle.Left, cellRectangle.Bottom, cellRectangle.Right, cellRectangle.Bottom);
                  }
                }
                for (int index = y; index < this.FirstDisplayedScrollingRowIndex + displayRowCount; ++index)
                {
                  Rectangle cellRect = this.GetCellRectangle(point, x, index);
                  if (cellRect.Width != 0)
                  {
                    //Brush brush = this.DrawSettings.OddRowBackBrush;
                    //if (index % 2 != 0)
                    //  brush = this.DrawSettings.EvenRowBackBrush;
                    object row = this.tempRows[index];
                    Brush brush = SynchronizerHlp.GetRowBackBrush(column, row, index, DrawSettings);

                    bool isSelectedRow = this.IsSelectRow(index) &&
                      !SynchronizerHlp.IsCellSelectionDisabled(column, row) &&
                      (!this.CompositeCellEnabled || cellRect.Height <= this.RowHeight);

                    if (!isSelectedRow)
                    {
                      Rectangle fillRect = cellRect;
                      if (DrawSettings.FillRowBackWithMarking)
                        fillRect = new Rectangle(cellRect.X, cellRect.Y,
                          cellRect.Width + 1, cellRect.Height + 1);
                      e.Graphics.FillRectangle(brush, fillRect);
                    }

                    if (!this.IsEditingCell(x, index))
                    {
                      object obj = SynchronizerHlp.GetValue(column, row);
                      if (column is SimpleNumberColumn)
                        obj = (object)(index + 1);
                      IGridCellFiller cellFiller = this.FindCellFiller(column, row);
                      if (this.CompositeCellEnabled && cellFiller is ICompositeCellPainter)
                      {
                        Size cellSizeInCells = this.GetCellSizeInCells(x, index);
                        ((ICompositeCellPainter)cellFiller).PaintCompositeCell(this, graphics, new Rectangle(new Point(x, index), cellSizeInCells), cellRect, column, row, obj);
                      }
                      else
                        cellFiller.PaintCell(graphics, cellRect, column, row, obj);
                    }
                    Pen pen1 = SynchronizerHlp.GetHMarkingPen(column, row) ?? this.DrawSettings.GridMarkingPen;
                    graphics.DrawLine(pen1, cellRect.Left, cellRect.Bottom, cellRect.Right, cellRect.Bottom);
                    if (!this.RelativeColumnWidthEnabled || this.tempColumns.Length > 1)
                    {
                      Pen pen2 = SynchronizerHlp.GetVMarkingPen(column, row) ?? this.DrawSettings.GridMarkingPen;
                      graphics.DrawLine(pen2, cellRect.Right, cellRect.Top, cellRect.Right, cellRect.Bottom);
                      if (!this.drawSettings.LeftBorderPaintDisabled && x == 0)
                        graphics.DrawLine(this.DrawSettings.GridBorderPen, cellRect.Left, cellRect.Top, cellRect.Left, cellRect.Bottom);
                    }

                    //if (isSelectedRow)
                    //{
                    //  graphics.FillRectangle(this.DrawSettings.SelectedCellsBrush,
                    //    new Rectangle(cellRect.X, cellRect.Y - 1,
                    //      cellRect.Width + (DrawSettings.FillRowBackWithMarking ? 1 : 0), cellRect.Height + 1));
                    //}
                  }
                }
              }
              else
                break;
            }
          }

          int firstDisplayed = this.FirstDisplayedScrollingRowIndex;
          foreach (int selectedIndex in selectedRowIndexByKey.Values)
          {
            if (selectedIndex < firstDisplayed || selectedIndex >= firstDisplayed + displayRowCount)
              continue;

            Rectangle rawCellRectangle = this.GetRawCellRectangle(point, 0, selectedIndex);
            graphics.FillRectangle(this.DrawSettings.SelectedCellsBrush,
              new Rectangle(visibleDataRegion.X, rawCellRectangle.Y,
                visibleDataRegion.Width, rawCellRectangle.Height + 1)
            );
          }
        }
        else if (!StringHlp.IsEmpty(this.CaptionForEmptyGrid))
        {
          Font defaultGridFont = this.DrawSettings.DefaultGridFont;
          int num = (int)graphics.MeasureString(this.CaptionForEmptyGrid, defaultGridFont).Width / Math.Max(1, visibleTableRectangle.Width - 4) + 1;
          Rectangle rect = new Rectangle(-point.X + this.GridControls.BorderWidth, this.HeaderAndFilterHeight, this.horizontalScroll.Maximum - this.GridControls.BorderWidth - 2, Math.Min(this.RowHeight * num, visibleDataRegion.Height));
          e.Graphics.FillRectangle(this.DrawSettings.InactiveFilterBackBrush, rect);
          e.Graphics.DrawRectangle(this.DrawSettings.GridMarkingPen, rect);
          e.Graphics.DrawString(this.CaptionForEmptyGrid, defaultGridFont, Brushes.Black, (RectangleF)rect, new StringFormat()
          {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
          });
        }
        if (this.cacheHScrollVisible && this.cacheVScrollVisible)
          e.Graphics.FillRectangle((Brush)new SolidBrush(this.RowPanel.BackColor), new Rectangle(this.RowPanel.Width - this.verticalScroll.Control.Width, this.RowPanel.Height - this.horizontalScroll.Control.Height, this.verticalScroll.Control.Width, this.horizontalScroll.Control.Height));
        if (this.FilterEnabled)
        {
          object row = this.FilterRowGetter();
          Rectangle rect1 = new Rectangle(-point.X + this.GridControls.BorderWidth, this.HeaderHeight, this.horizontalScroll.Maximum - this.GridControls.BorderWidth - 2, this.RowHeight - 1);
          if (this.CaptionForEmptyFilterEnabled && row == null && (this.EditingControl == null || ((GridCell)this.EditingControl.Tag).RowIndex != -1))
          {
            e.Graphics.FillRectangle(this.DrawSettings.InactiveFilterBackBrush, rect1);
            e.Graphics.DrawString(this.CaptionForEmptyFilter, new Font(this.DrawSettings.DefaultGridFont, FontStyle.Italic), Brushes.Black, (RectangleF)rect1, new StringFormat()
            {
              Alignment = StringAlignment.Center,
              LineAlignment = StringAlignment.Center
            });
          }
          else
          {
            e.Graphics.FillRectangle(this.DrawSettings.OddRowBackBrush, rect1);
            int columnIndex = -1;
            foreach (IGridColumn column in this.tempColumns)
            {
              ++columnIndex;
              IGridColumn filterColumn = this.GetFilterColumn(column);
              Rectangle cellRectangle = this.GetCellRectangle(point, columnIndex, -1);
              if (cellRectangle.Width != 0)
              {
                object obj = SynchronizerHlp.GetValue(filterColumn, row);
                this.FindCellFiller(filterColumn, row).PaintCell(graphics, cellRectangle, filterColumn, row, obj);
                e.Graphics.DrawLine(this.DrawSettings.GridMarkingPen, cellRectangle.Right, cellRectangle.Top, cellRectangle.Right, cellRectangle.Bottom);
              }
            }
          }
          e.Graphics.DrawRectangle(this.DrawSettings.GridBorderPen, rect1);
          Rectangle rect2 = new Rectangle(rect1.X, this.HeaderHeight + this.RowHeight, rect1.Width + 1, this.filterSeparatorHeight);
          e.Graphics.FillRectangle(this.DrawSettings.InactiveFilterBackBrush, rect2);
          e.Graphics.DrawLine(this.DrawSettings.GridBorderPen, rect1.Left, rect2.Bottom, rect1.Right, rect2.Bottom);
        }
        this.HeaderPanel.Location = new Point(-point.X, 0);
        if (this.EditingControl != null)
        {
          GridCell gridCell = (GridCell)this.EditingControl.Tag;
          this.EditingControl.Bounds = this.GetCellRectangle(point, gridCell.ColumnIndex, gridCell.RowIndex);
        }
        if (!this.IsCellWithMessage)
          return;
        this.ShowCellMessage(point);
      }
      catch (Exception ex)
      {
        Logger.WriteException(ex);
      }
    }
  }
}
