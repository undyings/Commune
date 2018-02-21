using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HAnchor : ExtensionContainer, IHtmlControl
  {
    readonly string anchor;
    public HAnchor(string anchor) :
      base("HAnchor", "")
    {
      this.anchor = anchor;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      return h.A(
        new HAttribute("name", anchor)
      );
    }
  }
}
