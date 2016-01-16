using System;
using System.Collections.Generic;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public class HColumn<T> : ToneContainer, IHtmlColumn
  {
    readonly string name;
    public string Name
    {
      get
      {
        return name;
      }
    }
    readonly Getter<IHtmlControl, T> cellGetter;
    public HColumn(string name, Getter<IHtmlControl, T> cellGetter)
    {
      this.name = name;
      this.cellGetter = cellGetter;
    }

    public IHtmlControl GetCell(object row)
    {
      return cellGetter((T)row);
    }
     
  }
}
