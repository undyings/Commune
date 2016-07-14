#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

#endregion

namespace Commune.Basis
{

  /// <summary>
  /// Алгоритмы/Методы для работы с типизированными коллекциями
  /// </summary>
  public class CollectionHlp
  {
    public static IEnumerable<T> UnionWhile<T>(IList<T> items, 
      Getter<bool, T, T> unionCondition, Getter<T, T[]> unionToItem)
    {
      for (int i = 0; i < items.Count; ++i)
      {
        int j = i + 1;
        for (; j < items.Count; ++j)
        {
          if (!unionCondition(items[j - 1], items[j]))
            break;
        }

        if (i == j - 1)
        {
          yield return items[i];
          continue;
        }

        yield return unionToItem(_.GetRange(items, i, j - i));

        i = j - 1;
      }
    }

    public static T? FirstValue<T>(IEnumerable<T> collection) where T : struct
    {
      if (collection != null)
        foreach (T item in collection)
          return item;
      return null;
    }

    public static T First<T>(IEnumerable<T> collection)
    {
      if (collection != null)
        foreach (T item in collection)
          return item;
      return default(T);
    }

    public static T? LastValue<T>(IEnumerable<T> collection) where T : struct
    {
      T? lastItem = null;
      if (collection != null)
        foreach (T item in collection)
          lastItem = item;
      return lastItem;
    }

    public static T Last<T>(IEnumerable<T> collection)
    {
      T lastItem = default(T);
      if (collection != null)
        foreach (T item in collection)
          lastItem = item;
      return lastItem;
    }

    public static T Last<T>(IList<T> list)
    {
      if (list.Count != 0)
        return list[list.Count - 1];
      return default(T);
    }

    public static T? LastValue<T>(IList<T> list) where T : struct
    {
      if (list.Count != 0)
        return list[list.Count - 1];
      return null;
    }

    /// <summary>
    /// Возвращает true если длина коллекции больше заданной, иначе - false.
    /// </summary>
    public static bool CountMore(IEnumerable collection, int specifiedCount)
    {
      int count = 0;
      if (collection != null)
        foreach (object item in collection)
        {
          count++;
          if (count > specifiedCount)
            return true;
        }
      return false;
    }

    public static bool Empty(IEnumerable collection)
    {
      return !CountMore(collection, 0);
    }

    public static IReadOnlySet<T> AsReadOnly<T>(List<T> list)
    {
      return new ListReadOnlySet<T>(list);
    }

    public static IReadOnlySet<T> AsReadOnly<T>(T[] array)
    {
      return new ArrayReadOnlySet<T>(array);
    }

    public static int BinarySearch<TKey, TItem>(IList<TItem> items, TKey key, Getter<int, TKey, TKey> comparer) 
      where TItem : TKey
    {
      return BinarySearch(items, key, delegate(TItem listItem) { return listItem; }, comparer);
    }

    public static bool Contains<T>(IEnumerable<T> items, T findItem)
    {
      foreach (T item in items)
      {
        if (object.Equals(item, findItem))
          return true;
      }
      return false;
    }

    public static bool Contains<TKey, TItem>(IEnumerable<TItem> items, TKey findKey,
      Getter<TKey, TItem> keyGetter)
    {
      foreach (TItem item in items)
      {
        if (object.Equals(keyGetter(item), findKey))
          return true;
      }
      return false;
    }

    public static void GetDiapason<TItem, TKey>(List<TItem> sortedList,
      Getter<TKey, TItem> keyGetter, TKey? min, TKey? max, out int minIndex, out int count) where TKey : struct
    {
      minIndex = 0;
      if (min != null)
      {
        minIndex = _.BinarySearch(sortedList, min.Value, keyGetter);
        if (minIndex < 0)
          minIndex = ~minIndex;
        else
        {
          for (int i = minIndex - 1; i >= 0; --i)
          {
            if (!ObjectHlp.IsEquals(keyGetter(sortedList[i]), min.Value))
              break;
            minIndex = i;
          }
        }
      }

      int maxIndex = sortedList.Count;
      if (max != null)
      {
        maxIndex = _.BinarySearch(sortedList, max.Value, keyGetter);
        if (maxIndex < 0)
          maxIndex = ~maxIndex;
        else
        {
          for (int i = maxIndex + 1; i < sortedList.Count; ++i)
          {
            if (!ObjectHlp.IsEquals(keyGetter(sortedList[i]), max.Value))
              break;
            maxIndex = i;
          }
          maxIndex++;
        }
      }

      count = maxIndex - minIndex;
    }

