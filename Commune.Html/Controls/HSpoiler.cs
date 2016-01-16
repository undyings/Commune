using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HSpoiler : ExtensionContainer, IHtmlControl
  {
    readonly string spoilerCaption;
    readonly IHtmlControl spoilerText;
    readonly HStyle[] pseudoClasses;
    public HSpoiler(string spoilerCaption, IHtmlControl spoilerText, params HStyle[] pseudoClasses) :
      base("HSpoiler", "")
    {
      this.spoilerCaption = spoilerCaption;
      this.spoilerText = spoilerText;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string buttonCssName = string.Format("{0}_btn", cssClassName);
      string textCssName = string.Format("{0}_text", cssClassName);

      HtmlHlp.AddClassToCss(css, buttonCssName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ", cssClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ~ .{1} ", cssClassName, textCssName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked ~ .{1} ", cssClassName, textCssName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "block") }
      );

      return h.Div(h.@class(cssClassName),
        new HElement("label",
          h.Input(h.type("checkbox")),
          h.Span(h.@class(buttonCssName), spoilerCaption),
          spoilerText.ToHtml(textCssName, css)
        )
      );
    }
  }
}
