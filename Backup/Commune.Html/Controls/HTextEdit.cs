using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HTextEdit : ExtensionContainer, IHtmlControl
  {
    readonly string value;
    public HTextEdit(string dataName, string value) :
      base("HTextEdit", dataName)
    {
      this.value = value;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName, 
        h.type("text"), h.data("name", Name), h.value(value))
      );
    }

    public IHtmlControl[] Controls
    {
      get { return Array<IHtmlControl>.Empty; }
    }
  }
}
