using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HInputButton : ExtensionContainer, IHtmlControl
  {
    readonly string caption;
    public HInputButton(string caption) :
      base("HInputButton", "")
    {
      this.caption = caption;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      List<object> elements = new List<object>();

      elements.Add(h.type("button"));
      elements.Add(h.value(caption));

      hevent onevent = GetExtended("onevent") as hevent;
      if (onevent != null)
        elements.Add(onevent);

      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.OnClick(";");
      defaults.Cursor(CursorStyle.Pointer);

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );
    }

    public IHtmlControl[] Controls
    {
      get { return Array<IHtmlControl>.Empty; }
    }
  }
}
