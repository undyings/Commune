using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Commune.Basis
{
  public class WrapperCollection<TWrapper, TSource> : IEnumerable<TWrapper>, IEnumerable
  {
    readonly Getter<IEnumerable<TSource>> sourcesGetter;
    readonly Getter<bool, TSource> existChecker;
    readonly Getter<TWrapper, TSource> wrapperCreator;
    readonly TimeSpan removeWrapperCollectorSpan;
    readonly double limitRatioRemoveWrappers;

    readonly Dictionary<TSource, TWrapper> wrapperBySource = new Dictionary<TSource, TWrapper>();

    public WrapperCollection(Getter<IEnumerable<TSource>> sourcesGetter,
      Getter<bool, TSource> existChecker, Getter<TWrapper, TSource> wrapperCreator,
      TimeSpan removeWrapperCollectorSpan, double limitRatioRemoveWrappers)
    {
      this.sourcesGetter = sourcesGetter;
      this.existChecker = existChecker;
      this.wrapperCreator = wrapperCreator;
      this.removeWrapperCollectorSpan = removeWrapperCollectorSpan;
      this.limitRatioRemoveWrappers = limitRatioRemoveWrappers;
    }

    public WrapperCollection(Getter<IEnumerable<TSource>> sourcesGetter,
      Getter<bool, TSource> existChecker, Getter<TWrapper, TSource> wrapperCreator) :
      this (sourcesGetter, existChecker, wrapperCreator, TimeSpan.FromMinutes(1), 0.25)
    {
    }

    public bool ExistSource(TSource source)
    {
      return existChecker(source);
    }

    public IEnumerable<TWrapper> GetEnumerable()
    {
      int count = 0;
      IEnumerable<TSource> sources = sourcesGetter();
      foreach (TSource source in sources)
      {
        if (!ExistSource(source))
          continue;

        TWrapper wrapper;
        if (!wrapperBySource.TryGetValue(source, out wrapper))
        {
          wrapper = wrapperCreator(source);
          wrapperBySource[source] = wrapper;
        }
        yield return wrapper;
        count++;
      }

      float optimalCount = count != 0 ? count : 1;
      if (wrapperBySource.Count != count && 
        (DateTime.UtcNow - lastCollectTime > removeWrapperCollectorSpan ||
        (wrapperBySource.Count - count) / optimalCount > limitRatioRemoveWrappers))
      {
        PurgeCollection();
      }
    }

    DateTime lastCollectTime = DateTime.UtcNow;
    void PurgeCollection()
    {
      foreach (TSource source in _.ToArray(wrapperBySource.Keys))
      {
        if (!existChecker(source))
        {
          TWrapper wrapper;
          if (wrapperBySource.TryGetValue(source, out wrapper))
          {
            if (wrapper is IDisposable)
            {
              ((IDisposable)wrapper).Dispose();
            }
            wrapperBySource.Remove(source);
          }

          //TWrapper wrapper = wrapperBySource[source];
          //if (wrapper is IDisposable)
          //{
          //  ((IDisposable)wrapper).Dispose();
          //}
          //wrapperBySource.Remove(source);
        }
      }
      lastCollectTime = DateTime.UtcNow;
    }

    IEnumerator<TWrapper> IEnumerable<TWrapper>.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
      return ((IEnumerable<TWrapper>)this).GetEnumerator();
    }

    public TWrapper GetOrCreateWrapper(TSource source)
    {
      if (DateTime.UtcNow - lastCollectTime > removeWrapperCollectorSpan)
        PurgeCollection();

      if (!existChecker(source))
        return default(TWrapper);

      if (!wrapperBySource.ContainsKey(source))
        wrapperBySource[source] = wrapperCreator(source);
      return wrapperBySource[source];
    }

    //IEnumerator<TWrapper> IEnumerable<TWrapper>.GetEnumerator()
    //{
    //  int count = 0;
    //  IEnumerable<TSource> sources = sourcesGetter();
    //  foreach (TSource source in sources)
    //  {
    //    if (!ExistSource(source))
    //      continue;

    //    if (!wrapperBySource.ContainsKey(source))
    //      wrapperBySource[source] = wrapperCreator(source);
    //    count++;
    //  }

    //  double optimalCount = count;
    //  if (optimalCount == 0)
    //    optimalCount = 1;
    //  if ((wrapperBySource.Count - count != 0 && DateTime.UtcNow - lastCollectTime > removeWrapperCollectorSpan) ||
    //    (wrapperBySource.Count - count) / optimalCount > limitRatioRemoveWrappers)
    //  {
    //    foreach (TSource source in _.ToArray(wrapperBySource.Keys))
    //    {
    //      if (!existChecker(source))
    //      {
    //        TWrapper wrapper = wrapperBySource[source];
    //        if (wrapper is IDisposable)
    //        {
    //          ((IDisposable)wrapper).Dispose();
    //        }
    //        wrapperBySource.Remove(source);
    //      }
    //    }
    //    lastCollectTime = DateTime.UtcNow;
    //  }

    //  return new DictionaryEnumerator<TWrapper, TSource>(sources.GetEnumerator(), wrapperBySource);
    //}

    //public IEnumerator GetEnumerator()
    //{
    //  return ((IEnumerable<TWrapper>)this).GetEnumerator();
    //}
  }

  //public class DictionaryEnumerator<TItem, TSource> : IEnumerator<TItem>, IEnumerator
  //{
  //  readonly IEnumerator<TSource> sourceEnumerator;
  //  readonly Dictionary<TSource, TItem> itemBySource = new Dictionary<TSource, TItem>();
  //  public DictionaryEnumerator(IEnumerator<TSource> sourceEnumerator, Dictionary<TSource, TItem> itemBySource)
  //  {
  //    this.sourceEnumerator = sourceEnumerator;
  //    this.itemBySource = itemBySource;
  //  }

  //  public TItem Current
  //  {
  //    get { return itemBySource[sourceEnumerator.Current]; }
  //  }

  //  public void Dispose()
  //  {
  //    sourceEnumerator.Dispose();
  //  }

  //  object IEnumerator.Current
  //  {
  //    get { return itemBySource[sourceEnumerator.Current]; }
  //  }

  //  public bool MoveNext()
  //  {
  //    return sourceEnumerator.MoveNext();
  //  }

  //  public void Reset()
  //  {
  //    sourceEnumerator.Reset();
  //  }
  //}

  //public class MutableEnumerable<TItem, TSource> : IEnumerable<TItem>, IEnumerable
  //{
  //  readonly Getter<IEnumerable<TSource>> sourceEnumerableGetter;
  //  readonly Getter<TItem, TSource> toItemConverter;
  //  public MutableEnumerable(Getter<IEnumerable<TSource>> sourceEnumerableGetter, 
  //    Getter<TItem, TSource> toItemConverter)
  //  {
  //    this.sourceEnumerableGetter = sourceEnumerableGetter;
  //    this.toItemConverter = toItemConverter;
  //  }

  //  public IEnumerator<TItem> GetEnumerator()
  //  {
  //    return new MutableEnumerator<TItem, TSource>(sourceEnumerableGetter().GetEnumerator(), toItemConverter);
  //  }

  //  IEnumerator IEnumerable.GetEnumerator()
  //  {
  //    return GetEnumerator();
  //  }
  //}

  public class MutableEnumerator<TItem, TSource> : IEnumerator<TItem>, IEnumerator
  {
    readonly IEnumerator<TSource> sourceEnumerator;
    readonly Getter<TItem, TSource> toItemConverter;
    public MutableEnumerator(IEnumerator<TSource> sourceEnumerator, Getter<TItem, TSource> toItemConverter)
    {
      this.sourceEnumerator = sourceEnumerator;
      this.toItemConverter = toItemConverter;
    }

    public TItem Current
    {
      get { return toItemConverter(sourceEnumerator.Current); }
    }

    public void Dispose()
    {
      sourceEnumerator.Dispose();
    }

    object IEnumerator.Current
    {
      get { return Current; }
    }

    public bool MoveNext()
    {
      return sourceEnumerator.MoveNext();
    }

    public void Reset()
    {
      sourceEnumerator.Reset();
    }
  }

  //public class ComparerWrapper<TWrapper, TSource> : IComparer<TWrapper> where TWrapper : TSource
  //{
  //  readonly Getter<int, TSource, TSource> comparer;
  //  public ComparerWrapper(Getter<int, TSource, TSource> comparer)
  //  {
  //    this.comparer = comparer;
  //  }

  //  public int Compare(TWrapper x, TWrapper y)
  //  {
  //    return comparer(x, y);
  //  }
  //}
}
