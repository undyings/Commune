using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class CollectionSynchronizer
  {
    //public static void Synchronize<TLeft, TRight, TKey>(
    //  IEnumerable<TLeft> left, Getter<TKey, TLeft> leftKeyer,
    //  IEnumerable<TRight> right, Getter<TKey, TRight> rightKeyer,
    //  Executter<TLeft> onlyLeft, Executter<TRight> onlyRight, Executter<TLeft, TRight> both)
    //  where TLeft : class
    //  where TRight : class
    //{
    //  Dictionary<TKey, TLeft> leftIndex = CollectionHlp.MakeUniqueIndex(left, leftKeyer);
    //  Dictionary<TKey, TRight> rightIndex = CollectionHlp.MakeUniqueIndex(right, rightKeyer);

    //  Synchronize(
    //    left, leftKeyer, delegate(TKey key) { return DictionaryHlp.GetValueOrDefault(leftIndex, key); },
    //    right, rightKeyer, delegate(TKey key) { return DictionaryHlp.GetValueOrDefault(rightIndex, key); },
    //    onlyLeft, onlyRight, both);
    //}

    public static void Synchronize<TLeft, TRight, TKey>(
      IEnumerable<TLeft> left, Getter<TKey, TLeft> leftKeyer,
      IEnumerable<TRight> right, Getter<TKey, TRight> rightKeyer,
      Executter<TLeft> onlyLeft, Executter<TRight> onlyRight, Executter<TLeft, TRight> both)
    {
      Dictionary<TKey, TRight> rightIndex = CollectionHlp.MakeUniqueIndex(right, rightKeyer);

      foreach (TLeft l in left)
      {
        TKey lkey = leftKeyer(l);
        TRight r;
        if (rightIndex.TryGetValue(lkey, out r))
        {
          both(l, r);
          rightIndex.Remove(lkey);
        }
        else
        {
          onlyLeft(l);
        }
      }

      foreach (KeyValuePair<TKey, TRight> rPair in rightIndex)
        onlyRight(rPair.Value);
    }

    public static void Synchronize<TLeft, TRight, TKey>(
      IEnumerable<TLeft> left, Getter<TKey, TLeft> leftKeyer, Getter<TLeft, TKey> leftIndexer,
      IEnumerable<TRight> right, Getter<TKey, TRight> rightKeyer, Getter<TRight, TKey> rightIndexer,
      Executter<TLeft> onlyLeft, Executter<TRight> onlyRight, Executter<TLeft, TRight> both)
      where TLeft : class
      where TRight : class
    {
      foreach (TLeft l in left)
      {
        TKey lKey = leftKeyer(l);

        TRight r = rightIndexer(lKey);
        if (r == null)
          onlyLeft(l);
        else
          both(l, r);
      }

      foreach (TRight r in right)
      {
        TKey rKey = rightKeyer(r);
        if (leftIndexer(rKey) == null)
          onlyRight(r);
      }
    }

    public static CompareResults<TLeft, TRight> Compare<TLeft, TRight, TKey>(
      IEnumerable<TLeft> left, Getter<TKey, TLeft> leftKeyer,
      IEnumerable<TRight> right, Getter<TKey, TRight> rightKeyer)
      where TLeft : class
      where TRight : class
    {
      Dictionary<TKey, TLeft> leftIndex = CollectionHlp.MakeUniqueIndex(left, leftKeyer);
      Dictionary<TKey, TRight> rightIndex = CollectionHlp.MakeUniqueIndex(right, rightKeyer);

      return Compare(
        left, leftKeyer, delegate(TKey key) { return DictionaryHlp.GetValueOrDefault(leftIndex, key); },
        right, rightKeyer, delegate(TKey key) { return DictionaryHlp.GetValueOrDefault(rightIndex, key); });
    }

    public static CompareResults<TLeft, TRight> Compare<TLeft, TRight, TKey>(
      IEnumerable<TLeft> left, Getter<TKey, TLeft> leftKeyer, Getter<TLeft, TKey> leftIndexer,
      IEnumerable<TRight> right, Getter<TKey, TRight> rightKeyer, Getter<TRight, TKey> rightIndexer)
      where TLeft : class
      where TRight : class
    {
      List<TLeft> onlyLeft = new List<TLeft>();
      List<KeyValuePair<TLeft, TRight>> pairs = new List<KeyValuePair<TLeft,TRight>>();
      List<TRight> onlyRight = new List<TRight>();

      foreach (TLeft l in left)
      {
        TKey lKey = leftKeyer(l);

        TRight r = rightIndexer(lKey);
        if (r == null)
          onlyLeft.Add(l);
        else
          pairs.Add(new KeyValuePair<TLeft,TRight>(l, r));
      }

      foreach (TRight r in right)
      {
        TKey rKey = rightKeyer(r);
        if (leftIndexer(rKey) == null)
          onlyRight.Add(r);
      }

      return new CompareResults<TLeft, TRight>(onlyLeft, pairs, onlyRight);
    }

    public class CompareResults<TLeft, TRight>
    {
      public CompareResults(List<TLeft> onlyLeft, List<KeyValuePair<TLeft, TRight>> pairs,
        List<TRight> onlyRight)
      {
        this.OnlyLeft = onlyLeft;
        this.Pairs = pairs;
        this.OnlyRight = onlyRight;
      }

      public List<TLeft> OnlyLeft;
      public List<KeyValuePair<TLeft, TRight>> Pairs;
      public List<TRight> OnlyRight;

      public IEnumerable<TLeft> PairsLeft
      {
        get
        {
          foreach (KeyValuePair<TLeft, TRight> pair in Pairs)
            yield return pair.Key;
        }
      }

      public IEnumerable<TRight> PairsRight
      {
        get
        {
          foreach (KeyValuePair<TLeft, TRight> pair in Pairs)
            yield return pair.Value;
        }
      }
    }
  }
}
