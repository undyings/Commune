using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Commune.Basis
{
  public interface IReadOnlySet<T> : IEnumerable<T>, IEnumerable
  {
    int IndexOf(T item);
    T this[int index] { get; }
    int Count { get; }
    int BinarySearch(T item, Getter<int, T, T> comparer);
    T[] ToArray();
  }

  public class ListReadOnlySet<T> : ListReadOnlySet<T, T>
  {
    public ListReadOnlySet(List<T> items)
      : base(items)
    {
    }
  }

  public class ListReadOnlySet<TItem, TList> : IReadOnlySet<TItem> where TList : TItem
  {
    readonly List<TList> items;
    public ListReadOnlySet(List<TList> items)
    {
      this.items = items;
    }

    public int IndexOf(TItem item)
    {
      if (item is TList)
        return items.IndexOf((TList)item);
      return -1;
    }

    public TItem this[int index]
    {
      get
      {
        return items[index];
      }
    }

    public int Count
    {
      get
      {
        return items.Count;
      }
    }

    public int BinarySearch(TItem item, Getter<int, TItem, TItem> comparer)
    {
      return _.BinarySearch(items, item, comparer);
    }

    public TItem[] ToArray()
    {
      TItem[] copy = new TItem[items.Count];
      for (int i = 0; i < items.Count; ++i)
        copy[i] = items[i];
      return copy;
    }

    public IEnumerator GetEnumerator()
    {
      return items.GetEnumerator();
    }

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
      return new MutableEnumerator<TItem, TList>(((IEnumerable<TList>)items).GetEnumerator(),
        delegate(TList sourceItem) { return sourceItem; });
    }
  }

  public class ArrayReadOnlySet<T> : ArrayReadOnlySet<T, T>
  {
    public ArrayReadOnlySet(T[] array) :
      base(array)
    {
    }
  }

  public class ArrayReadOnlySet<TItem, TArray> : IReadOnlySet<TItem> where TArray : TItem
  {
    readonly TArray[] items;
    public ArrayReadOnlySet(TArray[] items)
    {
      this.items = items;
    }

    public int IndexOf(TItem item)
    {
      if (item is TArray)
        return Array.IndexOf(items, (TArray)item);
      return -1;
    }

    public TItem this[int index]
    {
      get
      {
        return items[index];
      }
    }

    public int Count
    {
      get 
      {
        return items.Length;
      }
    }

    public int BinarySearch(TItem item, Getter<int, TItem, TItem> comparer)
    {
      return _.BinarySearch(items, item, comparer);
    }

    public TItem[] ToArray()
    {
      TItem[] copy = new TItem[items.Length];
      for (int i = 0; i < items.Length; ++i)
        copy[i] = items[i];
      return copy;
    }

    public IEnumerator GetEnumerator()
    {
      return items.GetEnumerator();
    }

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
      return new MutableEnumerator<TItem, TArray>(((IEnumerable<TArray>)items).GetEnumerator(),
        delegate(TArray sourceItem) { return sourceItem; } );
    }
  }
}
