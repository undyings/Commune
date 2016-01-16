using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class DictionaryHlp
  {
    public static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
    {
      return GetValueOrDefault(dict, key, default(TValue));
    }

    public static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue defValue)
    {
      if (key == null)
        return defValue;

      TValue value;
      if (dict.TryGetValue(key, out value))
        return value;
      return defValue;
    }

    public static void AddToGroupedDict<TKey, TValue>(IDictionary<TKey, List<TValue>> dict, TKey index, TValue value)
    {
      if (!dict.ContainsKey(index))
        dict[index] = new List<TValue>();
      dict[index].Add(value);
    }

    /// <summary>
    /// Если в словаре нет такого ключа, то размещает под указанным ключом указанное значение
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="defValue"></param>
    public static void AssignIfNotExist<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue defValue)
    {
      if (!dict.ContainsKey(key))
      {
        dict[key] = defValue;
      }
    }

    /// <summary>
    /// Присваивает значение по умолчанию под указанным ключом, если под таким ключом в словаре еще нет значения
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    public static void AssignDefaultIfNotExist<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
    {
      AssignIfNotExist(dict, key, default(TValue));
    }


    public static TValue? FindValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) 
      where TValue : struct
    {
      TValue value;
      if (dictionary.TryGetValue(key, out value))
      {
        return value;
      }
      return null;
    }


    static TValue GetOrAssignIfNotExists<TKey, TValue>(IDictionary<TKey, TValue> dict,
      TKey key, Getter<TValue> defaultValueGetter)
    {
      TValue value;
      if (dict.TryGetValue(key, out value))
        return value;
      value = defaultValueGetter();
      dict.Add(key, value);
      return value;
    }
  }
}
