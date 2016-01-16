using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HLabel : ExtensionContainer, IHtmlControl
  {
    readonly string caption;
    public HLabel(string caption) :
      base("HLabel", "")
    {
      this.caption = caption;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.VAlign(null);
      defaults.Padding("4px 8px");
      defaults.Display("inline-block");

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, caption));
    }
  }
}
