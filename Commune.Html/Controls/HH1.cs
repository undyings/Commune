using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public abstract class HH : ExtensionContainer, IHtmlControl
  {
    readonly string elementType;
    readonly string caption;
    readonly HStyle[] pseudoClasses;
    public HH(string elementType, object value, params HStyle[] pseudoClasses) :
      base(elementType, "")
    {
      this.elementType = elementType;
      this.caption = value != null ? value.ToString() : "";
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.Display("inline-block");
      defaults.Margin(0);

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      return new HElement(elementType, HtmlHlp.ContentForHElement(this, cssClassName, caption));
    }
  }

  public class HH1 : HH
  {
    public HH1(object value, params HStyle[] pseudoClasses) :
      base("h1", value, pseudoClasses)
    {
    }
  }

  public class HH2 : HH
  {
    public HH2(object value, params HStyle[] pseudoClasses) :
      base("h2", value, pseudoClasses)
    {
    }
  }

  public class HH3 : HH
  {
    public HH3(object value, params HStyle[] pseudoClasses) :
      base("h3", value, pseudoClasses)
    {
    }
  }

  public class HH4 : HH
  {
    public HH4(object value, params HStyle[] pseudoClasses) :
      base("h4", value, pseudoClasses)
    {
    }
  }

  public class HH5 : HH
  {
    public HH5(object value, params HStyle[] pseudoClasses) :
      base("h5", value, pseudoClasses)
    {
    }
  }
}
