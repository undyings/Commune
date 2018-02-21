using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Forms
{
  public interface IMultiColumn : IGridColumn
  {
    object GetExtended(string name, object row);
  }

  public abstract class MultiColumn : ISupportDefaultExtensionColumn
  {
    readonly string name;
    readonly IGridColumn[] columns;
    readonly Dictionary<string, ColumnExtensionAttribute> extensionsByName =
      new Dictionary<string, ColumnExtensionAttribute>();
    public MultiColumn(string name, IGridColumn[] columns, params ColumnExtensionAttribute[] extensions)
    {
      this.name = name;
      this.columns = columns;
      AddExtensions(extensions);
    }

    void AddExtensions(params ColumnExtensionAttribute[] extensions)
    {
      foreach (ColumnExtensionAttribute extension in extensions)
      {
        if (extensionsByName.ContainsKey(extension.Name))
          Logger.AddMessage("В колонке '{0}' расширение '{1}' задается повторно", Name, extension.Name);
        extensionsByName[extension.Name] = extension;
      }
    }

    public string Name
    {
      get { return name; }
    }

    protected object GetMultiExtended(string extensionName)
    {
      if (extensionName == "IsReadOnly")
      {
        foreach (IGridColumn column in columns)
        {
          if (column != null && (bool)column.GetExtended(extensionName) == false)
            return false;
        }
        return true;
      }

      ColumnExtensionAttribute extensionAttr;
      if (extensionsByName.TryGetValue(extensionName, out extensionAttr))
        return extensionAttr.Value;

      return null;
    }

    protected object GetDefaultExtended(string extensionName)
    {
      Getter<IGridColumn> getter = GetMultiExtended("DefaultRowSettings") as Getter<IGridColumn>;
      if (getter == null)
        return null;
      IGridColumn column = getter();
      return column != null ? column.GetExtended(extensionName) : null;
    }

    protected object GetDefaultExtended(string extensionName, object row)
    {
      Getter<IGridColumn> getter = GetMultiExtended("DefaultRowSettings") as Getter<IGridColumn>;
      if (getter == null)
        return null;
      return SynchronizerHlp.GetExtended(getter(), extensionName, row);
    }

    public void WithExtension(ColumnExtensionAttribute extension)
    {
      AddExtensions(extension);
    }

    protected static bool IsOwnRow<T>(GridColumn<T> column, object row)
    {
      return row is T && !SynchronizerHlp.IsAnotherRow(column, row);
    }
  }

  public class MultiColumn<T1, T2> : MultiColumn, IMultiColumn
  {
    readonly GridColumn<T1> column1;
    readonly GridColumn<T2> column2;
    public MultiColumn(string name, GridColumn<T1> column1, GridColumn<T2> column2,
      params ColumnExtensionAttribute[] extensions) :
      base(name, new IGridColumn[] { column1, column2 }, extensions)
    {
      this.column1 = column1;
      this.column2 = column2;
    }

    public object GetValue(object row)
    {
      if (IsOwnRow(column1, row))
        return column1.GetValue(row);
      if (IsOwnRow(column2, row))
        return column2.GetValue(row);
      return null;
    }

    public void SetValue(object row, object value)
    {
      if (IsOwnRow(column1, row))
        column1.SetValue(row, value);
      else if (IsOwnRow(column2, row))
        column2.SetValue(row, value);
    }

    public object GetExtended(string name)
    {
      object extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name);
    }

    public object GetExtended(string name, object row)
    {
      object extended = null;
      if (IsOwnRow(column1, row))
        extended = column1.GetExtended(name);
      else if (IsOwnRow(column2, row))
        extended = column2.GetExtended(name);
      if (extended != null)
        return extended;
      extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name, row);
    }
  }

  public class MultiColumn<T1, T2, T3> : MultiColumn, IMultiColumn
  {
    readonly GridColumn<T1> column1;
    readonly GridColumn<T2> column2;
    readonly GridColumn<T3> column3;
    public MultiColumn(string name, GridColumn<T1> column1, GridColumn<T2> column2, GridColumn<T3> column3,
      params ColumnExtensionAttribute[] extensions) :
      base(name, new IGridColumn[] { column1, column2, column3 }, extensions)
    {
      this.column1 = column1;
      this.column2 = column2;
      this.column3 = column3;
    }

    public object GetValue(object row)
    {
      if (IsOwnRow(column1, row))
        return column1.GetValue(row);
      if (IsOwnRow(column2, row))
        return column2.GetValue(row);
      if (IsOwnRow(column3, row))
        return column3.GetValue(row);
      return null;
    }

    public void SetValue(object row, object value)
    {
      if (IsOwnRow(column1, row))
        column1.SetValue(row, value);
      else if (IsOwnRow(column2, row))
        column2.SetValue(row, value);
      else if (IsOwnRow(column3, row))
        column3.SetValue(row, value);
    }

    public object GetExtended(string name)
    {
      object extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name);
    }

    public object GetExtended(string name, object row)
    {
      object extended = null;
      if (IsOwnRow(column1, row))
        extended = column1.GetExtended(name);
      else if (IsOwnRow(column2, row))
        extended = column2.GetExtended(name);
      else if (IsOwnRow(column3, row))
        extended = column3.GetExtended(name);
      if (extended != null)
        return extended;
      extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name, row);
    }
  }

  public class MultiColumn<T1, T2, T3, T4> : MultiColumn, IMultiColumn
  {
    readonly GridColumn<T1> column1;
    readonly GridColumn<T2> column2;
    readonly GridColumn<T3> column3;
    readonly GridColumn<T4> column4;
    public MultiColumn(string name, GridColumn<T1> column1, GridColumn<T2> column2, GridColumn<T3> column3,
      GridColumn<T4> column4, params ColumnExtensionAttribute[] extensions) :
      base(name, new IGridColumn[] { column1, column2, column3, column4 }, extensions)
    {
      this.column1 = column1;
      this.column2 = column2;
      this.column3 = column3;
      this.column4 = column4;
    }

    public object GetValue(object row)
    {
      if (IsOwnRow(column1, row))
        return column1.GetValue(row);
      if (IsOwnRow(column2, row))
        return column2.GetValue(row);
      if (IsOwnRow(column3, row))
        return column3.GetValue(row);
      if (IsOwnRow(column4, row))
        return column4.GetValue(row);
      return null;
    }

    public void SetValue(object row, object value)
    {
      if (IsOwnRow(column1, row))
        column1.SetValue(row, value);
      else if (IsOwnRow(column2, row))
        column2.SetValue(row, value);
      else if (IsOwnRow(column3, row))
        column3.SetValue(row, value);
      else if (IsOwnRow(column4, row))
        column4.SetValue(row, value);
    }

    public object GetExtended(string name)
    {
      object extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name);
    }

    public object GetExtended(string name, object row)
    {
      object extended = null;
      if (IsOwnRow(column1, row))
        extended = column1.GetExtended(name);
      else if (IsOwnRow(column2, row))
        extended = column2.GetExtended(name);
      else if (IsOwnRow(column3, row))
        extended = column3.GetExtended(name);
      else if (IsOwnRow(column4, row))
        extended = column4.GetExtended(name);
      if (extended != null)
        return extended;
      extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name, row);
    }
  }

  public class MultiColumn<T1, T2, T3, T4, T5> : MultiColumn, IMultiColumn
  {
    readonly GridColumn<T1> column1;
    readonly GridColumn<T2> column2;
    readonly GridColumn<T3> column3;
    readonly GridColumn<T4> column4;
    readonly GridColumn<T5> column5;
    public MultiColumn(string name, GridColumn<T1> column1, GridColumn<T2> column2, GridColumn<T3> column3,
      GridColumn<T4> column4, GridColumn<T5> column5, params ColumnExtensionAttribute[] extensions) :
      base(name, new IGridColumn[] { column1, column2, column3, column4, column5 }, extensions)
    {
      this.column1 = column1;
      this.column2 = column2;
      this.column3 = column3;
      this.column4 = column4;
      this.column5 = column5;
    }

    public object GetValue(object row)
    {
      if (IsOwnRow(column1, row))
        return column1.GetValue(row);
      if (IsOwnRow(column2, row))
        return column2.GetValue(row);
      if (IsOwnRow(column3, row))
        return column3.GetValue(row);
      if (IsOwnRow(column4, row))
        return column4.GetValue(row);
      if (IsOwnRow(column5, row))
        return column5.GetValue(row);
      return null;
    }

    public void SetValue(object row, object value)
    {
      if (IsOwnRow(column1, row))
        column1.SetValue(row, value);
      else if (IsOwnRow(column2, row))
        column2.SetValue(row, value);
      else if (IsOwnRow(column3, row))
        column3.SetValue(row, value);
      else if (IsOwnRow(column4, row))
        column4.SetValue(row, value);
      else if (IsOwnRow(column5, row))
        column5.SetValue(row, value);
    }

    public object GetExtended(string name)
    {
      object extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name);
    }

    public object GetExtended(string name, object row)
    {
      object extended = null;
      if (IsOwnRow(column1, row))
        extended = column1.GetExtended(name);
      else if (IsOwnRow(column2, row))
        extended = column2.GetExtended(name);
      else if (IsOwnRow(column3, row))
        extended = column3.GetExtended(name);
      else if (IsOwnRow(column4, row))
        extended = column4.GetExtended(name);
      else if (IsOwnRow(column5, row))
        extended = column5.GetExtended(name);
      if (extended != null)
        return extended;
      extended = GetMultiExtended(name);
      if (extended != null)
        return extended;
      return GetDefaultExtended(name, row);
    }
  }
}