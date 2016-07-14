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
    readonly IHtmlControl openingIcon;
    readonly IHtmlControl closingIcon;
    readonly bool isAfterIcon;
    readonly IHtmlControl captionControl;
    readonly IHtmlControl blockControl;
    readonly HStyle[] pseudoClasses;
    public HSpoiler(IHtmlControl openingIcon, IHtmlControl closingIcon, bool isAfterIcon,
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

    public HSpoiler(IHtmlControl captionControl, IHtmlControl blockControl,
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

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      List<HElement> elements = new List<HElement>();

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
          h.Div(elements.ToArray()),
          blockControl.ToHtml(blockCssName, css)
        )
      );
    }
  }

  //public class HSpoiler2 : ExtensionContainer, IHtmlControl
  //{
  //  readonly string spoilerCaption;
  //  readonly IHtmlControl spoilerText;
  //  readonly HStyle[] pseudoClasses;
  //  public HSpoiler2(string spoilerCaption, IHtmlControl spoilerText, params HStyle[] pseudoClasses) :
  //    base("HSpoiler", "")
  //  {
  //    this.spoilerCaption = spoilerCaption;
  //    this.spoilerText = spoilerText;
  //    this.pseudoClasses = pseudoClasses;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    string buttonCssName = string.Format("{0}_btn", cssClassName);
  //    string textCssName = string.Format("{0}_text", cssClassName);

  //    HtmlHlp.AddClassToCss(css, buttonCssName, CssExtensions);

  //    foreach (HStyle pseudo in pseudoClasses)
  //      HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

  //    HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ", cssClassName),
  //      new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
  //    );

  //    HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox] ~ .{1} ", cssClassName, textCssName),
  //      new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
  //    );

  //    HtmlHlp.AddClassToCss(css, string.Format("{0} input[type=checkbox]:checked ~ .{1} ", cssClassName, textCssName),
  //      new CssExtensionAttribute[] { new CssExtensionAttribute("display", "block") }
  //    );

  //    return h.Div(h.@class(cssClassName),
  //      new HElement("label",
  //        h.Input(h.type("checkbox")),
  //        h.Span(h.@class(buttonCssName), spoilerCaption),
  //        spoilerText.ToHtml(textCssName, css)
  //      )
  //    );
  //  }
  //}
}
