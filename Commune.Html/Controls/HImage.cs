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

  public class HEventImage : ExtensionContainer, IHtmlControl, IEventEditExtension
  {
    readonly string url;
    readonly HStyle[] pseudoClasses;
    public HEventImage(string url, params HStyle[] pseudoClasses) :
      base("HImage", "")
    {
      this.url = url;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.OnClick(";");

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      List<object> elements = new List<object>();
      {
        elements.Add(h.src(url));

        hevent onevent = GetExtended("onevent") as hevent;
        if (onevent != null)
          elements.Add(onevent);
      }

      return h.Img(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }
  }
}
