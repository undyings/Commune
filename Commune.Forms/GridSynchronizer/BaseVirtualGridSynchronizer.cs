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
  public abstract class BaseVirtualGridSynchronizer : ControlSynchronizer
  {
    protected readonly ToolTip toolTip = new ToolTip();
    protected readonly NotifierWithDelay toolTipShower = new NotifierWithDelay(TimeSpan.FromMilliseconds(250.0));
    protected readonly ToolTip messageToolTip = new ToolTip();
    private Dictionary<string, int> columnWidthByName = new Dictionary<string, int>();
    public TimeSpan MessageShowDelay = TimeSpan.FromSeconds(3.0);
    private DateTime setMessageTime = DateTime.UtcNow;
    private float heightMultiplier = 1.8f;
    protected readonly Dictionary<object, int> selectedRowIndexByKey = new Dictionary<object, int>();
    protected readonly InternalGridCache gridCache = new InternalGridCache();
    private TimeSpan _updateInterval = TimeSpan.FromSeconds(1.0);
    public int DefaultMinColumnWidth = 31;
    private readonly Dictionary<string, double> relativeWidthByColumnName = new Dictionary<string, double>();
    private readonly Dictionary<string, int> maxColumnWidthByName = new Dictionary<string, int>();
    public readonly Panel RowPanel;
    public readonly Graphics graphics;
    public IGridColumn DefaultRowSettings;
    public readonly IVirtualGridControls GridControls;
    protected readonly VirtualGridDrawSettings drawSettings;
    private readonly LazyMaker headersMaker;
    private readonly LazyMaker columnsWidthMaker;
    private long columnsChangeTick;
    private long userColumnWidthChangeTick;
    public Getter<bool, int> SelectionRowEnabler;
    public Executter SelectionRowInformer;
    private GridCell _cellWithMessage;
    private Control editingControl;
    public bool GoToNextCellByEnterEnabled;
    protected bool cacheVScrollVisible;
    protected bool cacheHScrollVisible;
    protected int maxDisplayedRowCount;
    private long selectedRowsChangeTick;
    public bool NotClearDeleteRowSelectionEnabled;
    public bool SelectionDisabled;
    public bool MultiSelect;
    public bool CompositeCellEnabled;
    public bool RelativeColumnWidthEnabled;
    private int cacheVisibleDataRegionWidth;

    public abstract IGridColumn[] Columns { get; }

    protected GridCell cellWithMessage
    {
      get
      {
        return this._cellWithMessage;
      }
      set
      {
        this._cellWithMessage = value;
        this.setMessageTime = DateTime.UtcNow;
      }
    }

    protected bool IsCellWithMessage
    {
      get
      {
        if (this.cellWithMessage != null)
          return true;
        else
          return false;
      }
    }

    public GridCell EditingControlCell
    {
      get
      {
        if (this.EditingControl == null)
          return (GridCell)null;
        else
          return (GridCell)this.EditingControl.Tag;
      }
    }

    protected Control EditingControl
    {
      get
      {
        if (this.editingControl != null)
        {
          GridCell gridCell1 = (GridCell)this.editingControl.Tag;
          GridCell gridCell2 = this.GetGridCell(gridCell1.ColumnIndex, gridCell1.RowIndex);
          if (gridCell2 == null || !SynchronizerHlp.IsNoDelayApply(gridCell1.Property) && !object.Equals(gridCell1.OldValue, gridCell2.Property.GetValue(gridCell2.DataItem)))
            this.EditingControl = gridCell2 != null ? this.CreateEditingControl(gridCell2) : (Control)null;
        }
        return this.editingControl;
      }
      set
      {
        if (this.toolTip.Active)
          this.toolTip.Active = false;
        if (this.editingControl != null && this.editingControl != value)
        {
          this.editingControl.Leave -= new EventHandler(this.editingControl_Leave);
          this.editingControl.KeyDown -= new KeyEventHandler(this.editingControl_KeyDown);
          this.editingControl.Dispose();
          this.RowPanel.Controls.Remove(this.editingControl);
          this.editingControl = (Control)null;
        }
        if (value != null)
        {
          this.RowPanel.Focus();
          if (this.RowPanel.Focused)
          {
            this.RowPanel.Controls.Add(value);
            value.Focus();
            value.Leave += new EventHandler(this.editingControl_Leave);
            value.KeyDown += new KeyEventHandler(this.editingControl_KeyDown);
          }
          else
          {
            value.Dispose();
            value = (Control)null;
          }
        }
        this.editingControl = value;
        this.RowPanel.Invalidate();
      }
    }

    protected ISimpleScroll verticalScroll
    {
      get
      {
        return this.GridControls.VScroll;
      }
    }

    protected ISimpleScroll horizontalScroll
    {
      get
      {
        return this.GridControls.HScroll;
      }
    }

    protected Control HeaderPanel
    {
      get
      {
        return this.GridControls.HeaderPanel;
      }
    }

    public int HeaderHeight
    {
      get
      {
        return this.HeaderPanel.Height;
      }
      set
      {
        if (value < 0)
          return;
        this.HeaderPanel.Height = value;
      }
    }

    public abstract int HeaderAndFilterHeight { get; }

    public float HeightMultiplier
    {
      get
      {
        return this.heightMultiplier;
      }
      set
      {
        this.heightMultiplier = value;
      }
    }

    public int RowHeight
    {
      get
      {
        return (int)((double)this.drawSettings.DefaultGridFont.GetHeight(this.graphics) * (double)this.heightMultiplier);
      }
    }

    public long SelectedRowsChangeTick
    {
      get
      {
        return this.selectedRowsChangeTick;
      }
    }

    public int? SelectedRowIndex
    {
      get
      {
        using (Dictionary<object, int>.ValueCollection.Enumerator enumerator = this.selectedRowIndexByKey.Values.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return new int?(enumerator.Current);
        }
        return new int?();
      }
    }

    public abstract int RowsCount { get; }

    public int ColumnsCount
    {
      get
      {
        return this.Columns.Length;
      }
    }

    public int FirstDisplayedScrollingRowIndex
    {
      get
      {
        return this.verticalScroll.Value;
      }
      set
      {
        this.Synchronize(true);
        this.verticalScroll.Value = Math.Max(0, Math.Min(value, Math.Min(this.verticalScroll.Maximum, this.verticalScroll.Maximum + 1 - this.maxDisplayedRowCount)));
      }
    }

    public Rectangle VisibleDataRegion
    {
      get
      {
        return this.GetVisibleDataRegion(this.cacheVScrollVisible, this.cacheHScrollVisible);
      }
    }

    public Rectangle VisibleTableRectangle
    {
      get
      {
        Rectangle visibleDataRegion = this.VisibleDataRegion;
        return new Rectangle(visibleDataRegion.X, visibleDataRegion.Y, Math.Min(visibleDataRegion.Width, this.horizontalScroll.Maximum + 1 - this.GridControls.BorderWidth - this.GridControls.SplitterWidth / 2 - this.horizontalScroll.Value), Math.Min(visibleDataRegion.Height, this.DisplayedRowCount(true) * this.RowHeight));
      }
    }

    protected override Control Control_ForSync
    {
      get
      {
        return (Control)this.RowPanel;
      }
    }

    protected override TimeSpan UpdateInterval_ForSync
    {
      get
      {
        return this.UpdateInterval;
      }
    }

    public TimeSpan UpdateInterval
    {
      get
      {
        return this._updateInterval;
      }
      set
      {
        this._updateInterval = value;
        this.UpdateTimer(true);
      }
    }

    protected BaseVirtualGridSynchronizer(Panel rowPanel, IVirtualGridControls gridControls, VirtualGridDrawSettings drawSettings)
    {
      BaseVirtualGridSynchronizer gridSynchronizer = this;
      this.RowPanel = rowPanel;
      this.GridControls = gridControls;
      this.drawSettings = drawSettings;
      rowPanel.AutoScroll = false;
      this.graphics = rowPanel.CreateGraphics();

      this.headersMaker = new LazyMaker<long>(delegate
      {
        this.ResetCellWithMessage();
        IGridColumn[] columns = this.Columns;
        for (int index = 0; index < columns.Length; ++index)
        {
          IGridColumn gridColumn = columns[index];
          if (gridColumn is ISupportDefaultExtensionColumn && gridColumn.GetExtended("DefaultRowSettings") == null)
            ((ISupportDefaultExtensionColumn)gridColumn).WithExtension(GridExt.DefaultRowSettings((Getter<IGridColumn>)(() => this.DefaultRowSettings)));
        }
        this.GridControls.RecreateGridHeader((IList<IGridColumn>)columns);
      },
        delegate { return columnsChangeTick; }
      );

      this.columnsWidthMaker = new LazyMaker<long, Size, int, long>(
        delegate
        {
          int minWidth = 0;
          if (this.RelativeColumnWidthEnabled)
          {
            foreach (IColumnHeader columnHeader in this.GridControls.ColumnHeaders)
            {
              foreach (int index in columnHeader.ColumnIndices)
                minWidth += this.GetMinColumnWidth(this.Columns[index]);
            }
          }
          else
          {
            foreach (IColumnHeader columnHeader in this.GridControls.ColumnHeaders)
            {
              foreach (int index in columnHeader.ColumnIndices)
              {
                IGridColumn column = this.Columns[index];
                int minColumnWidth;
                if (!this.columnWidthByName.TryGetValue(column.Name, out minColumnWidth))
                  minColumnWidth = this.GetMinColumnWidth(column);
                minWidth += minColumnWidth;
              }
            }
          }
          Tuple<bool, bool> scrollVisible = this.GetScrollVisible(minWidth);
          this.SetWidthColumns(scrollVisible.Item1, scrollVisible.Item2);
          this.SyncScroll(scrollVisible.Item1, scrollVisible.Item2);
        },
        delegate { return this.headersMaker.ChangeTick; },
        delegate { return rowPanel.Size; },
        delegate { return this.RowsCount; },
        delegate { return this.userColumnWidthChangeTick; }
      );
      rowPanel.Resize += new EventHandler(this.panel_Resize);
      rowPanel.MouseLeave += new EventHandler(this.panel_MouseLeave);
      rowPanel.MouseClick += new MouseEventHandler(this.panel_MouseClick);
      rowPanel.MouseDoubleClick += new MouseEventHandler(this.rowPanel_MouseDoubleClick);
      rowPanel.MouseWheel += new MouseEventHandler(this.panel_MouseWheel);
      rowPanel.Disposed += new EventHandler(this.rowPanel_Disposed);
      this.toolTipShower.NotifyEvent += new EventHandler(this.toolTipShower_NotifyEvent);
      this.RowPanel.Controls.Add(this.verticalScroll.Control);
      this.verticalScroll.Control.Visible = false;
      this.verticalScroll.ValueChanged += new EventHandler(this.verticalScroll_ValueChanged);
      this.verticalScroll.Minumum = 0;
      this.RowPanel.Controls.Add(this.horizontalScroll.Control);
      this.horizontalScroll.Control.Visible = false;
      this.horizontalScroll.ValueChanged += new EventHandler(this.horizontalScroll_ValueChanged);
      this.horizontalScroll.Minumum = 0;
      this.HeaderPanel.Location = new Point(0, 0);
      this.RowPanel.Controls.Add(this.HeaderPanel);
      this.HeaderPanel.SendToBack();
      this.GridControls.ColumnWidthChanged += new ColumnHeaderEventHandler(this.GridControls_ColumnWidthChanged);
      this.UpdateTimer(true);
    }

    private void rowPanel_Disposed(object sender, EventArgs e)
    {
      this.toolTip.Dispose();
      this.messageToolTip.Dispose();
    }

    private void panel_Resize(object sender, EventArgs e)
    {
      this.Synchronize(true);
    }

    private void panel_MouseLeave(object sender, EventArgs e)
    {
      this.toolTip.Active = false;
    }

    private void panel_MouseWheel(object sender, MouseEventArgs e)
    {
      this.FirstDisplayedScrollingRowIndex = this.verticalScroll.Value - e.Delta / SystemInformation.MouseWheelScrollDelta;
    }

    private void rowPanel_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      Point? cellByClientCoord = this.GetCellByClientCoord(this.ReferencePoint(), e.Location);
      if (!cellByClientCoord.HasValue)
        return;
      GridCell gridCell = this.GetGridCell(cellByClientCoord.Value.X, cellByClientCoord.Value.Y);
      if (gridCell == null || !SynchronizerHlp.MakeCellDoubleClick(gridCell.Property, gridCell.DataItem))
        return;
      this.RowPanel.Invalidate();
    }

    public bool EditOnlyForSelectRow = false;

    private void panel_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
        this.RowPanel.Focus();
      Point? cellByClientCoord = this.GetCellByClientCoord(this.ReferencePoint(), e.Location);
      if (!cellByClientCoord.HasValue)
        return;
      int x = cellByClientCoord.Value.X;
      int y = cellByClientCoord.Value.Y;
      if (e.Button != MouseButtons.Left)
        return;
      GridCell gridCell = this.GetGridCell(x, y);
      if (this.SelectionRowEnabler != null && !this.SelectionRowEnabler(y))
        return;
      GridCell newEditGridCell = gridCell;
      if (EditOnlyForSelectRow && !IsSelectRow(y))
        newEditGridCell = null;
      if (this.FinishEditing(true, newEditGridCell) && !this.SelectionDisabled && y != -1)
      {
        if (Control.ModifierKeys == Keys.Shift && this.MultiSelect)
        {
          this.SelectRow(y);
          this.CheckAndUpdateSelectedRows();
        }
        else if (Control.ModifierKeys == Keys.Control)
        {
          if (this.IsSelectRow(y))
          {
            this.UnselectRow(y);
          }
          else
          {
            if (!this.MultiSelect)
              this.UnselectAllRows();
            this.SelectRow(y);
            if (this.MultiSelect)
              this.CheckAndUpdateSelectedRows();
          }
        }
        else
        {
          this.UnselectAllRows();
          this.SelectRow(y);
        }
        this.RowPanel.Invalidate();
      }
      if (gridCell == null)
        return;
      if (this.SelectionRowInformer != null)
        this.SelectionRowInformer();
      if (!this.GetCellRectangle(x, y).HasValue || !SynchronizerHlp.MakeCellClick(gridCell, e.Location))
        return;
      this.RowPanel.Invalidate();
    }

    protected abstract GridCell GetGridCell(int columnIndex, int rowIndex);

    public bool SetEditCellMessage(ToolTipIcon toolTipIcon, string title, string editCellMessage)
    {
      if (this.EditingControl == null)
        return false;
      this.cellWithMessage = (GridCell)this.EditingControl.Tag;
      this.cellWithMessage.Message = editCellMessage;
      this.messageToolTip.ToolTipIcon = toolTipIcon;
      this.messageToolTip.ToolTipTitle = title;
      if (!this.IsVisibleCell(this.cellWithMessage.ColumnIndex, this.cellWithMessage.RowIndex))
        this.MakeVisibleCell(this.cellWithMessage.ColumnIndex, this.cellWithMessage.RowIndex);
      this.RowPanel.Invalidate();
      return true;
    }

    public bool SetEditCellMessage(string editCellMessage)
    {
      return this.SetEditCellMessage(ToolTipIcon.None, "", editCellMessage);
    }

    protected abstract int GetPropertyIndex(string propertyName);

    protected abstract int GetDataItemIndex(object dataItem);

    public bool SetEditCell(string propertyName, object dataItem)
    {
      return this.SetCellMessage(propertyName, dataItem, ToolTipIcon.None, "", "");
    }

    public bool SetCellMessage(string propertyName, object dataItem, ToolTipIcon toolTipIcon, string title, string message)
    {
      int dataItemIndex = this.GetDataItemIndex(dataItem);
      int propertyIndex = this.GetPropertyIndex(propertyName);
      if (propertyIndex == -1 || dataItemIndex == -1)
        return false;
      this.messageToolTip.ToolTipIcon = toolTipIcon;
      this.messageToolTip.ToolTipTitle = title;
      int columnIndex = this.GetColumnIndex(propertyIndex, dataItemIndex);
      int rowIndex = this.GetRowIndex(propertyIndex, dataItemIndex);
      GridCell gridCell = this.GetGridCell(columnIndex, rowIndex);
      gridCell.Message = message;
      if (!this.FinishEditing(true, gridCell))
        return false;
      this.cellWithMessage = gridCell;
      if (this.EditingControl == null)
        this.RowPanel.Focus();
      if (!this.IsVisibleCell(columnIndex, rowIndex))
        this.MakeVisibleCell(columnIndex, rowIndex);
      return true;
    }

    protected abstract int GetColumnIndex(int propertyIndex, int itemIndex);

    protected abstract int GetRowIndex(int propertyIndex, int itemIndex);

    protected void ResetCellWithMessage()
    {
      this.cellWithMessage = (GridCell)null;
      this.messageToolTip.Active = false;
    }

    protected void ShowCellMessage(Point refPoint)
    {
      Rectangle cellRectangle = this.GetCellRectangle(refPoint, this.cellWithMessage.ColumnIndex, this.cellWithMessage.RowIndex);
      SynchronizerHlp.ShowToolTip(this.messageToolTip, this.cellWithMessage.Message, (Control)this.RowPanel, this.RowPanel.PointToScreen(new Point(cellRectangle.Right + 5, cellRectangle.Bottom + 2)));
    }

    protected bool IsEditingCell(int columnIndex, int rowIndex)
    {
      if (this.EditingControl == null)
        return false;
      GridCell gridCell = (GridCell)this.EditingControl.Tag;
      if (gridCell.ColumnIndex == columnIndex && gridCell.RowIndex == rowIndex)
        return true;
      else
        return false;
    }

    public void ApplyEditing()
    {
      this.ResetCellWithMessage();
      if (this.EditingControl == null)
        return;
      GridCell gridCell = (GridCell)this.EditingControl.Tag;
      this.FindCellFiller(gridCell.Property, gridCell.DataItem).PushedValue(this.EditingControl, gridCell.Property, gridCell.DataItem);
      gridCell.OldValue = gridCell.Property.GetValue(gridCell.DataItem);
    }

    public bool FinishEditing(bool applyChange, GridCell newEditCell)
    {
      if (applyChange)
        this.ApplyEditing();
      else
        this.ResetCellWithMessage();
      if (this.IsCellWithMessage)
        return false;
      this.EditingControl = this.CreateEditingControl(newEditCell);
      return true;
    }

    private Control CreateEditingControl(GridCell newEditCell)
    {
      if (newEditCell != null)
      {
        IGridColumn column = newEditCell.Property;
        object row = newEditCell.DataItem;
        if (!SynchronizerHlp.IsReadOnlyCell(column, row))
        {
          Rectangle cellRectangle = this.GetCellRectangle(this.ReferencePoint(), newEditCell.ColumnIndex, newEditCell.RowIndex);
          Control editControl = this.FindCellFiller(column, row).CreateEditControl(cellRectangle, column, row, SynchronizerHlp.GetValue(column, row));
          if (editControl != null)
          {
            editControl.Location = cellRectangle.Location;
            editControl.Tag = (object)newEditCell;
            return editControl;
          }
        }
      }
      return (Control)null;
    }

    protected abstract void GoToNextCell(GridCell editCell);

    private void editingControl_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == (Keys.LButton | Keys.MButton | Keys.Back))
      {
        this.ApplyEditing();
        if (this.EditingControl == null)
          return;
        GridCell editCell = (GridCell)this.EditingControl.Tag;
        SynchronizerHlp.MakeCellEnter(editCell.Property, editCell.DataItem);
        if (!this.GoToNextCellByEnterEnabled || this.IsCellWithMessage)
          return;
        this.GoToNextCell(editCell);
      }
      else
      {
        if (e.KeyCode != (Keys.LButton | Keys.RButton | Keys.Back | Keys.ShiftKey))
          return;
        this.FinishEditing(false, (GridCell)null);
      }
    }

    public IGridCellFiller FindCellFiller(IGridColumn column, object row)
    {
      return SynchronizerHlp.GetExtended(column, "CellFiller", row) as IGridCellFiller ?? this.drawSettings.DefaultGridCellFiller;
    }

    private void editingControl_Leave(object sender, EventArgs e)
    {
      this.FinishEditing(true, (GridCell)null);
    }

    private void horizontalScroll_ValueChanged(object sender, EventArgs e)
    {
      this.RowPanel.Invalidate();
    }

    private void verticalScroll_ValueChanged(object sender, EventArgs e)
    {
      this.RowPanel.Invalidate();
    }

    private void toolTipShower_NotifyEvent(object sender, EventArgs e)
    {
      ToolTipArgs toolTipArgs = (ToolTipArgs)sender;
      if (!(Control.MousePosition == toolTipArgs.MouseScreenLocation))
        return;
      SynchronizerHlp.ShowToolTip(this.toolTip, toolTipArgs.ToolTipCaption, (Control)this.RowPanel, new Point(toolTipArgs.MouseScreenLocation.X + toolTipArgs.ToolTipShift.X, toolTipArgs.MouseScreenLocation.Y + toolTipArgs.ToolTipShift.Y));
    }

    //public bool HorizontalScrollDisabled = false;

    private Tuple<bool, bool> GetScrollVisible(int minWidth)
    {
      Rectangle visibleDataRegion1 = this.GetVisibleDataRegion(false, false);
      Rectangle visibleDataRegion2 = this.GetVisibleDataRegion(true, true);

      bool? hScrollVisible = new bool?();
      if (minWidth < visibleDataRegion2.Width)
        hScrollVisible = new bool?(false);
      else if (minWidth > visibleDataRegion1.Width)
        hScrollVisible = new bool?(true);

      bool? vScrollVisible = new bool?();
      if (this.RowsCount < visibleDataRegion2.Height / this.RowHeight)
        vScrollVisible = new bool?(false);
      else if (this.RowsCount > visibleDataRegion1.Height / this.RowHeight)
        vScrollVisible = new bool?(true);

      if (!hScrollVisible.HasValue || !vScrollVisible.HasValue)
      {
        if (!hScrollVisible.HasValue && !vScrollVisible.HasValue)
        {
          hScrollVisible = new bool?(false);
          vScrollVisible = new bool?(false);
        }
        else if (!hScrollVisible.HasValue)
          hScrollVisible = vScrollVisible;
        else if (!vScrollVisible.HasValue)
          vScrollVisible = hScrollVisible;
      }
      return CollectionHlp.Tuple<bool, bool>(vScrollVisible.Value, hScrollVisible.Value);
    }

    private bool SyncScroll(bool vScrollVisible, bool hScrollVisible)
    {
      Rectangle visibleDataRegion = this.GetVisibleDataRegion(vScrollVisible, hScrollVisible);
      int val2 = this.GridControls.BorderWidth * 2;
      foreach (IColumnHeader columnHeader in this.GridControls.ColumnHeaders)
        val2 += columnHeader.Width;
      this.HeaderPanel.Width = Math.Max(visibleDataRegion.Width, val2 * 2);
      this.GridControls.EndChange();
      this.maxDisplayedRowCount = visibleDataRegion.Height / this.RowHeight;
      this.horizontalScroll.Maximum = Math.Max(0, val2 - 1);
      this.horizontalScroll.Control.Location = new Point(0, this.RowPanel.Height - this.horizontalScroll.Control.Height);
      this.verticalScroll.Maximum = Math.Max(0, this.RowsCount - 1);
      this.verticalScroll.Control.Location = new Point(this.RowPanel.Width - this.verticalScroll.Control.Width, 0);
      this.horizontalScroll.LargeChange = Math.Max(0, Math.Min(visibleDataRegion.Width, val2));
      if (this.horizontalScroll.Value < 0 || this.horizontalScroll.Value > val2 - this.horizontalScroll.LargeChange)
        this.horizontalScroll.Value = Math.Max(0, val2 - this.horizontalScroll.LargeChange);
      this.verticalScroll.LargeChange = Math.Max(0, this.maxDisplayedRowCount);
      if (this.verticalScroll.Value < 0 || this.verticalScroll.Value > this.verticalScroll.Maximum)
        this.verticalScroll.Value = 0;
      else if (this.verticalScroll.Value > Math.Max(0, this.RowsCount - this.verticalScroll.LargeChange))
        this.verticalScroll.Value = Math.Max(0, this.RowsCount - this.verticalScroll.LargeChange);
      if (hScrollVisible && vScrollVisible)
      {
        this.horizontalScroll.Control.Width = this.RowPanel.Width - this.verticalScroll.Control.Width;
        this.verticalScroll.Control.Height = this.RowPanel.Height - this.horizontalScroll.Control.Height;
      }
      else if (hScrollVisible)
        this.horizontalScroll.Control.Width = this.RowPanel.Width;
      else if (vScrollVisible)
        this.verticalScroll.Control.Height = this.RowPanel.Height;
      if (this.cacheHScrollVisible == hScrollVisible && this.cacheVScrollVisible == vScrollVisible)
        return false;
      this.horizontalScroll.Control.Visible = hScrollVisible;
      this.verticalScroll.Control.Visible = vScrollVisible;
      this.cacheHScrollVisible = hScrollVisible;
      this.cacheVScrollVisible = vScrollVisible;
      return true;
    }

    protected abstract object GetRowKey(int rowIndex);

    protected bool CheckAndUpdateSelectedRows()
    {
      bool flag = false;
      int num = -1;
      foreach (KeyValuePair<object, int> keyValuePair in this.selectedRowIndexByKey)
      {
        if (keyValuePair.Value < num || !object.Equals(keyValuePair.Key, this.GetRowKey(keyValuePair.Value)))
        {
          flag = true;
          break;
        }
        else
          num = keyValuePair.Value;
      }
      if (!flag)
        return true;
      Dictionary<object, int> dictionary = new Dictionary<object, int>();
      for (int rowIndex = 0; rowIndex < this.RowsCount; ++rowIndex)
      {
        object rowKey = this.GetRowKey(rowIndex);
        if (rowKey != null && this.selectedRowIndexByKey.ContainsKey(rowKey))
        {
          dictionary[rowKey] = rowIndex;
          if (dictionary.Count == this.selectedRowIndexByKey.Count)
            break;
        }
      }
      if (this.NotClearDeleteRowSelectionEnabled && this.RowsCount != 0 && dictionary.Count == 0)
      {
        int? nullable = CollectionHlp.MinInStructs<int>((IEnumerable<int>)this.selectedRowIndexByKey.Values);
        if (nullable.HasValue)
        {
          int rowIndex = Math.Min(this.RowsCount - 1, nullable.Value);
          object rowKey = this.GetRowKey(rowIndex);
          if (rowKey != null)
            dictionary[rowKey] = rowIndex;
        }
      }
      this.selectedRowIndexByKey.Clear();
      foreach (KeyValuePair<object, int> keyValuePair in dictionary)
        this.selectedRowIndexByKey[keyValuePair.Key] = keyValuePair.Value;
      ++this.selectedRowsChangeTick;
      return false;
    }

    protected bool IsSelectRow(int rowIndex)
    {
      object rowKey = this.GetRowKey(rowIndex);
      int num;
      if (rowKey != null && this.selectedRowIndexByKey.TryGetValue(rowKey, out num))
        return num == rowIndex;
      else
        return false;
    }

    protected void SelectRow(int rowIndex)
    {
      if (this.SelectionDisabled)
        return;
      object rowKey = this.GetRowKey(rowIndex);
      if (rowKey == null)
        return;
      this.selectedRowIndexByKey[rowKey] = rowIndex;
      ++this.selectedRowsChangeTick;
    }

    protected void UnselectRow(int rowIndex)
    {
      object rowKey = this.GetRowKey(rowIndex);
      if (rowKey == null)
        return;
      this.selectedRowIndexByKey.Remove(rowKey);
      ++this.selectedRowsChangeTick;
    }

    protected void UnselectAllRows()
    {
      if (this.selectedRowIndexByKey.Count == 0)
        return;
      this.selectedRowIndexByKey.Clear();
      ++this.selectedRowsChangeTick;
    }

    public void SelectAll()
    {
      if (!this.MultiSelect)
        return;
      for (int rowIndex = 0; rowIndex < this.RowsCount; ++rowIndex)
        this.SelectRow(rowIndex);
      this.RowPanel.Invalidate();
    }

    public void ClearSelection()
    {
      this.UnselectAllRows();
      this.RowPanel.Invalidate();
    }

    protected Point ReferencePoint()
    {
      return new Point(this.horizontalScroll.Value, this.verticalScroll.Value * this.RowHeight - this.HeaderAndFilterHeight);
    }

    protected Point? GetRealCell(Point rawCell)
    {
      int x;
      for (x = rawCell.X; x >= 0; --x)
      {
        GridCell gridCell = this.GetGridCell(x, rawCell.Y);
        if (SynchronizerHlp.IsRightFakeCell(gridCell.Property, gridCell.DataItem))
        {
          if (x == 0)
            return null;
        }
        else
        {
          if (SynchronizerHlp.IsCompositeCell(gridCell.Property, gridCell.DataItem))
            return new Point(x, rawCell.Y);
          break;
        }
        //else if (!SynchronizerHlp.IsLowerFakeCell(gridCell.Property, gridCell.DataItem))
        //{
        //  if (SynchronizerHlp.IsCompositeCell(gridCell.Property, gridCell.DataItem))
        //    return new Point(x, rawCell.Y);
        //  if (x == rawCell.X)
        //    return new Point?(new Point(x, rawCell.Y));
        //  else
        //    return new Point?();
        //}
        //else
        //  break;
      }
      for (int y = rawCell.Y; y >= 0; --y)
      {
        GridCell gridCell = this.GetGridCell(x, y);
        if (!SynchronizerHlp.IsLowerFakeCell(gridCell.Property, gridCell.DataItem))
        {
          //if (SynchronizerHlp.IsCompositeCell(gridCell.Property, gridCell.DataItem))
          return new Point(x, y);
          //else
          //  return null;
        }
      }
      return new Point?();
    }

    protected Size GetCellSizeInCells(int columnIndex, int rowIndex)
    {
      GridCell gridCell1 = this.GetGridCell(columnIndex, rowIndex);
      if (SynchronizerHlp.IsRightFakeCell(gridCell1.Property, gridCell1.DataItem) || SynchronizerHlp.IsLowerFakeCell(gridCell1.Property, gridCell1.DataItem))
        return new Size(0, 1);
      if (!SynchronizerHlp.IsCompositeCell(gridCell1.Property, gridCell1.DataItem))
        return new Size(1, 1);
      int width = 1;
      for (int columnIndex1 = columnIndex + 1; columnIndex1 < this.ColumnsCount; ++columnIndex1)
      {
        GridCell gridCell2 = this.GetGridCell(columnIndex1, rowIndex);
        if (SynchronizerHlp.IsRightFakeCell(gridCell2.Property, gridCell2.DataItem))
          ++width;
        else
          break;
      }
      int height = 1;
      for (int rowIndex1 = rowIndex + 1; rowIndex1 < this.RowsCount; ++rowIndex1)
      {
        GridCell gridCell2 = this.GetGridCell(columnIndex, rowIndex1);
        if (SynchronizerHlp.IsLowerFakeCell(gridCell2.Property, gridCell2.DataItem))
          ++height;
        else
          break;
      }
      return new Size(width, height);
    }

    public Point? GetCellPositionByClientCoord(Point clientCoord)
    {
      return this.GetCellByClientCoord(this.ReferencePoint(), clientCoord);
    }

    protected Point? GetCellByClientCoord(Point referencePoint, Point clientCoord)
    {
      Rectangle visibleDataRegion = this.VisibleDataRegion;
      if (clientCoord.Y <= this.HeaderPanel.Height)
        return new Point?();
      int y = clientCoord.Y > this.HeaderAndFilterHeight ? this.FirstDisplayedScrollingRowIndex + (clientCoord.Y - visibleDataRegion.Y) / this.RowHeight : -1;
      if (y >= this.RowsCount)
        return new Point?();
      for (int index = 0; index < this.ColumnsCount; ++index)
      {
        Rectangle rawCellRectangle = this.GetRawCellRectangle(referencePoint, index, 0);
        if (clientCoord.X >= rawCellRectangle.X && clientCoord.X < rawCellRectangle.X + rawCellRectangle.Width)
        {
          Point rawCell = new Point(index, y);
          if (this.CompositeCellEnabled)
            return this.GetRealCell(rawCell);
          else
            return new Point?(rawCell);
        }
      }
      return new Point?();
    }

    public Rectangle? GetCellRectangle(int columnIndex, int rowIndex)
    {
      if (columnIndex < 0 || columnIndex >= this.ColumnsCount || (rowIndex < 0 || rowIndex >= this.RowsCount))
        return new Rectangle?();
      else
        return new Rectangle?(this.GetCellRectangle(this.ReferencePoint(), columnIndex, rowIndex));
    }

    protected Rectangle GetCellRectangle(Point referencePoint, int columnIndex, int rowIndex)
    {
      Rectangle rawCellRectangle1 = this.GetRawCellRectangle(referencePoint, columnIndex, rowIndex);
      if (!this.CompositeCellEnabled)
        return rawCellRectangle1;
      Size cellSizeInCells = this.GetCellSizeInCells(columnIndex, rowIndex);
      if (cellSizeInCells.Width == 1 && cellSizeInCells.Height == 1)
        return rawCellRectangle1;
      int width = cellSizeInCells.Width != 0 ? rawCellRectangle1.Width : 0;
      for (int index = 1; index < cellSizeInCells.Width; ++index)
      {
        Rectangle rawCellRectangle2 = this.GetRawCellRectangle(referencePoint, columnIndex + index, rowIndex);
        width += rawCellRectangle2.Width + 1;
      }
      int height = rawCellRectangle1.Height;
      for (int index = 1; index < cellSizeInCells.Height; ++index)
      {
        Rectangle rawCellRectangle2 = this.GetRawCellRectangle(referencePoint, columnIndex, rowIndex + index);
        height += rawCellRectangle2.Height + 1;
      }
      return new Rectangle(rawCellRectangle1.Location, new Size(width, height));
    }

    public Rectangle GetRawCellRectangle(int columnIndex, int rowIndex)
    {
      return this.GetRawCellRectangle(this.ReferencePoint(), columnIndex, rowIndex);
    }

    protected Rectangle GetRawCellRectangle(Point referencePoint, int columnIndex, int rowIndex)
    {
      int borderWidth = this.GridControls.BorderWidth;
      for (int index = 0; index < columnIndex; ++index)
        borderWidth += this.columnWidthByName[this.Columns[index].Name];
      int num1 = Math.Max(0, borderWidth - this.GridControls.SplitterWidth / 2);
      int num2 = this.columnWidthByName[this.Columns[columnIndex].Name];
      int height = this.RowHeight - 1;
      int y;
      if (rowIndex == -1)
      {
        y = this.HeaderPanel.Height;
        if (columnIndex == 0 && num2 < 60)
        {
          ++num1;
          ++y;
          --height;
        }
      }
      else
        y = rowIndex * this.RowHeight - referencePoint.Y;
      return new Rectangle(num1 - referencePoint.X, y, num2 - 1, height);
    }

    public bool IsVisibleRow(int rowIndex)
    {
      if (rowIndex < this.FirstDisplayedScrollingRowIndex || rowIndex != this.FirstDisplayedScrollingRowIndex && rowIndex >= this.FirstDisplayedScrollingRowIndex + Math.Max(1, this.DisplayedRowCount(false)))
        return false;
      else
        return true;
    }

    public bool IsVisibleColumn(int columnIndex)
    {
      Rectangle visibleTableRectangle = this.VisibleTableRectangle;
      Rectangle rawCellRectangle = this.GetRawCellRectangle(this.ReferencePoint(), columnIndex, 0);
      if (rawCellRectangle.Left < visibleTableRectangle.Left || rawCellRectangle.Right > visibleTableRectangle.Right && rawCellRectangle.Left != visibleTableRectangle.Left)
        return false;
      else
        return true;
    }

    public bool IsVisibleCell(int columnIndex, int rowIndex)
    {
      if (this.IsVisibleRow(rowIndex))
        return this.IsVisibleColumn(columnIndex);
      else
        return false;
    }

    public void MakeVisibleCell(int columnIndex, int rowIndex)
    {
      Rectangle? cellRectangle = this.GetCellRectangle(columnIndex, rowIndex);
      if (!cellRectangle.HasValue)
        return;
      int num = this.ReferencePoint().X + cellRectangle.Value.X;
      if (!this.IsVisibleRow(rowIndex))
        this.FirstDisplayedScrollingRowIndex = rowIndex;
      if (!this.IsVisibleColumn(columnIndex))
        this.horizontalScroll.Value = Math.Max(0, Math.Min(num - 1, this.horizontalScroll.Maximum + 1 - this.horizontalScroll.LargeChange));
      this.RowPanel.Invalidate();
    }

    protected int GetDisplayedRowCount(bool includePartialRow, int height, int rowHeight)
    {
      int val1 = height / rowHeight;
      if (includePartialRow && height % rowHeight != 0)
        ++val1;
      return Math.Max(0, Math.Min(val1, this.RowsCount - this.FirstDisplayedScrollingRowIndex));
    }

    public int DisplayedRowCount(bool includePartialRow)
    {
      return this.GetDisplayedRowCount(includePartialRow, this.VisibleDataRegion.Height, this.RowHeight);
    }

    protected Rectangle GetVisibleDataRegion(bool vScrollVisible, bool hScrollVisible)
    {
      int width = this.RowPanel.Width;
      if (vScrollVisible)
        width -= this.verticalScroll.Control.Width + 1;
      int height = this.RowPanel.Height - this.HeaderAndFilterHeight;
      if (hScrollVisible)
        height -= this.horizontalScroll.Control.Height;
      return new Rectangle(0, this.HeaderAndFilterHeight, width, height);
    }

    protected abstract void Synchronize(bool isForce);

    public void ForceSynchronize()
    {
      this.Synchronize(true);
    }

    protected override void timer_Tick(object sender, EventArgs e)
    {
      this.Synchronize(false);
      if (!this.IsCellWithMessage || !(DateTime.UtcNow - this.setMessageTime > this.MessageShowDelay))
        return;
      this.ResetCellWithMessage();
    }

    public double? GetRelativeWidth(string columnName)
    {
      double num;
      if (this.relativeWidthByColumnName.TryGetValue(columnName, out num))
        return new double?(num);
      else
        return new double?();
    }

    public void SetRelativeWidth(string columnName, double relativeWidth)
    {
      this.relativeWidthByColumnName[columnName] = relativeWidth;
    }

    private int GetMinColumnWidth(IGridColumn column)
    {
      int num = (int)SynchronizerHlp.GetWidth(this.graphics, column.GetExtended("MinWidth"),
        this.drawSettings.DefaultGridFont);
      if (num == 0)
        return this.DefaultMinColumnWidth;
      else
        return num;
      //return Math.Max(DefaultMinColumnWidth, num);
    }

    private int GetMaxColumnWidth(IGridColumn column)
    {
      if (this.maxColumnWidthByName.ContainsKey(column.Name))
        return this.maxColumnWidthByName[column.Name];
      double num = SynchronizerHlp.GetWidth(this.graphics, column.GetExtended("MaxWidth"), this.drawSettings.DefaultGridFont);
      if (num == 0.0)
        num = (double)int.MaxValue;
      return (int)num;
    }

    private Dictionary<string, int> GetColumnWidthesForRelativeMode(IList<IGridColumn> columns, int dueGridWidth)
    {
      Dictionary<string, int> minWidthByColumnName = new Dictionary<string, int>();
      Dictionary<string, int> maxWidthByColumnName = new Dictionary<string, int>();
      int num1 = 0;
      double num2 = 0.0;
      foreach (IGridColumn column in (IEnumerable<IGridColumn>)columns)
      {
        if (!this.relativeWidthByColumnName.ContainsKey(column.Name))
        {
          int num3 = SynchronizerHlp.GetRelativeWidth(column) ?? 0;
          if (num3 < 1)
            num3 = 1;
          this.relativeWidthByColumnName[column.Name] = (double)num3;
        }
        int minColumnWidth = this.GetMinColumnWidth(column);
        minWidthByColumnName[column.Name] = minColumnWidth;
        num1 += minColumnWidth;
        maxWidthByColumnName[column.Name] = this.GetMaxColumnWidth(column);
        num2 += this.relativeWidthByColumnName[column.Name];
      }
      if (num1 > dueGridWidth)
        return minWidthByColumnName;
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      IList<IGridColumn> list1 = CollectionHlp.SortBy<double, IGridColumn>((IEnumerable<IGridColumn>)columns, (Getter<double, IGridColumn>)(column => this.relativeWidthByColumnName[column.Name] * (double)dueGridWidth / 100.0 / (double)minWidthByColumnName[column.Name]));
      double num4 = num2;
      int num5 = dueGridWidth;
      List<IGridColumn> list2 = new List<IGridColumn>();
      foreach (IGridColumn gridColumn in (IEnumerable<IGridColumn>)list1)
      {
        int num3 = minWidthByColumnName[gridColumn.Name];
        double num6 = this.relativeWidthByColumnName[gridColumn.Name];
        if ((int)(num6 / num4 * (double)num5) <= num3)
        {
          dictionary[gridColumn.Name] = num3;
          num4 -= num6;
          num5 -= num3;
        }
        else
          list2.Add(gridColumn);
      }
      foreach (IGridColumn gridColumn in (IEnumerable<IGridColumn>)CollectionHlp.SortBy<int, IGridColumn>((IEnumerable<IGridColumn>)list2, (Getter<int, IGridColumn>)(column => maxWidthByColumnName[column.Name] / minWidthByColumnName[column.Name])))
      {
        double num3 = this.relativeWidthByColumnName[gridColumn.Name];
        int num6 = (int)Math.Min((double)maxWidthByColumnName[gridColumn.Name], Math.Max((double)minWidthByColumnName[gridColumn.Name], num3 / num4 * (double)num5));
        dictionary[gridColumn.Name] = num6;
        num4 -= num3;
        num5 -= num6;
      }
      return dictionary;
    }

    public void ForceRecreateColumns()
    {
      columnsChangeTick++;
      ForceSynchronize();
    }


    private int GetDueGridWidth()
    {
      return this.VisibleDataRegion.Width - 2;
    }

    protected bool CheckObsoleteColumns(IList<IGridColumn> currentColumns)
    {
      if (this.Columns.Length != currentColumns.Count)
      {
        ++this.columnsChangeTick;
        return true;
      }
      else
      {
        for (int index = 0; index < this.Columns.Length; ++index)
        {
          if (this.Columns[index] != currentColumns[index])
          {
            ++this.columnsChangeTick;
            return true;
          }
        }
        return false;
      }
    }

    protected bool SyncView()
    {
      this.headersMaker.Make();
      return this.columnsWidthMaker.Make();
    }

    private void SetWidthColumns(bool vScrollVisible, bool hScrollVisible)
    {
      if (this.RelativeColumnWidthEnabled)
        this.columnWidthByName = this.GetColumnWidthesForRelativeMode((IList<IGridColumn>)this.Columns, this.GetVisibleDataRegion(vScrollVisible, hScrollVisible).Width - 2);
      foreach (IColumnHeader columnHeader in this.GridControls.ColumnHeaders)
      {
        int num = 0;
        foreach (int index in columnHeader.ColumnIndices)
        {
          IGridColumn column = this.Columns[index];
          int minColumnWidth;
          if (!this.columnWidthByName.TryGetValue(column.Name, out minColumnWidth))
          {
            minColumnWidth = this.GetMinColumnWidth(column);
            this.columnWidthByName[column.Name] = minColumnWidth;
          }
          num += minColumnWidth;
        }
        columnHeader.Width = num;
        if (columnHeader.Width != num)
          Logger.AddMessage("Не удалось выставить заданную ширину колонки: {0}, {1}", (object)num, (object)columnHeader.Width);
      }

      //foreach (string name in columnWidthByName.Keys)
      //{
      //  TraceHlp2.AddMessage("Column.Width: {0}, {1}", name, columnWidthByName[name]);
      //}
    }

    private void GridControls_ColumnWidthChanged(object sender, IColumnHeader columnHeader)
    {
      ++this.userColumnWidthChangeTick;
      if (this.RelativeColumnWidthEnabled)
      {
        int dueGridWidth = this.GetDueGridWidth();
        int headerIndex = columnHeader.ColumnIndices[0];
        int columnIndex = columnHeader.ColumnIndices[columnHeader.ColumnIndices.Length - 1];
        int leftWidth = 0;
        for (int i = 0; i < headerIndex; ++i)
          leftWidth += this.columnWidthByName[this.Columns[i].Name];

        int minRightWidth = 0;
        int maxRightWidth = 0;
        for (int i = columnIndex + 1; i < this.ColumnsCount; ++i)
        {
          IGridColumn column = this.Columns[i];
          minRightWidth += this.GetMinColumnWidth(column);
          if (maxRightWidth != int.MaxValue)
            maxRightWidth += this.GetMaxColumnWidth(column);
        }

        int headerMinWidth = 0;
        int headerMaxWidth = 0;
        for (int i = headerIndex; i <= columnIndex; ++i)
        {
          IGridColumn column = this.Columns[i];
          headerMinWidth += this.GetMinColumnWidth(column);
          if (headerMaxWidth != int.MaxValue)
            headerMaxWidth += this.GetMaxColumnWidth(column);
        }

        if (leftWidth + headerMinWidth + minRightWidth >= dueGridWidth)
        {
          columnHeader.Width = headerMinWidth;
          if (columnHeader.Width != headerMinWidth)
          {
            Logger.AddMessage("Не удалось выставить заданную ширину колонки2: {0}, {1}",
              headerMinWidth, columnHeader.Width);
          }
          this.ForceSynchronize();
          return;
        }
        else
        {
          //int val1_3 = Math.Max(headerMinWidth, dueGridWidth - leftWidth - maxRightWidth);
          //int dueGridWidth2 = Math.Min(Math.Min(headerMaxWidth, dueGridWidth - leftWidth - minRightWidth), 
          //  Math.Max(val1_3, columnHeader.Width));

          int columnHeaderWidth = Math.Max(headerMinWidth, Math.Min(headerMaxWidth, columnHeader.Width));
          if (leftWidth + columnHeaderWidth + minRightWidth > dueGridWidth)
            columnHeaderWidth = dueGridWidth - leftWidth - minRightWidth;

          Dictionary<string, int> widthesForRelativeMode1 = this.GetColumnWidthesForRelativeMode(
            ArrayHlp.GetRange(this.Columns, headerIndex, columnIndex - headerIndex + 1), columnHeaderWidth);
          Dictionary<string, int> widthesForRelativeMode2 = this.GetColumnWidthesForRelativeMode(
            ArrayHlp.GetRange(this.Columns, columnIndex + 1, this.Columns.Length - columnIndex - 1),
            dueGridWidth - leftWidth - columnHeaderWidth);

          for (int i = 0; i < headerIndex; ++i)
          {
            string name = this.Columns[i].Name;
            this.relativeWidthByColumnName[name] = this.columnWidthByName[name] * 100.0 / dueGridWidth;
          }
          foreach (string columnName in widthesForRelativeMode1.Keys)
            this.relativeWidthByColumnName[columnName] = widthesForRelativeMode1[columnName] *
              100.0 / dueGridWidth;
          foreach (string columnName in widthesForRelativeMode2.Keys)
            this.relativeWidthByColumnName[columnName] = widthesForRelativeMode2[columnName] *
              100.0 / dueGridWidth;
        }
      }
      if (!this.FinishEditing(true, (GridCell)null))
        this.FinishEditing(false, (GridCell)null);
      this.Synchronize(true);
    }
  }
}
