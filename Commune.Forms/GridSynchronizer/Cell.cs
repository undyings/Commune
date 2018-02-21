using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Drawing;

namespace Commune.Forms
{
  public class Cell
  {
    public readonly object Value;
    readonly Dictionary<string, object> propertyByName = new Dictionary<string, object>();

    public Cell(object value, params ColumnExtensionAttribute[] properties)
    {
      this.Value = value;
      foreach (ColumnExtensionAttribute prop in properties)
        propertyByName[prop.Name] = prop.Value;
    }

    public object Get(string propertyName)
    {
      return DictionaryHlp.GetValueOrDefault(propertyByName, propertyName);
    }
  }
}