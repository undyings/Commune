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
    readonly HStyle[] pseudoClasses;
    public HSpan(string caption, params HStyle[] pseudoClasses) :
      base("HSpan", "")
    {
      this.caption = caption;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      return h.Span(HtmlHlp.ContentForHElement(this, cssClassName, caption));
    }
  }
}
