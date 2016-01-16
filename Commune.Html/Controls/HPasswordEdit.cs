using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HPasswordEdit : ExtensionContainer, IHtmlControl
  {
    readonly string value;
    public HPasswordEdit(string dataName, string value) :
      base("HTextEdit", dataName)
    {
      this.value = value;
    }

    public HPasswordEdit(string dataName) :
      this(dataName, "")
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName,
        h.type("password"), h.data("name", Name), h.value(value))
      );
    }
  }
}
