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
    public HImage(string url) :
      base("HImage", "")
    {
      this.url = url;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      return h.Img(HtmlHlp.ContentForHElement(this, cssClassName, h.src(url))
      );
    }

    public IHtmlControl[] Controls
    {
      get { return Array<IHtmlControl>.Empty; }
    }
  }
}
