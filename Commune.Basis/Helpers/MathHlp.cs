#region Using directives

using System;
using System.Collections.Generic;
using System.Text;


#endregion
//using NullableTypes;

namespace Commune.Basis
{
  public class MathHlp
  {
    //        public static NullableDouble Pow(NullableDouble value, NullableDouble power)
    //        {
    //            if (value.IsNull || power.IsNull)
    //                return NullableDouble.Null;
    //            return Math.Pow(value.Value, power.Value);
    //        }
    //
    //        public static NullableDouble Abs(NullableDouble value)
    //        {
    //            if (value.IsNull)
    //                return NullableDouble.Null;
    //            return Math.Abs(value.Value);
    //        }
    //
    //        public static NullableDouble Sqrt(NullableDouble value)
    //        {
    //            if (value.IsNull)
    //                return NullableDouble.Null;
    //            return Math.Sqrt(value.Value);
    //        }
    //        public static NullableDouble Round(NullableDouble value, double e_value)
    //        {
    //            if (value.IsNull)
    //                return NullableDouble.Null;
    //            return Round(value.Value, e_value);
    //        }

    public static int NotNull(int i)
    {
      if (i == 0)
        return 1;
      return i;
    }
    public static double NotNull(double i)
    {
      if (i == 0)
        return 1;
      return i;
    }

    public static int IntParse(string stringNumber)
    {
      try
      {
        return int.Parse(stringNumber);
      }
      catch (Exception exc)
      {
        throw new Exception(string.Format("Строку '{0}' не получилось перевести в число", stringNumber), exc);
      }
    }

    //    public static NullableDouble Round(NullableDouble value, double? e_value)
    //    {
    //      if (e_value == null)
    //        return value;
    //      if (value.IsNull)
    //        return NullableDouble.Null;
    //      return Round(value.Value, e_value.Value);
    //    }

    // Не удалять, используется из-за простоты имени Mod.
    [Obsolete("Использовать Math.IEEERemainder")]
    public static double Mod(double divident, double divisor)
    {
      double result = divident - Math.Floor(divident / divisor) * divisor;
      return result;
    }

    public static bool IsEquals(double d1, double d2)
    {
      return (Math.Abs(d1 - d2) <= double.Epsilon);
    }
    public static int Bound(int value, int min, int max)
    {
      if (value < min)
        return min;
      if (value > max)
        return max;
      return value;
    }
    public static double Sqr(double value)
    {
      return value * value;
    }

    public static bool IsBetween(int x, int v1, int v2)
    {
      if (v1 > v2)
        ObjectHlp.Swap(ref v1, ref v2);
      return v1 <= x && x <= v2;
    }

    #region = AlignLow, Up =
    public static int AlignLow(int n, int period)
    {
      return n - n % period;
    }
    public static long AlignLow(long n, long period)
    {
      return n - n % period;
    }
    public static uint AlignLow(uint n, uint period)
    {
      return n - n % period;
    }
    public static ulong AlignLow(ulong n, ulong period)
    {
      return n - n % period;
    }
    public static float AlignLow(float n, float period)
    {
      return n - n % period;
    }
    public static double AlignLow(double n, double period)
    {
      return n - n % period;
    }
    public static TimeSpan AlignLow(TimeSpan n, TimeSpan period)
    {
      return TimeSpan.FromTicks(n.Ticks - n.Ticks % period.Ticks);
    }
    public static DateTime AlignLow(DateTime date, TimeSpan period)
    {
      return new DateTime(MathHlp.AlignLow(date.Ticks, period.Ticks));
    }


    public static int AlignUp(int n, int period)
    {
      return ((n + period - 1) / period) * period;
    }
    public static long AlignUp(long n, long period)
    {
      return ((n + period - 1) / period) * period;
    }
    public static uint AlignUp(uint n, uint period)
    {
      return ((n + period - 1) / period) * period;
    }
    public static ulong AlignUp(ulong n, ulong period)
    {
      return ((n + period - 1) / period) * period;
    }
    public static float AlignUp(float n, float period)
    {
      return (float)Math.Ceiling(n / period) * period;
    }
    public static double AlignUp(double n, double period)
    {
      return Math.Ceiling(n / period) * period;
    }
    public static TimeSpan AlignUp(TimeSpan n, TimeSpan period)
    {
      return TimeSpan.FromTicks(((n.Ticks + period.Ticks - 1) / period.Ticks) * period.Ticks);
    }
    public static DateTime AlignUp(DateTime date, TimeSpan period)
    {
      return new DateTime(MathHlp.AlignUp(date.Ticks, period.Ticks));
    }

    #endregion

    public static T Min<T>(List<T> values) where T : IComparable<T>
    {
      if (values.Count < 1)
        throw new ArgumentException("Невозможно вычислить минимум, коллекция пустая");

      T first = values[0];
      if (values.Count == 1)
        return first;

      T min = first;
      foreach (T value in values)
      {
        T next = value;
        if (next.CompareTo(min) == -1)
          min = next;
      }
      return min;
    }

    public static T Max<T>(List<T> values) where T : IComparable<T>
    {
      if (values.Count < 1)
        throw new ArgumentException("Невозможно вычислить минимум, коллекция пустая");

      T first = values[0];
      if (values.Count == 1)
        return first;

      T max = first;
      foreach (T value in values)
      {
        T next = value;
        if (next.CompareTo(max) == 1)
          max = next;
      }
      return max;
    }

    public static T Max<T>(T minusInfinity, IEnumerable<T> collection)
      where T : IComparable<T>
    {
      T result = minusInfinity;
      foreach (T val in collection)
        if (val.CompareTo(result) > 0)
          result = val;
      return result;
    }

    public static T Min<T>(T plusInfinity, IEnumerable<T> collection)
      where T : IComparable<T>
    {
      T result = plusInfinity;
      foreach (T val in collection)
        if (val.CompareTo(result) < 0)
          result = val;
      return result;
    }

    public static T Max<T>(params T[] args)
      where T : IComparable<T>
    {
      bool inited = false;
      T result = default(T);
      foreach (T val in args)
      {
        if (!inited || val.CompareTo(result) > 0)
        {
          result = val;
          inited = true;
        }
      }
      if(!inited)
        throw new ArgumentException("Невозможно вычислить минимум, коллекция пустая");
      return result;
    }

    public static T Min<T>(params T[] args)
      where T : IComparable<T>
    {
      bool inited = false;
      T result = default(T);
      foreach (T val in args)
      {
        if (!inited || val.CompareTo(result) < 0)
        {
          result = val;
          inited = true;
        }
      }
      if (!inited)
        throw new ArgumentException("Невозможно вычислить минимум, коллекция пустая");
      return result;
    }

    public static int ArgMin<T>(params T[] args)
      where T : IComparable<T>
    {
      int result = -1;
      for (int i = 0; i < args.Length; i++)
      {
        if (result == -1 || args[i].CompareTo(args[result]) < 0)
          result = i;
      }
      return result;
    }

    public static int ArgMax<T>(params T[] args)
      where T : IComparable<T>
    {
      int result = -1;
      for (int i = 0; i < args.Length; i++)
      {
        if (result == -1 || args[i].CompareTo(args[result]) > 0)
          result = i;
      }
      return result;
    }


  }
}
