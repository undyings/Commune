using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HElementControl : ExtensionContainer, IHtmlControl
  {
    readonly HElement element;
    public HElementControl(HElement element, string cssClassName) :
      base("HElementControl", cssClassName)
    {
      this.element = element;
    }

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      if (!StringHlp.IsEmpty(Name))
      {
        HtmlHlp.AddClassToCss(css, Name, CssExtensions);
      }

      return element;
    }
  }
}
