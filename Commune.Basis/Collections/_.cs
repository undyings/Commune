using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Commune.Basis
{
  public class _ : CollectionHlp
  {
    public static long? Sum(IEnumerable<long> items)
    {       
      long? sum = null;
      foreach (long item in items)
        if (sum == null)
          sum = item;
        else
          sum = sum.Value + item;
      return sum;
    }

    public static int? Sum(IEnumerable<int> items)
    {
      int? sum = null;
      foreach (int item in items)
        if (sum == null)
          sum = item;
        else
          sum = sum.Value + item;
      return sum;
    }

    public static double? Sum(IEnumerable<double> items)
    {
      double? sum = null;
      foreach (double item in items)
        if (sum == null)
          sum = item;
        else
          sum = sum.Value + item;
      return sum;
    }

    public static float? Sum(IEnumerable<float> items)
    {
      float? sum = null;
      foreach (float item in items)
        if (sum == null)
          sum = item;
        else
          sum = sum.Value + item;
      return sum;
    }

    public static IEnumerable<TDest> Convert<TDest, TSource>(IEnumerable sourceCollection,
      Getter<TDest, TSource> converter)
    {
      foreach (TSource source in sourceCollection)
        yield return converter(source);  
    }

    public static T? Min<T>(IEnumerable<T> items) where T : struct, IComparable<T>
    {
      return CollectionHlp.MinInStructs<T>(items);
    }
 
    public static T? Max<T>(IEnumerable<T> items) where T : struct, IComparable<T>
    {
      return CollectionHlp.MaxInStructs<T>(items);
    }


    public static List<IntervalUnion<T>> UniteOverlapIntervals<T>(IList<T> intervals,
      Getter<DateTime[], T> intervalGetter)
    {
      List<IntervalUnion<T>> unions = new List<IntervalUnion<T>>();
      if (intervals.Count == 0)
        return unions;

      List<T> sources = new List<T>();
      sources.Add(intervals[0]);
      DateTime[] unionInterval = intervalGetter(intervals[0]);
      for (int i = 1; i < intervals.Count; ++i)
      {
        DateTime[] interval = intervalGetter(intervals[i]);
        if (interval[0] > unionInterval[1])
        {
          unions.Add(new IntervalUnion<T>(unionInterval[0], unionInterval[1], sources.ToArray()));
          sources.Clear();
          sources.Add(intervals[i]);
          unionInterval = interval;
          continue;
        }

        sources.Add(intervals[i]);
        if (interval[1] > unionInterval[1])
          unionInterval[1] = interval[1];
      }

      unions.Add(new IntervalUnion<T>(unionInterval[0], unionInterval[1], sources.ToArray()));
      return unions;
    }

    public static bool Remove<TItem, TKey>(IList<TItem> items, TKey key,
      Getter<TKey, TItem> keyGetter)
    {
      TItem item = _.Find(items, key, keyGetter);
      if (item == null)
        return false;

      return items.Remove(item);
    }

    public static bool RemoveItems<T>(List<T> items, Getter<bool, T> isRemoveGetter)
    {
      bool isRemoved = false;
      for (int i = items.Count - 1; i >= 0; --i)
      {
        if (isRemoveGetter(items[i]))
        {
          items.RemoveAt(i);
          isRemoved = true;
        }
      }
      return isRemoved;
    }

  }
}