    public static int BinarySearch<TKey, TItem>(TItem searchItem, IList<TItem> collection,
      Getter<TKey, TItem> keyGetter, Getter<int, TKey, TKey> comparer)
    {
      return BinarySearch(collection, keyGetter(searchItem), keyGetter, comparer);
    }

    public static int BinarySearch<TKey, TItem>(IList<TItem> collection, TKey searchKey,
      Getter<TKey, TItem> keyGetter)
    {
      return BinarySearch(collection, searchKey, keyGetter, Comparer<TKey>.Default.Compare);
    }

    public static int BinarySearch<TKey, TItem>(IList<TItem> collection, TKey searchKey,
      Getter<TKey, TItem> keyGetter, Getter<int, TKey, TKey> comparer)
    {
      if (comparer == null)
        comparer = Comparer<TKey>.Default.Compare;

      int beginIndex = 0;
      int endIndex = collection.Count - 1;
      while (beginIndex <= endIndex)
      {
        int bisectionIndex = beginIndex + ((endIndex - beginIndex) >> 1);
        int result = comparer(keyGetter(collection[bisectionIndex]), searchKey);
        if (result == 0)
        {
          return bisectionIndex;
        }
        if (result < 0)
        {
          beginIndex = bisectionIndex + 1;
        }
        else
        {
          endIndex = bisectionIndex - 1;
        }
      }
      return ~beginIndex;
    }

    public static bool InsertInSortedList<TItem, TKey>(IList<TItem> collection, TItem insertItem,
      Getter<TKey, TItem> keyGetter)
    {
      return InsertInSortedList(collection, insertItem, keyGetter, null, false);
    }

    public static bool InsertInSortedList<TItem, TKey>(IList<TItem> collection, TItem insertItem,
      Getter<TKey, TItem> keyGetter, Getter<int, TKey, TKey> comparer, bool disableRepeats)
    {
      int position = BinarySearch(insertItem, collection, keyGetter, comparer);
      if (position < 0)
      {
        if (disableRepeats)
          return false;
        position = ~position;
      }
      collection.Insert(position, insertItem);
      return true;
    }

    public static Tuple<T1, T2> Tuple<T1, T2>(T1 first, T2 second)
    {
      return new Tuple<T1, T2>(first, second);
    }

    public static Tuple<T1, T2, T3> Tuple<T1, T2, T3>(T1 first, T2 second, T3 third)
    {
      return new Tuple<T1, T2, T3>(first, second, third);
    }

    public static Tuple<T1, T2, T3, T4> Tuple<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
    {
      return new Tuple<T1, T2, T3, T4>(first, second, third, fourth);
    }

    public static Tuple<T1, T2, T3, T4, T5> Tuple<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
    {
      return new Tuple<T1, T2, T3, T4, T5>(first, second, third, fourth, fifth);
    }

    public static Tuple<T1, T2, T3, T4, T5, T6> Tuple<T1, T2, T3, T4, T5, T6>(T1 first, T2 second, T3 third,
      T4 fourth, T5 fifth, T6 sixth)
    {
      return new Tuple<T1, T2, T3, T4, T5, T6>(first, second, third, fourth, fifth, sixth);
    }

    /// <summary>
    /// Сравнивает два значения. Если ни одно из значений не поддерживает IComparable возвращает 0.
    /// </summary>
    public static int ValueComparison(object value1, object value2)
    {
      IComparable comparable1 = value1 as IComparable;
      if (comparable1 != null)
      {
        return comparable1.CompareTo(value2);
      }
      else
      {
        IComparable comparable2 = value2 as IComparable;
        if (comparable2 != null)
        {
          return -comparable2.CompareTo(value1);
        }
      }
      return 0;
    }

    /// <summary>
    /// Поэлементно сравнивает содержимое двух массивов одинаковой длины
    /// </summary>
    public static int ArrayComparison<T>(T[] values1, T[] values2)
    {
      int result = values1.Length.CompareTo(values2.Length);
      if (result != 0)
        return result;

      for (int i = 0; i < values1.Length; ++i)
      {
        int cmp = ValueComparison(values1[i], values2[i]);
        if (cmp != 0)
          return cmp;
      }
      return 0;
    }

