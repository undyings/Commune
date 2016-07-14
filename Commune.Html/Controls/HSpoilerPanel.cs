using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HSpoilerPanel : ExtensionContainer, IHtmlControl
  {
    readonly IHtmlControl openingIcon;
    readonly IHtmlControl closingIcon;
    readonly bool isAfterIcon;
    readonly IHtmlControl captionControl;
    readonly IHtmlControl blockControl;
    readonly HStyle[] pseudoClasses;
    public HSpoilerPanel(IHtmlControl openingIcon, IHtmlControl closingIcon, bool isAfterIcon,
      IHtmlControl captionControl, IHtmlControl blockControl,
      params HStyle[] pseudoClasses) :
      base("HSpoiler", "")
    {
      this.openingIcon = openingIcon;
      this.closingIcon = closingIcon;
      this.isAfterIcon = isAfterIcon;
      this.captionControl = captionControl;
      this.blockControl = blockControl;
      this.pseudoClasses = pseudoClasses;
    }

    public HSpoilerPanel(IHtmlControl captionControl, IHtmlControl blockControl,
      params HStyle[] pseudoClasses) :
        this(null, null, false, captionControl, blockControl, pseudoClasses)
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string openingIconCssName = string.Format("{0}_open", cssClassName);
      string closingIconCssName = string.Format("{0}_close", cssClassName);
      string captionCssName = string.Format("{0}_caption", cssClassName);
      string blockCssName = string.Format("{0}_block", cssClassName);
      string checkCssName = string.Format("{0}_check", cssClassName);

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      List<object> elements = new List<object>();

      elements.Add(
        h.onclick(
          string.Format(
            "$('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);",
            checkCssName
          )
        )
      );

      if (openingIcon != null)
      {
        elements.Add(openingIcon.ToHtml(openingIconCssName, css));

        HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] + div .{1} ",
          cssClassName, openingIconCssName),
          new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") }
        );

        HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked + div .{1} ",
          cssClassName, openingIconCssName),
          new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
        );
      }
      if (closingIcon != null)
      {
        elements.Add(closingIcon.ToHtml(closingIconCssName, css));

        HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] + div .{1} ",
          cssClassName, closingIconCssName),
          new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
        );

        HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked + div .{1} ",
          cssClassName, closingIconCssName),
          new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") }
        );
      }

      HElement captionElement = captionControl.ToHtml(captionCssName, css);
      if (!isAfterIcon)
        elements.Add(captionElement);
      else
        elements.Insert(0, captionElement);

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ", cssClassName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ~ .{1} ",
        cssClassName, blockCssName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
      );

      HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked ~ .{1} ",
        cssClassName, blockCssName),
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "block") }
      );

      return h.Div(h.@class(cssClassName),
        new HElement("label",
          h.Input(h.type("checkbox")),
          h.Div(elements.ToArray())
        ),
        h.Input(h.type("checkbox"), h.@class(checkCssName)),
        blockControl.ToHtml(blockCssName, css)
      );
    }
  }
}
