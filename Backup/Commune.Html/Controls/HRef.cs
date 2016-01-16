using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HUrl : ExtensionContainer, IHtmlControl
  {
    readonly string caption;
    readonly string url;
    readonly IHtmlControl[] controls = null;
    public HUrl(string url, string caption) :
      base("HUrl", "")
    {
      this.url = url;
      this.caption = caption;
    }

    public HUrl(string url, params IHtmlControl[] controls) :
      base("HUrl", "")
    {
      this.url = url;
      this.controls = controls;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      List<object> elements = new List<object>();
      elements.Add(h.href(url));

      if (controls == null)
      {
        elements.Add(caption);
      }
      else
      {
        int index = -1;
        foreach (IHtmlControl control in controls)
        {
          index++;

          string childCssClassName = control.Name;
          if (StringHlp.IsEmpty(childCssClassName))
            childCssClassName = string.Format("{0}_{1}", cssClassName, index + 1);

          HElement element = control.ToHtml(childCssClassName, css);

          bool isHide = (control.GetExtended("hide") as bool?) ?? false;
          if (!isHide)
            elements.Add(element);
        }
      }

      return h.A(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }
  }
}
