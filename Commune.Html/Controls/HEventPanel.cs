using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HEventPanel : ExtensionContainer, IHtmlControl, IEventEditExtension
  {
    public readonly IHtmlControl[] controls;
    readonly HStyle[] pseudoClasses;
    public HEventPanel(string name, params IHtmlControl[] controls) :
      this(name, controls, new HStyle[0])
    {
      this.controls = controls;
    }

    public HEventPanel(string name, IHtmlControl[] controls, params HStyle[] pseudoClasses) :
      base("HPanel", name)
    {
      this.controls = controls;
      this.pseudoClasses = pseudoClasses;
    }

    public HEventPanel(params IHtmlControl[] controls) :
      this("", controls)
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      //defaults.OnClick(";");

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      List<object> elements = new List<object>();

      string container = GetExtended("container") as string;
      if (!StringHlp.IsEmpty(container))
      {
        elements.Add(h.data("name", container));
        elements.Add(h.data("id", container));
      }

      int index = -1;
      foreach (IHtmlControl control in controls)
      {
        if (control == null)
          continue;

        index++;

        string childCssClassName = control.Name;
        if (StringHlp.IsEmpty(childCssClassName))
          childCssClassName = string.Format("{0}_{1}", cssClassName, index + 1);

        HElement element = control.ToHtml(childCssClassName, css);

        bool isHide = (control.GetExtended("hide") as bool?) ?? false;
        if (!isHide)
          elements.Add(element);
      }

      hevent onevent = GetExtended("onevent") as hevent;
      if (onevent != null)
        elements.Add(onevent);

      return new HEventElement("div", HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray()));
    }
  }
}
