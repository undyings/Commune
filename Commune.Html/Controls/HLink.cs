using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HLink : ExtensionContainer, IHtmlControl
  {
    readonly object linkObject;
    //readonly IHtmlControl innerControl;
    readonly string url;
    readonly HStyle[] pseudoClasses;
    public HLink(string url, string caption, params HStyle[] pseudoClasses) :
      base("HLink", "")
    {
      this.url = url;
      this.linkObject = caption;
      this.pseudoClasses = pseudoClasses;
    }

    public HLink(string url, IHtmlControl innerControl, params HStyle[] pseudoClasses) :
      base("HLink", "")
    {
      this.url = url;
      this.linkObject = innerControl;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      List<object> elements = new List<object>();
      elements.Add(h.href(url));

      if (linkObject is IHtmlControl)
      {
        HElement innerElement = ((IHtmlControl)linkObject).ToHtml(string.Format("{0}_inner", cssClassName), css);

        elements.Add(innerElement);
      }
      else
      {
        elements.Add(linkObject);
      }

      return h.A(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }
  }
}
