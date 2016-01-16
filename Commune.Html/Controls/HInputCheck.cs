using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HInputCheck : ExtensionContainer, IHtmlControl
  {
    readonly bool value;
    readonly HStyle[] pseudoClasses;
    public HInputCheck(string dataName, bool value, params HStyle[] pseudoClasses) :
      base("HCheckEdit", dataName)
    {
      this.value = value;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      {
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
        defaults.Display("inline-block");
      }

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      List<object> elements = new List<object>();
      elements.Add(h.type("checkbox"));
      elements.Add(h.data("name", Name));
      elements.Add(h.data("id", Name));
      if (value)
        elements.Add(h.@checked());

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }
  }
}
