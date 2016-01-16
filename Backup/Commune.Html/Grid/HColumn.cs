using System;
using System.Collections.Generic;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public class HColumn<T> : ExtensionContainer, IHtmlColumn
  {
    readonly Getter<IHtmlControl, T> cellGetter;
    public HColumn(string name, Getter<IHtmlControl, T> cellGetter) :
      base("HColumn", name)
    {
      this.cellGetter = cellGetter;
    }

    public IHtmlControl GetCell(object row)
    {
      return cellGetter((T)row);
    }
  }
}
