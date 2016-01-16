using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Data;

namespace Commune.Data
{
  public class FieldBlank
  {
    public readonly string FieldName;
    public readonly Getter<object, object> ToGridValue;
    public readonly Getter<object, object> ToDataBaseValue;
    public readonly object DefaultValue;

    public FieldBlank(string fieldName, Getter<object, object> toGridValue,
      Getter<object, object> toDataBaseValue, object defaultValue)
    {
      this.FieldName = fieldName;
      this.ToGridValue = toGridValue;
      this.ToDataBaseValue = toDataBaseValue;
      this.DefaultValue = defaultValue;
    }

    public object GetValue(IRowLink rowLink)
    {
      if (rowLink == null)
        return null;
      object rawValue = rowLink.GetValue(FieldName);
      if (rawValue == DBNull.Value || rawValue == null)
        return null;
      if (ToGridValue != null)
        return ToGridValue(rawValue);
      return rawValue;
    }

    public void SetValue(IRowLink rowLink, object value)
    {
      if (rowLink == null)
        return;

      object rawValue = value;
      if (ToDataBaseValue != null)
        rawValue = ToDataBaseValue(value);
      if (rawValue == null)
        rawValue = DBNull.Value;

      rowLink.SetValue(FieldName, rawValue);
    }

    public void SetDefaultValue(IRowLink rowLink)
    {
      SetValue(rowLink, DefaultValue);
    }

    //Extension
    public void Copy(IRowLink destRow, IRowLink sourceRow)
    {
      object sourceValue = GetValue(sourceRow);
      if (!ObjectHlp.IsEquals(destRow.GetValue(FieldName), sourceValue))
        SetValue(destRow, sourceValue);
    }
  }

  public class FieldBlank<T> : FieldBlank, IPropertyBlank<Nothing, T>
  {
    public T Get(IRowLink rowLink)
    {
      object value = GetValue(rowLink);
      try
      {
        if (value == null)
          return default(T);
        return (T)value;
      }
      catch (Exception)
      {
        Logger.AddMessage("Cast: {0}, {1}, {2}", value, value.GetType(), typeof(T));
        throw;
      }
    }

    public void Set(IRowLink rowLink, T value)
    {
      SetValue(rowLink, value);
    }

    public T[] All(params IRowLink[] rowLinks)
    {
      List<T> values = new List<T>(rowLinks.Length);
      foreach (IRowLink rowLink in rowLinks)
        values.Add(Get(rowLink));
      return values.ToArray();
    }

    public T Any(params IRowLink[] rowLinks)
    {
      if (rowLinks.Length == 0)
        return default(T);
      return Get(rowLinks[0]);
    }

    public FieldBlank(string fieldName, Getter<object, object> toGridValue,
      Getter<object, object> toDataBaseValue, T defaultValue) :
      base(fieldName, toGridValue, toDataBaseValue, defaultValue)
    {
    }

    public FieldBlank(string fieldName, T defaultValue) :
      this(fieldName, null, null, defaultValue)
    {
    }

    public FieldBlank(string fieldName) :
      this(fieldName, null, null, default(T))
    {
    }

    public FieldBlank(string fieldName, IFieldConverter fieldConverter, T defaultValue) :
      this(fieldName, fieldConverter.ToGridValue, fieldConverter.ToDatabaseValue, defaultValue)
    {
    }

    public FieldBlank(string fieldName, IFieldConverter fieldConverter) :
      this(fieldName, fieldConverter, default(T))
    {
    }

    public Nothing Kind
    {
      get { return null; }
    }

    FieldBlank<T> IPropertyBlank<Nothing, T>.Field
    {
      get { return this; }
    }
  }

  public class FieldLink
  {
    public readonly FieldBlank FieldBlank;
    public readonly int ColumnIndex;
    public readonly bool IsIndicesPart;

    public FieldLink(FieldBlank fieldBlank, int columnIndex, bool isIndicesPart)
    {
      this.FieldBlank = fieldBlank;
      this.ColumnIndex = columnIndex;
      this.IsIndicesPart = isIndicesPart;
    }
  }
}
