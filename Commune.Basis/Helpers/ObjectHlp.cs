#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion
//using NullableTypes;

namespace Commune.Basis
{
  public class ObjectHlp
  {
      public static object GetDefaultValue(Type type)
      {
          if (type.IsClass)
              return null;
          else
              return Activator.CreateInstance(type);

      }

      /// <summary>
      /// If returned value is true result is obsolete - you need to continue comparing
      /// </summary>
      /// <param name="o1"></param>
      /// <param name="o2"></param>
      /// <param name="Result of comparing "></param>
      /// <returns>Whether you need to continue comparing</returns>
      public static bool NullCompare(object o1, object o2, out bool result)
      {
          result = false;
          if (o1 == null && o2 == null)
              result = true;
          else if (o1 == null || o2 == null)
              result = false;
          else return true;
          return false;
      }

    public static bool AsStringCompare(object o1, object o2)
    {
      if (o1 == null && o2 == null)
        return true;
      if (o1 == null || o2 == null)
        return false;
      return o1.ToString().Equals(o2.ToString());
    }

      public static bool IsEquals<T>(T o1, T o2)
      {
          if ((object)o1 == null && (object)o2 == null)
              return true;
          if ((object)o1 == null || (object)o2 == null)
              return false;
          return o1.Equals(o2);
      }

      public static string ToString<T>(T item)
      {
          if (item == null)
              return null;
          return item.ToString();
      }
      public static string ToString(object o, string nullString)
      {
          if (o == null)
              return nullString;
          return o.ToString();
      }
      public static string ToString(object o, bool isDisplayNull)
      {
          if (o == null)
              return isDisplayNull ? "[null]" : null;
          return o.ToString();
      }

      public static object CreateDefaultValue(Type type)
      {
          try
          {
              return Activator.CreateInstance(type);
          }
          catch (Exception exc)
          {
              throw new Exception(string.Format("Не получилось создать объект по умолчанию для типа '{0}'", type), exc);
          }
      }
      static Random random = new Random();
      public static object CreateRandomValue(Type type)
      {
          if (type == typeof(int))
              return random.Next();
          if (type == typeof(double))
              return ((double)random.Next()) / 10;
          if (type == typeof(string))
              return "worker" + (random.Next() % 20 + 1).ToString();

          return CreateDefaultValue(type);
      }
      public static Type CreateRandomType()
      {
          switch ((int)(random.Next() % 4))
          {
              case 0: return typeof(String);
              case 1: return typeof(DateTime);
              case 2: return typeof(int);
              case 3: return typeof(double);
              case 4: return typeof(TimeSpan);
          }
          return typeof(int);
      }
      [Obsolete("Используйте функцию NotNull<T>")]
      public static object IfNull(object checkingValue, object defaultValue)
      {

          if (checkingValue == null)
              return defaultValue;
          else
              return checkingValue;
      }
      public static object IfNull(object checkingValue, object ifNullValue, object ifNotNullValue)
      {
          if (checkingValue == null)
              return ifNullValue;
          else
              return ifNotNullValue;
      }

    public static void Swap<T>(ref T t1, ref T t2)
    {
      T t = t1;
      t1 = t2;
      t2 = t;
    }
  }
}
