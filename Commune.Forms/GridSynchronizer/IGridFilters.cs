using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Windows.Forms;

namespace Commune.Forms
{
  public interface IGridFilters<TRow>
  {
    VirtualGridSynchronizer GridSynch { get; }
    GridFilterStorage Storage { get; }
    IList<TRow> SortedFilteredRows { get; }
  }

  public class GridFilters<TRow> : GridFilters<TRow, TRow>
  {
    public GridFilters(VirtualGridSynchronizer gridSynch,
      Getter<IEnumerable<TRow>> allRowsGetter, Getter<object[], TRow> rowSorter,
      Getter<long> rowListChangeTickGetter, Getter<long> rowsChangeTickGetter,
      Getter<long> rowsSourceChangeTickGetter)
      : base(gridSynch, allRowsGetter, delegate (TRow row) { return row; }, rowSorter,
      rowListChangeTickGetter, rowsChangeTickGetter, rowsSourceChangeTickGetter)
    {
    }
  }

  public class GridFilters<TRow, TRowId> : IGridFilters<TRow>
  {
    private readonly GridFilterStorage filterStorage = new GridFilterStorage();
    private readonly VirtualGridSynchronizer gridSynch;
    private readonly Getter<IEnumerable<TRow>> allRowsGetter;
    private readonly Getter<TRowId, TRow> rowIdGetter;
    private readonly Getter<object[], TRow> rowSorter;
    private readonly Getter<long> rowListChangeTickGetter;
    private readonly Getter<long> rowsChangeTickGetter;
    private readonly Getter<long> rowsSourceChangeTickGetter;
    private readonly RawCache<Tuple<Dictionary<TRowId, bool>, long, long>> hideRowByIdCache;
    private readonly RawCache<IList<TRow>> sortedFilteredRowsCache;

    public VirtualGridSynchronizer GridSynch
    {
      get
      {
        return this.gridSynch;
      }
    }

    long rowListChangeTick
    {
      get
      {
        if (this.rowListChangeTickGetter == null)
          return 0;
        return this.rowListChangeTickGetter();
      }
    }

    long rowsChangeTick
    {
      get
      {
        if (this.rowsChangeTickGetter == null)
          return 0;
        return this.rowsChangeTickGetter();
      }
    }

    long rowsSourceChangeTick
    {
      get
      {
        if (this.rowsSourceChangeTickGetter == null)
          return 0;
        return this.rowsSourceChangeTickGetter();
      }
    }

    public GridFilterStorage Storage
    {
      get
      {
        return this.filterStorage;
      }
    }

    Dictionary<TRowId, bool> hideRowById
    {
      get
      {
        return this.hideRowByIdCache.Result.Item1;
      }
    }

    long cacheRowsChangeTick
    {
      get
      {
        return this.hideRowByIdCache.Result.Item2;
      }
    }

    long cacheGlobalChangeTick
    {
      get
      {
        return this.hideRowByIdCache.Result.Item3;
      }
    }

    public IList<TRow> SortedFilteredRows
    {
      get
      {
        return this.sortedFilteredRowsCache.Result;
      }
    }

    public long SortedFilteredRowsChangeTick
    {
      get
      {
        return this.sortedFilteredRowsCache.ChangeTick;
      }
    }

    public bool IsObsoleteFiltration
    {
      get
      {
        if (this.cacheGlobalChangeTick == this.rowsSourceChangeTick)
          return this.cacheRowsChangeTick != this.rowsChangeTick;
        else
          return true;
      }
    }

