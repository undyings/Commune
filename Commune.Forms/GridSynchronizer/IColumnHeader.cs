using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commune.Basis;
using System.Windows.Forms;

namespace Commune.Forms
{
  public delegate void ColumnHeaderEventHandler(object sender, IColumnHeader columnHeader);

  public interface IColumnHeader
  {
    int Width { get; set; }
    int[] ColumnIndices { get; }
  }

  public class WindowsFormsColumnHeader : IColumnHeader
  {
    private readonly WindowsFormsGridHeader gridHeader;
    private readonly int headerIndex;
    private readonly int[] columnIndices;

    public int[] ColumnIndices
    {
      get
      {
        return this.columnIndices;
      }
    }

    DataGridViewColumn gridColumn
    {
      get
      {
        try
        {
          return this.gridHeader.GridView.Columns[this.headerIndex];
        }
        catch
        {
          Logger.AddMessage("GetGridColumn: {0}, {1}, {2}", (object)this.headerIndex, (object)this.gridHeader.ColumnHeaders.Length, (object)this.gridHeader.GridView.Columns.Count);
          throw;
        }
      }
    }

    public int Width
    {
      get
      {
        return this.gridColumn.Width;
      }
      set
      {
        this.gridHeader.BeginChange();
        this.gridColumn.Width = Math.Max(0, value);
        this.gridHeader.EndChange();
      }
    }

    public WindowsFormsColumnHeader(WindowsFormsGridHeader gridHeader, int headerIndex, int[] columnIndices)
    {
      this.gridHeader = gridHeader;
      this.headerIndex = headerIndex;
      this.columnIndices = columnIndices;
    }
  }
}
