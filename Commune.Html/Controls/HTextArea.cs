using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HTextArea : ExtensionContainer, IHtmlControl
  {
    readonly string text;
    public HTextArea(string dataName, string text) :
      base("HTextArea", dataName)
    {
      this.text = text;
    }

    public HTextArea(string dataName) :
      this(dataName, "")
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.CssAttribute("resize", "none");

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      return h.TextArea(HtmlHlp.ContentForHElement(this, cssClassName,
        h.data("name", Name), h.data("id", Name), text)
      );
    }
  }
}