    /// <summary>
    /// Вычисляет HashCode. Если значение null, то возвращает 0.
    /// </summary>
    public static int GetHashCode<T>(T p)
    {
      return p != null ? p.GetHashCode() : 0;
    }

    /// <summary>
    /// Раскладывыет содержимое коллекции items по "ящикам". Два элемента поподают в один "ящик"(List), 
    /// если у них совпадают ключи (т. е. значения extractor(item)) 
    /// </summary>
    public static Dictionary<TKey, List<TItem>> MakeIndex<TKey, TItem>(IEnumerable items, Getter<TKey, TItem> keyGetter)
    {
      Dictionary<TKey, List<TItem>> dictionary = new Dictionary<TKey, List<TItem>>();
      foreach (TItem item in items)
      {
        TKey key = keyGetter(item);
        if (dictionary.ContainsKey(key))
          dictionary[key].Add(item);
        else
        {
          List<TItem> list = new List<TItem>();
          list.Add(item);
          dictionary[key] = list;
        }
      }
      return dictionary;
    }
    /// <summary>
    /// оставляем для каждого ключа только первое вхождение
    /// </summary>
    public static Dictionary<TKey, TItem> MakeUniqueIndex<TKey, TItem>(IEnumerable items, Getter<TKey, TItem> keyGetter)
    {
      Dictionary<TKey, TItem> index;
      if (items is ICollection)
        index = new Dictionary<TKey, TItem>(((ICollection)items).Count);
      else
        index = new Dictionary<TKey, TItem>();

      foreach (TItem item in items)
      {
        TKey key = keyGetter(item);
        if (!index.ContainsKey(key))
        {
          index[key] = item;
        }
      }
      return index;
    }

    /// <summary>
    /// оставляем для каждого ключа только первое вхождение
    /// </summary>
    public static Dictionary<TKey, TItem> MakeUniqueIndex<TKey, TItem>(IEnumerable<TItem> items,
      Getter<TKey, TItem> keyGetter)
    {
      Dictionary<TKey, TItem> index;
      if (items is ICollection<TItem>)
        index = new Dictionary<TKey, TItem>(((ICollection<TItem>)items).Count);
      else
        index = new Dictionary<TKey, TItem>();

      foreach (TItem item in items)
      {
        TKey key = keyGetter(item);
        index[key] = item;
      }
      return index;
    }

    /// <summary>
    /// оставляем для каждого ключа только первое вхождение
    /// </summary>
    public static ICollection<TItem> Distinct<TKey, TItem>(IEnumerable items, Getter<TKey, TItem> keyGetter)
    {
      return MakeUniqueIndex<TKey, TItem>(items, keyGetter).Values;
    }
    /// <summary>
    /// Оставляет только одно вхождение из коллекции
    /// </summary>
    public static ICollection<TItem> Distinct<TItem>(IEnumerable<TItem> items)
    {
      return MakeUniqueIndex<TItem, TItem>(items, delegate(TItem item) { return item; }).Values;
    }

    /// <summary>
    /// Отображение extractor должно быть инъективно, т. е. ключи должны быть уникальны.
    /// Если нет, то останется только последнее вхождение для каждого ключа.
    /// </summary>
    public static Dictionary<TKey, TItem> MakeIndex<TKey, TItem>(ICollection<TItem> col, Getter<TKey, TItem> extractor)
    {
      Dictionary<TKey, TItem> index = new Dictionary<TKey, TItem>(col.Count);
      foreach (TItem item in col)
      {
        index[extractor(item)] = item;
      }
      return index;
    }
    /// <summary>
    /// Значение при размножение не копируется (не клонируется)
    /// </summary>
    public static List<T> Repeat<T>(T value, int count)
    {
      List<T> values = new List<T>();
      for (int i = 0; i < count; ++i)
        values.Add(value);
      return values;
    }

    public static T PopSynchronized<T>(IList<T> items, object syncRoot)
    {
      lock (syncRoot)
      {
        if (items.Count == 0)
          return default(T);
        T item = items[0];
        items.RemoveAt(0);
        return item;
      }
    }

    public static List<T> From<T>(System.Collections.IEnumerable items)
    {
      List<T> res = new List<T>();
      if (items != null)
        foreach (T item in items)
          res.Add(item);
      return res;
    }

