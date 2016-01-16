using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HSpan : ExtensionContainer, IHtmlControl
  {
    readonly string caption;
    public HSpan(string caption) :
      base("HSpan", "")
    {
      this.caption = caption;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      return h.Span(HtmlHlp.ContentForHElement(this, cssClassName, caption));
    }
  }
}
