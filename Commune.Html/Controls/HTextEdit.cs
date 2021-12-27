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
		readonly HStyle[] pseudoClasses;
		public HTextEdit(string dataName, string value, params HStyle[] pseudoClasses) :
      base("HTextEdit", dataName)
    {
      this.value = value;
			this.pseudoClasses = pseudoClasses;
    }

    public HTextEdit(string dataName, params HStyle[] pseudoClasses) :
      this(dataName, "", pseudoClasses)
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

			foreach (HStyle pseudo in pseudoClasses)
				HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

			HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      return h.Input(HtmlHlp.ContentForHElement(this, cssClassName,
        h.type("text"), h.data("name", Name), h.value(value), h.data("id", Name))
      );
    }
  }
}
