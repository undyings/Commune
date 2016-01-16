using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HCheckEdit : ExtensionContainer, IHtmlControl
  {
    readonly bool value;
    public HCheckEdit(string dataName, bool value) :
      base("HCheckEdit", dataName)
    {
      this.value = value;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      List<object> elements = new List<object>();
      elements.Add(h.type("checkbox"));
      elements.Add(h.data("name", Name));
      if (value)
        elements.Add(h.@checked());

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }

    public IHtmlControl[] Controls
    {
      get { return Array<IHtmlControl>.Empty; }
    }
  }
}
