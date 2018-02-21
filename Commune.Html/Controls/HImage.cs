using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HImage : ExtensionContainer, IHtmlControl
  {
    readonly string url;
    readonly HStyle[] pseudoClasses;
    public HImage(string url, params HStyle[] pseudoClasses) :
      base("HImage", "")
    {
      this.url = url;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      return h.Img(HtmlHlp.ContentForHElement(this, cssClassName, h.src(url))
      );
    }
  }
}
