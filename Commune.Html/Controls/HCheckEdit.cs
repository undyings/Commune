using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Drawing;

namespace Commune.Html
{
  public class HCheckEdit : ExtensionContainer, IHtmlControl
  {
    readonly bool value;
    readonly HTone frameStyle;
    readonly HTone markStyle;
    readonly HStyle[] pseudoClasses;
    public HCheckEdit(string dataName, bool value, HTone frameStyle, HTone markStyle, params HStyle[] pseudoClasses) :
      base("HCheckEdit", dataName)
    {
      this.value = value;
      this.frameStyle = frameStyle;
      this.markStyle = markStyle;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string checkClassName = string.Format("{0}_check", Name);
      string frameClassName = string.Format("{0}_frame", Name);

      HTone innerStyle = new HTone().Display("none");
      HtmlHlp.AddClassToCss(css, checkClassName, innerStyle.CssExtensions);

      HtmlHlp.AddClassToCss(css, frameClassName, frameStyle.CssExtensions);

      HtmlHlp.AddExtensionsToCss(css,
        markStyle.CssExtensions,
        ".{0} input[type=checkbox]:checked ~ .{1}::before ",
        cssClassName, frameClassName
      );

      {
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
        defaults.Display("inline-block");
        defaults.OnClick(string.Format(
          "e.preventDefault(); $('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);",
          checkClassName)
        );
      }

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      List<object> checkElements = new List<object>();
      checkElements.Add(h.@class(checkClassName));
      checkElements.Add(h.type("checkbox"));
      checkElements.Add(h.data("name", Name));
      checkElements.Add(h.data("id", checkClassName));
      if (value)
        checkElements.Add(h.@checked());

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName,
        h.Input(checkElements.ToArray()),
        h.Div(h.@class(frameClassName))
      ));
    }
  }
}
