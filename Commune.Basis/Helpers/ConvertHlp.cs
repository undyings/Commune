using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class ConvertHlp
  {
    public static bool IsNull(object value)
    {
      return (value == null || Convert.IsDBNull(value)); //value == DBNull.Value);
    }

    public static bool IsNullableType<T>()
    {
      return (typeof(T).IsGenericType && !typeof(T).IsGenericTypeDefinition &&
          typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    public static T ToType<T>(object value)
    {
      Type type = typeof(T);
      if (IsNullableType<T>())
      {
        if (IsNull(value))
          return default(T);
        type = Nullable.GetUnderlyingType(typeof(T));
      }
      return (T)Convert.ChangeType(value, type);
    }

    public static T ToTypeOrDefault<T>(object value)
    {
      if (IsNull(value))
        return default(T);

      try
      {
        return ToType<T>(value);
      }
      catch (Exception)
      {
        return default(T);
      }
    }

    public static double? ToDouble(object value)
    {
      try
      {
        if (value == null || value is DBNull)
          return null;
        if (value is string)
        {
          string s = (string)value;
          return double.Parse(s.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }
        return Convert.ToDouble(value);
      }
      catch (Exception exc)
      {
    //    TraceHlp2.WriteException(exc);
      }
      return null;
    }

    public static Guid ToGuid(object value)
    {
      if (value is Guid)
        return (Guid)value;
      else if (value is byte[])
        return new Guid((byte[])value);
      else
        return new Guid(value.ToString());
    }

    public static Guid? ToGuid_Safe(object value)
    {
      try
      {
        if (value is Guid)
          return (Guid)value;
        else if (value is byte[])
          return new Guid((byte[])value);
        else if (value is string)
        {
          var s = (string)value;
          if (s == "")
            return null;
          return new Guid(s);
        }
        else
          return new Guid(value.ToString());
      }
      catch (Exception)
      {
      }
      return null;
    }

    public static string GuidToBase64(Guid id)
    {
      return Convert.ToBase64String(id.ToByteArray());
    }

    //int? ToInt(object value)
    //{
    //  if (value == null)
    //    return null;
    //  else
    //    return Convert.ToInt32(value);
    //}

    public static int? ToInt(object value)
    {
      try
      {
        if (value == null || value == DBNull.Value)
          return null;
        return Convert.ToInt32(value);
      }
      catch (Exception exc)
      {
      }
      return null;
    }
  }
}