    public static List<T> Combine<T>(params IEnumerable<T>[] lists)
    {
      List<T> results = new List<T>();
      AddTo(results, lists);
      return results;
    }
    public static void AddTo<T>(List<T> results, params IEnumerable<T>[] lists)
    {
      foreach (IEnumerable<T> list in lists)
        results.AddRange(list);
    }

    public static bool Exist<T>(IEnumerable<T> items, T findItem)
    {
      foreach (T item in items)
      {
        if (object.Equals(item, findItem))
          return true;
      }
      return false;
    }

    public static TItem Find<TItem, TKey>(IEnumerable<TItem> items, TKey key,
      Getter<TKey, TItem> keyGetter)
    {
      foreach (TItem item in items)
      {
        if (object.Equals(keyGetter(item), key))
          return item;
      }
      return default(TItem);
    }

    public static T Find<T>(IEnumerable<T> items, Getter<bool, T> condition) //where T:class
    {
      foreach (T item in items)
      {
        if (condition(item))
          return item;
      }
      return default(T);
    }

    public static T? MinInStructs<T>(IEnumerable<T> items) where T : struct, IComparable<T>
    {
      T? min = null;
      foreach (T item in items)
      {
        if (min == null)
          min = item;
        else if (min.Value.CompareTo(item) > 0)
          min = item;
      }
      return min;
    }

    public static T? MaxInStructs<T>(IEnumerable<T> items) where T : struct, IComparable<T>
    {
      T? max = null;
      foreach (T item in items)
      {
        if (max == null)
          max = item;
        else if (max.Value.CompareTo(item) < 0)
          max = item;
      }
      return max;
    }
    public static T MinInClasses<T>(IEnumerable<T> items) where T : class, IComparable<T>
    {
      T min = null;
      foreach (T item in items)
      {
        if (min == null)
          min = item;
        else if (min.CompareTo(item) > 0)
          min = item;
      }
      return min;
    }

    public static T MaxInClasses<T>(IEnumerable<T> items) where T : class, IComparable<T>
    {
      T max = null;
      foreach (T item in items)
      {
        if (max == null)
          max = item;
        else if (max.CompareTo(item) < 0)
          max = item;
      }
      return max;
    }

    public static T MinInClasses<T>(IEnumerable<T> items, Comparison<T> comparision) where T : class
    {
      T min = null;
      foreach (T item in items)
      {
        if (min == null)
          min = item;
        else if (comparision(min, item) > 0)
          min = item;
      }
      return min;
    }

    public static T MaxInClasses<T>(IEnumerable<T> items, Comparison<T> comparision) where T : class
    {
      T max = null;
      foreach (T item in items)
      {
        if (max == null)
          max = item;
        else if (comparision(max, item) < 0)
          max = item;
      }
      return max;
    }

    public static T? MinInStructs<T>(IEnumerable<T> items, Comparison<T> comparision) where T : struct
    {
      T? min = null;
      foreach (T item in items)
      {
        if (min == null)
          min = item;
        else if (comparision(min.Value, item) > 0)
          min = item;
      }
      return min;
    }

    public static T? MaxInStructs<T>(IEnumerable<T> items, Comparison<T> comparision) where T : struct
    {
      T? max = null;
      foreach (T item in items)
      {
        if (max == null)
          max = item;
        else if (comparision(max.Value, item) < 0)
          max = item;
      }
      return max;
    }

    public static IEnumerable<T> NotNull<T>(IEnumerable<T> items)
    {
      if (items == null)
        return Array<T>.Empty;
      return items;
    }
    public static IList<T> NotNull<T>(IList<T> items)
    {
      if (items == null)
        return Array<T>.Empty;
      return items;
    }
    public static ICollection<T> NotNull<T>(ICollection<T> items)
    {
      if (items == null)
        return Array<T>.Empty;
      return items;
    }

    public static List<T> FindAll<T>(IEnumerable<T> items, Predicate<T> isGood)
    {
      List<T> results = new List<T>();
      foreach (T item in items)
        if (isGood(item))
          results.Add(item);
      return results;
    }
    public static List<object> FindAll(IEnumerable items, Predicate<object> isGood)
    {
      List<object> results = new List<object>();
      foreach (object item in items)
        if (isGood(item))
          results.Add(item);
      return results;
    }
 
    public static T[] ToArray1<T>(IEnumerable items)
    {
      if (items == null)
        return Array<T>.Empty;

      List<T> list = new List<T>();
      foreach (T item in items)
        list.Add(item);
      return list.ToArray();
    }

