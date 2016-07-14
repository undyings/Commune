using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HPanel : ExtensionContainer, IHtmlControl
  {
    public readonly IHtmlControl[] controls;
    public HPanel(string name, params IHtmlControl[] controls) :
      base("HPanel", name)
    {
      this.controls = controls;
    }

    public HPanel(params IHtmlControl[] controls) :
      this("", controls)
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      List<object> elements = new List<object>();

      string container = GetExtended("container") as string;
      if (!StringHlp.IsEmpty(container))
        elements.Add(h.data("name", container));

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

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray()));
    }
  }
}
