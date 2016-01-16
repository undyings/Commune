﻿using System;
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
    readonly HStyle[] pseudoClasses;
    public HLabel(string caption, params HStyle[] pseudoClasses) :
      base("HLabel", "")
    {
      this.caption = caption;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.Display("inline-block");

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, caption));
    }
  }
}