    public static T[] ToArray1<T>(ICollection items)
    {
      if (items == null)
        return Array<T>.Empty;

      T[] array = new T[items.Count];
      int index = 0;
      foreach (T item in items)
        array[index++] = item;
      return array;
    }

    public static T[] ToArray<T>(ICollection<T> items)
    {
      if (items == null)
        return Array<T>.Empty;

      T[] array = new T[items.Count];
      int index = 0;
      foreach (T item in items)
      {
        array[index++] = item;
      }
      return array;
    }

    static Random random = new Random();
    public static List<T> Shuffle<T>(IEnumerable<T> items)
    {
      List<T> tmpItems = new List<T>(items);
      List<T> results = new List<T>();
      while (tmpItems.Count > 0)
      {
        int index = random.Next(tmpItems.Count);
        results.Add(tmpItems[index]);
        tmpItems.RemoveAt(index);
      }
      return results;
    }

    public static List<TItem> SortBy<TKey, TItem>(IEnumerable<TItem> items, Getter<TKey, TItem> getter)
      where TKey : IComparable
    {
      List<TItem> list = new List<TItem>(items);
      list.Sort(delegate(TItem s1, TItem s2) { return getter(s1).CompareTo(getter(s2)); });
      return list;
    }
    public static List<TItem> SortBy<TKey, TItem>(IEnumerable<TItem> items, Getter<TKey, TItem> getter,
      Comparison<TKey> comparer)
    {
      List<TItem> list = new List<TItem>(items);
      list.Sort(delegate(TItem s1, TItem s2) { return comparer(getter(s1), getter(s2)); });
      return list;
    }

    //public static IList<T> Sort<T>(IEnumerable<T> items, Getter<object[], T> keyer)
    //{
    //  return SortBy<object[], T>(items, keyer, delegate(object[] props1, object[] props2)
    //          {
    //            for (int i = 0; i < props1.Length; ++i)
    //            {
    //              int cmp = 0;
    //              IComparable comparable1 = props1[i] as IComparable;
    //              if (comparable1 != null)
    //              {
    //                cmp = comparable1.CompareTo(props2[i]);
    //              }
    //              else
    //              {
    //                IComparable comparable2 = props2[i] as IComparable;
    //                if (comparable2 != null)
    //                {
    //                  cmp = -comparable2.CompareTo(props1[i]);
    //                }
    //              }
    //              if (cmp != 0)
    //                return cmp;
    //            }
    //            return 0;
    //          });
    //}


    public static T[] GetRange<T>(IList<T> items, int index, int count)
    {
      count = Math.Min(items.Count - index, count);
      if (count <= 0)
        return new T[] { };
      T[] results = new T[count];
      for (int i = 0; i < count; ++i)
      {
        results[i] = items[index + i];
      }
      return results;
    }
    public static bool AreEqual<T>(ICollection<T> left, ICollection<T> right, EqualityComparison<T> comparer)
    {
      if (comparer == null)
      {
        throw new ArgumentNullException("comparer");
      }
      if (left == null && right == null)
      {
        return true;
      }
      if (left == null || right == null)
      {
        return false;
      }
      if (left.Count != right.Count)
      {
        return false;
      }
      IEnumerator<T> leftEnumerator = left.GetEnumerator();
      IEnumerator<T> rightEnumerator = right.GetEnumerator();
      for (int i = 0; i < left.Count; i++)
      {
        leftEnumerator.MoveNext();
        rightEnumerator.MoveNext();
        T leftValue = leftEnumerator.Current;
        T rightValue = rightEnumerator.Current;
        if (leftValue == null && rightValue == null)
        {
          continue;
        }
        if (leftValue == null || rightValue == null)
        {
          return false;
        }

        if (!comparer(leftValue, rightValue))
        {
          return false;
        }
      }
      return true;
    }

    public static bool AreEqual<T>(ICollection<T> left, ICollection<T> right)
    {
      return AreEqual(left, right, delegate(T leftValue, T rightValue)
      {
        return (leftValue.Equals(rightValue));
      });
    }

