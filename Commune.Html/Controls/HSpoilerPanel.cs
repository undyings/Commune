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
    readonly bool isOpen;
    readonly HStyle[] pseudoClasses;
    public HSpoilerPanel(string checkDataName, IHtmlControl openingIcon, IHtmlControl closingIcon, 
      bool isAfterIcon, IHtmlControl captionControl, IHtmlControl blockControl, bool isOpen,
      params HStyle[] pseudoClasses) :
      base("HSpoiler", checkDataName)
    {
      this.openingIcon = openingIcon;
      this.closingIcon = closingIcon;
      this.isAfterIcon = isAfterIcon;
      this.captionControl = captionControl;
      this.blockControl = blockControl;
      this.isOpen = isOpen;
      this.pseudoClasses = pseudoClasses;
    }

    public HSpoilerPanel(string checkDataName, IHtmlControl openingControl, IHtmlControl closingControl,
      IHtmlControl blockControl, bool isOpen, params HStyle[] pseudoClasses) :
      this(checkDataName, openingControl, closingControl, false, new HPanel(), blockControl, isOpen, pseudoClasses)
    {
    }

    public HSpoilerPanel(IHtmlControl captionControl, IHtmlControl blockControl,
      params HStyle[] pseudoClasses) :
        this(null, null, null, false, captionControl, blockControl, false, pseudoClasses)
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
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(openingIcon);
        defaults.Cursor(CursorStyle.Pointer);

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
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(closingIcon);
        defaults.Cursor(CursorStyle.Pointer);

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

      List<object> checkElements = new List<object>();
      checkElements.Add(h.type("checkbox"));
      if (isOpen)
        checkElements.Add(h.@checked());

      if (!StringHlp.IsEmpty(Name))
      {
        checkElements.Add(h.data("name", Name));
        checkElements.Add(h.data("id", Name));
      }
      HElement innerCheck = h.Input(checkElements.ToArray());

      checkElements.Add(h.@class(checkCssName));
      HElement spoilerCheck = h.Input(checkElements.ToArray());

      return h.Div(h.@class(cssClassName),
        new HElement("label",
          innerCheck,
          h.Div(elements.ToArray())
        ),
        spoilerCheck,
        blockControl.ToHtml(blockCssName, css)
      );
    }
  }
}
