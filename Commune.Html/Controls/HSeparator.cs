using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Html
{
  public class HSeparator : ExtensionContainer, IHtmlControl
  {
    public HSeparator() :
      base("HSeparator", "")
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName));
    }
  }
}
