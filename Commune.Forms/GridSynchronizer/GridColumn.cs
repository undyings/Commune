using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Forms
{
  public class GridColumn<T> : IGridColumn, ISupportDefaultExtensionColumn
  {
    readonly string name;
    public readonly Getter<object, T> getter;
    public readonly Executter<T, object> setter;
    readonly Dictionary<string, ColumnExtensionAttribute> extensionsByName = new Dictionary<string, ColumnExtensionAttribute>();
    public GridColumn(string name, Getter<object, T> getter, Executter<T, object> setter,
      params ColumnExtensionAttribute[] extensions)
    {
      this.name = name;
      this.getter = getter;
      this.setter = setter;
      AddExtensions(extensions);
    }

    void AddExtensions(params ColumnExtensionAttribute[] extensions)
    {
      foreach (ColumnExtensionAttribute extension in extensions)
      {
        if (extension == null)
          continue;
        if (extensionsByName.ContainsKey(extension.Name))
          Logger.AddMessage("В колонке '{0}' расширение '{1}' задается повторно", Name, extension.Name);
        extensionsByName[extension.Name] = extension;
      }
    }

    public string Name
    {
      get { return name; }
    }

    public object GetValue(object row)
    {
      if (getter != null && (row == null || row is T))
        return getter((T)row);
      return null;
    }

    public void SetValue(object row, object value)
    {
      if (setter != null && (row == null || row is T))
        setter((T)row, value);
    }

    protected object GetInnerExtended(string extensionName)
    {
      if (extensionName == "IsReadOnly")
        return setter == null;

      ColumnExtensionAttribute extensionAttr;
      if (extensionsByName.TryGetValue(extensionName, out extensionAttr))
        return extensionAttr.Value;

      return null;
    }

    protected object GetDefaultExtended(string extensionName)
    {
      Getter<IGridColumn> getter = GetInnerExtended("DefaultRowSettings") as Getter<IGridColumn>;
      if (getter == null)
        return null;
      IGridColumn column = getter();
      return column != null ? column.GetExtended(extensionName) : null;
    }

    public object GetExtended(string extensionName)
    {
      object extended = GetInnerExtended(extensionName);
      if (extended != null)
        return extended;
      return GetDefaultExtended(extensionName);
    }

    public void WithExtension(ColumnExtensionAttribute extension)
    {
      AddExtensions(extension);
    }
  }
}