    /// <summary>
    /// Выполняет лексикографическое сравнение двух коллекций. 
    /// Предполагается, что элементы обеих коллекций сравнимы друг с другом
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static int Compare<T>(ICollection<T> left, ICollection<T> right) where T : IComparable
    {
      if (left == null && right == null)
      {
        return 0;
      }
      if (left == null)
      {
        return -1;
      }
      if (right == null)
      {
        return 1;
      }
      IEnumerator<T> leftEnumerator = left.GetEnumerator();
      IEnumerator<T> rightEnumerator = right.GetEnumerator();
      int minCount = Math.Min(left.Count, right.Count);
      for (int i = 0; i < minCount; i++)
      {
        leftEnumerator.MoveNext();
        rightEnumerator.MoveNext();

        int current;
        if (leftEnumerator.Current == null && rightEnumerator.Current == null)
        {
          current = 0;
        }
        else
        {
          if (leftEnumerator.Current == null)
          {
            current = -1;
          }
          else
          {
            if (rightEnumerator.Current == null)
            {
              current = 1;
            }
            else
            {
              current = leftEnumerator.Current.CompareTo(rightEnumerator.Current);
            }
          }
        }
        if (current != 0)
        {
          return current;
        }
      }
      return left.Count.CompareTo(right.Count);
    }
    /// <summary>
    /// Возвращает первые count элементов коллекции.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<T> Top<T>(IEnumerable<T> items, int count)
    {
      List<T> top = new List<T>();

      if (items == null)
        return top;

      int i = 0;
      foreach (T item in items)
      {
        if (i < count)
          top.Add(item);
        else 
          return top;
        i++;
      }
      return top;
    }

    private static IEnumerable<T> Intersection<T>(List<T> list1, IEnumerable<T> list2)
    {
      foreach (T var in list2)
      {
        if (list1.Contains(var))
          yield return var;
      }
    }

    private static List<T> Diff<T>(IEnumerable<T> source, IEnumerable<T> substractor)
    {
      List<T> result = new List<T>(source);
      foreach (T var in substractor)
        result.Remove(var);
      return result;
    }

    public static int GetCount<T>(ICollection<T> items)
    {
      if (items == null)
        return 0;

      return items.Count;
    }

    public static int GetCount<T>(IEnumerable<T> items)
    {
      int count = 0;
      foreach (T item in items)
        count++;
      return count;
    }

    public static Dictionary<TKey, TValue> DictionaryFrom<TKey, TValue>(IEnumerable<TKey> collection, Getter<TValue, TKey> valueGetter)
    {
      Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
      foreach (TKey key in collection)
        dict.Add(key, valueGetter(key));
      return dict;
    }
    public static IList<T> Reverse<T>(IEnumerable<T> items)
    {
      List<T> result = new List<T>(items);
      result.Reverse();
      return result;
    }

    public static Dictionary<TKey, List<TItem>> GroupBy<TKey, TItem>(IEnumerable<TItem> items, Getter<TKey, TItem> keyer)
    {
      Dictionary<TKey, List<TItem>> groups = new Dictionary<TKey, List<TItem>>();
      foreach (TItem item in items)
      {
        TKey key = keyer(item);
        if (groups.ContainsKey(key))
          groups[key].Add(item);
        else
        {
          List<TItem> group = new List<TItem>();
          group.Add(item);
          groups[key] = group;
        }
      }
      return groups;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TKey, TValue, TItem>(IEnumerable<TItem> items,
      Getter<TKey, TItem> keyGetter, Getter<TValue, TItem> valueGetter)
    {
      Dictionary<TKey, List<TValue>> groups = new Dictionary<TKey, List<TValue>>();
      foreach (TItem item in items)
      {
        TKey key = keyGetter(item);
        TValue value = valueGetter(item);
        if (groups.ContainsKey(key))
          groups[key].Add(value);
        else
        {
          List<TValue> group = new List<TValue>();
          group.Add(value);
          groups[key] = group;
        }
      }
      return groups;
    }

    public static List<TItem> Sort<TItem>(IEnumerable<TItem> items)
    {
      List<TItem> result = new List<TItem>(items);
      result.Sort();
      return result;
    }

    public static List<T> Add<T>(IEnumerable<T> items, T item)
    {
      List<T> temp = new List<T>(items);
      temp.Add(item);
      return temp;
    }

    public static T[] ToArray<T>(IEnumerable<T> enumerable)
    {
      if (enumerable is ICollection<T>)
        return _.ToArray((ICollection<T>)enumerable);
      return new List<T>(enumerable).ToArray();
    }
  }
}
