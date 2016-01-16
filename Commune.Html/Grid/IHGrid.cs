using System;
using System.Collections.Generic;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public interface IHGrid : IHtmlControl, IEditExtension
  {
  }

  public interface IHtmlColumn
  {
    string Name { get; }
    IHtmlControl GetCell(object row);
    //IEnumerable<CssExtensionAttribute> CssExtensions { get; }
  }
}
