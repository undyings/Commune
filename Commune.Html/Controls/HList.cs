using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HList : ExtensionContainer, IHtmlControl
  {
    readonly object[] items;
    readonly HTone itemStyle;
    readonly HStyle[] pseudoClasses;
    public HList(object[] items, HTone itemStyle, params HStyle[] pseudoClasses) :
      base("HList", "")
    {
      this.items = items;
      this.itemStyle = itemStyle;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      if (itemStyle != null)
      {
        HtmlHlp.AddExtensionsToCss(css, itemStyle.CssExtensions, ".{0} > li", cssClassName);
      }

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

      HElement[] liElements = new HElement[items.Length];
      for (int i = 0; i < items.Length; ++i)
      {
        object item = items[i];
        object element = item;
        if (item is IHtmlControl)
        {
          string itemClassName = string.Format("{0}_{1}", cssClassName, i + 1);
          element =  ((IHtmlControl)item).ToHtml(itemClassName, css);
        }
        liElements[i] = h.Li(element);
      };
      return h.Ul(HtmlHlp.ContentForHElement(this, cssClassName, liElements));
    }
  }
}
