using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HCheckButton : ExtensionContainer, IHtmlControl
  {
    readonly string dataName;
    readonly IHtmlControl vacant;
    readonly IHtmlControl selected;
    readonly string onClick;
    readonly HStyle[] pseudoClasses;
    public HCheckButton(string dataName, IHtmlControl vacant, IHtmlControl selected, string onClick,
      params HStyle[] pseudoClasses) :
      base("HCheckButton", dataName)
    {
      this.dataName = dataName;
      this.vacant = vacant;
      this.selected = selected;
      this.onClick = onClick;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string vacantClassName = string.Format("{0}_vacant", cssClassName);
      string selectedClassName = string.Format("{0}_selected", cssClassName);
      string checkClassName = string.Format("{0}_check", cssClassName);

      {
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
        defaults.InlineBlock();
        defaults.Cursor(CursorStyle.Pointer);
        defaults.OnClick(string.Format(
          "$('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);{1}",
          checkClassName, onClick)
        );
      }

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HTone checkStyle = new HTone().Display("none");
      HtmlHlp.AddClassToCss(css, checkClassName, checkStyle.CssExtensions);

      HElement vacantElement = vacant.ToHtml(vacantClassName, css);
      HElement selectedElement = selected.ToHtml(selectedClassName, css);


      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ~ .{1} ",
        cssClassName, selectedClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked ~ .{1} ",
        cssClassName, selectedClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked ~ .{1} ",
        cssClassName, vacantClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ~ .{1} ",
        cssClassName, vacantClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") }
      );

      List<object> checkElements = new List<object>();
      checkElements.Add(h.@class(checkClassName));
      checkElements.Add(h.type("checkbox"));
      checkElements.Add(h.data("name", Name));
      checkElements.Add(h.data("id", checkClassName));
      
      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName,
        h.Input(checkElements.ToArray()),
        vacantElement,
        selectedElement
      ));
    }
  }
}
