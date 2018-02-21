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
  public interface IVirtualGridControls
  {
    VirtualGridDrawSettings DrawSettings { get; }
    ISimpleScroll HScroll { get; }
    ISimpleScroll VScroll { get; }

    Control HeaderPanel { get; }
    IColumnHeader[] ColumnHeaders { get; }

    int SplitterWidth { get; }
    int BorderWidth { get; }

    event ColumnHeaderEventHandler ColumnWidthChanged;

    void RecreateGridHeader(IList<IGridColumn> columns);
    void BeginChange();
    void EndChange();
  }

  public class WindowsFormsGridHeader : IVirtualGridControls
  {
    private readonly Panel headerPanel;
    public readonly DataGridView GridView;
    private readonly ISimpleScroll hScroll;
    private readonly ISimpleScroll vScroll;
    private IColumnHeader[] columnHeaders;
    private readonly int borderWidth;
    private readonly VirtualGridDrawSettings drawSettings;
    private bool isChanging;

    public VirtualGridDrawSettings DrawSettings
    {
      get
      {
        return this.drawSettings;
      }
    }

    public IColumnHeader[] ColumnHeaders
    {
      get
      {
        return this.columnHeaders;
      }
    }

    public ISimpleScroll HScroll
    {
      get
      {
        return this.hScroll;
      }
    }

    public Control HeaderPanel
    {
      get
      {
        return (Control)this.headerPanel;
      }
    }

    public int BorderWidth
    {
      get
      {
        return this.borderWidth;
      }
    }

    public int SplitterWidth
    {
      get
      {
        return 1;
      }
    }

    public ISimpleScroll VScroll
    {
      get
      {
        return this.vScroll;
      }
    }

    public event ColumnHeaderEventHandler ColumnWidthChanged;

    public WindowsFormsGridHeader(int borderWidth)
      : this(borderWidth, false)
    {
    }

    public WindowsFormsGridHeader(int borderWidth, bool isPropertyGrid)
    {
      this.borderWidth = borderWidth;
      this.drawSettings = !isPropertyGrid ? new VirtualGridDrawSettings() : (VirtualGridDrawSettings)new VirtualPropertyGridDrawSettings();
      this.headerPanel = new Panel();
      this.hScroll = (ISimpleScroll)new WindowsFormsScroll((ScrollBar)new HScrollBar());
      this.vScroll = (ISimpleScroll)new WindowsFormsScroll((ScrollBar)new VScrollBar());
      this.GridView = new DataGridView();
      this.GridView.BackgroundColor = this.headerPanel.BackColor;
      this.GridView.BorderStyle = BorderStyle.None;
      this.GridView.Font = this.drawSettings.DefaultGridFont;
      this.GridView.RowHeadersVisible = false;
      this.GridView.AllowUserToAddRows = false;
      this.headerPanel.Controls.Add((Control)this.GridView);
      this.GridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(this.GridView_ColumnWidthChanged);
    }

    private void GridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
      if (this.isChanging || this.ColumnWidthChanged == null)
        return;
      this.ColumnWidthChanged(sender, (IColumnHeader)e.Column.Tag);
    }

    public void BeginChange()
    {
      this.isChanging = true;
    }

    public void EndChange()
    {
      if (this.GridView.Width != this.headerPanel.Width)
        this.GridView.Width = this.headerPanel.Width;
      if (this.GridView.Height != this.headerPanel.Height)
        this.GridView.Height = this.headerPanel.Height;
      this.isChanging = false;
    }

    public void RecreateGridHeader(IList<IGridColumn> columns)
    {
      List<DataGridViewColumn> list1 = new List<DataGridViewColumn>();
      foreach (DataGridViewColumn dataGridViewColumn in (BaseCollection)this.GridView.Columns)
        list1.Add(dataGridViewColumn);
      this.GridView.Columns.Clear();
      this.columnHeaders = new IColumnHeader[0];
      foreach (DataGridViewBand dataGridViewBand in list1)
        dataGridViewBand.Dispose();
      List<IColumnHeader> list2 = new List<IColumnHeader>();
      this.BeginChange();
      int headerIndex = -1;
      for (int index1 = 0; index1 < columns.Count; ++index1)
      {
        IGridColumn column = columns[index1];
        if (!SynchronizerHlp.IsFakeHeader(column))
        {
          ++headerIndex;
          List<int> list3 = new List<int>();
          list3.Add(index1);
          if (SynchronizerHlp.IsCompositeHeader(column))
          {
            for (int index2 = index1 + 1; index2 < columns.Count && SynchronizerHlp.IsFakeHeader(columns[index2]); ++index2)
              list3.Add(index2);
          }
          WindowsFormsColumnHeader formsColumnHeader = new WindowsFormsColumnHeader(this, headerIndex, list3.ToArray());
          list2.Add((IColumnHeader)formsColumnHeader);
          DataGridViewColumn dataGridViewColumn = new DataGridViewColumn();
          dataGridViewColumn.Name = SynchronizerHlp.GetDisplayName(column);
          dataGridViewColumn.Tag = (object)formsColumnHeader;
          this.GridView.Columns.Add(dataGridViewColumn);
          dataGridViewColumn.Visible = true;
        }
      }
      this.EndChange();
      this.columnHeaders = list2.ToArray();
    }
  }
}