    public GridFilters(VirtualGridSynchronizer gridSynch, Getter<IEnumerable<TRow>> allRowsGetter,
      Getter<TRowId, TRow> rowIdGetter, Getter<object[], TRow> rowSorter,
      Getter<long> rowListChangeTickGetter, Getter<long> rowsChangeTickGetter,
      Getter<long> rowsSourceChangeTickGetter)
    {
      GridFilters<TRow, TRowId> gridFilters = this;
      this.gridSynch = gridSynch;
      this.allRowsGetter = allRowsGetter;
      this.rowIdGetter = rowIdGetter;
      this.rowSorter = rowSorter;
      this.rowListChangeTickGetter = rowListChangeTickGetter;
      this.rowsChangeTickGetter = rowsChangeTickGetter;
      this.rowsSourceChangeTickGetter = rowsSourceChangeTickGetter;
      gridSynch.FilterRowGetter = delegate
      {
        if (this.filterStorage.Empty)
          return (object)null;
        else
          return new object();
      };

      this.hideRowByIdCache = new Cache<Tuple<Dictionary<TRowId, bool>, long, long>, long, long>(
        delegate
        {
          Dictionary<TRowId, bool> hideRowById = new Dictionary<TRowId, bool>();
          if (!gridFilters.filterStorage.Empty)
          {
            foreach (TRow row in allRowsGetter())
            {
              foreach (IGridColumn gridColumn in gridSynch.Columns)
              {
                bool? isShowRow = gridFilters.filterStorage.IsShowRow(gridColumn, row);
                if (isShowRow == false)
                {
                  hideRowById[rowIdGetter(row)] = true;
                }
                else if (isShowRow == true)
                {
                  hideRowById.Remove(rowIdGetter(row));
                  break;
                }
              }
            }
          }
          return CollectionHlp.Tuple(hideRowById, gridFilters.rowsChangeTick, gridFilters.rowsSourceChangeTick);
        },
        delegate { return this.filterStorage.FilterChangeTick; },
        delegate { return this.rowsSourceChangeTick; }
      );

      this.sortedFilteredRowsCache = new Cache<IList<TRow>, long, long, long>(
        delegate
        {
          List<TRow> list = new List<TRow>();
          foreach (TRow row in allRowsGetter())
          {
            if (!gridFilters.hideRowById.ContainsKey(rowIdGetter(row)))
              list.Add(row);
          }
          if (rowSorter == null)
            return list;

          return CollectionHlp.SortBy(list, rowSorter, CollectionHlp.ArrayComparison);
        },
        delegate { return this.rowsSourceChangeTick; },
        delegate { return this.rowListChangeTick; },
        delegate { return this.hideRowByIdCache.ChangeTick; }
      );
    }
  }

  public class GridFilterStorage
  {
    private readonly Dictionary<IGridColumn, object> filterValueByFilterColumn =
      new Dictionary<IGridColumn, object>();

    private long filterChangeTick;

    public long FilterChangeTick
    {
      get
      {
        return this.filterChangeTick;
      }
    }

    public bool Empty
    {
      get
      {
        return this.filterValueByFilterColumn.Count == 0;
      }
    }

    public bool? IsShowRow(IGridColumn gridColumn, object row)
    {
      IGridColumn filter = SynchronizerHlp.GetFilter(gridColumn);
      if (filter == null)
        return null;
      object valueOrDefault = DictionaryHlp.GetValueOrDefault<IGridColumn, object>((IDictionary<IGridColumn, object>)this.filterValueByFilterColumn, filter);
      if (valueOrDefault == null)
        return null;
      if (SynchronizerHlp.IsHideRow(gridColumn, filter, row, valueOrDefault))
        return false;
      if (SynchronizerHlp.IsOrCondition(filter, valueOrDefault))
        return true;
      return null;
    }

    public void Update()
    {
      ++this.filterChangeTick;
    }

    public object GetFilterValue(IGridColumn filterColumn)
    {
      return DictionaryHlp.GetValueOrDefault<IGridColumn, object>((IDictionary<IGridColumn, object>)this.filterValueByFilterColumn, filterColumn);
    }

    public void AssignFilterValue(IGridColumn filterColumn, object filterValue)
    {
      if (filterValue == null || filterValue as string == "")
        this.filterValueByFilterColumn.Remove(filterColumn);
      else
        this.filterValueByFilterColumn[filterColumn] = filterValue;
      ++this.filterChangeTick;
    }

    public void ResetFilters()
    {
      this.filterValueByFilterColumn.Clear();
      ++this.filterChangeTick;
    }
  }
}
