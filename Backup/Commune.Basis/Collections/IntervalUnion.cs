using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Commune.Basis
{
  public struct IntervalUnion<T>
  {
    public readonly DateTime BeginTime;
    public readonly DateTime EndTime;
    public readonly T[] Sources;

    public IntervalUnion(DateTime beginTime, DateTime endTime, T[] sources)
    {
      this.BeginTime = beginTime;
      this.EndTime = endTime;
      this.Sources = sources;
    }
  }

  public struct IntervalUnion
  {
    public readonly DateTime BeginTime;
    public readonly DateTime EndTime;
    public readonly object[] Sources;

    public IntervalUnion(DateTime beginTime, DateTime endTime, object[] sources)
    {
      this.BeginTime = beginTime;
      this.EndTime = endTime;
      this.Sources = sources;
    }

    public static List<IntervalUnion> Make(IList[] collections, Getter<DateTime[], object>[] intervalGetters)
    {
      List<IntervalUnion> list = new List<IntervalUnion>();
      int[] numArray = new int[collections.Length];
      DateTime? beginTime = null;
      while (true)
      {
        DateTime? endTime = null;
        for (int index1 = 0; index1 < numArray.Length; ++index1)
        {
          int index2 = numArray[index1] / 2;
          int index3 = numArray[index1] % 2;
          if (index2 < collections[index1].Count)
          {
            DateTime[] dateTimeArray = intervalGetters[index1](collections[index1][index2]);
            if (endTime == null || dateTimeArray[index3] < endTime.Value)
              endTime = dateTimeArray[index3];
          }
        }
        if (endTime != null)
        {
          object[] sources = new object[collections.Length];
          for (int index1 = 0; index1 < numArray.Length; ++index1)
          {
            int index2 = numArray[index1] / 2;
            int index3 = numArray[index1] % 2;
            if (index2 < collections[index1].Count)
            {
              DateTime[] dateTimeArray = intervalGetters[index1](collections[index1][index2]);
              if (index3 != 0 || !(dateTimeArray[index3] > endTime.Value))
              {
                if (dateTimeArray[index3] == endTime.Value)
                  ++numArray[index1];
                if (index3 != 0)
                  sources[index1] = collections[index1][index2];
              }
            }
          }
          if (beginTime != null && beginTime.Value != endTime.Value)
            list.Add(new IntervalUnion(beginTime.Value, endTime.Value, sources));
          beginTime = endTime.Value;
        }
        else
          break;
      }
      return list;
    }
  }
}