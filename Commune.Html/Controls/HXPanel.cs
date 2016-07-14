using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HXPanel : ExtensionContainer, IHtmlControl
  {
    public readonly IHtmlControl[] controls;
    public HXPanel(string name, params IHtmlControl[] controls) :
      base("HXPanel", name)
    {
      this.controls = controls;
    }

    public HXPanel(params IHtmlControl[] controls) :
      this("", controls)
    {
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

      List<object> elements = new List<object>();
 
      string container = GetExtended("container") as string;
      if (!StringHlp.IsEmpty(container))
        elements.Add(h.data("name", container));

      int index = -1;
      foreach (IHtmlControl control in controls)
      {
        index++;

        if (control is IEditExtension)
        {
          DefaultExtensionContainer defaults = new DefaultExtensionContainer((IEditExtension)control);
          defaults.Display("table-cell");
          defaults.VAlign(true);
        }

        string childCssClassName = control.Name;
        if (StringHlp.IsEmpty(childCssClassName))
          childCssClassName = string.Format("{0}_{1}", cssClassName, index + 1);

        HElement element = control.ToHtml(childCssClassName, css);

        bool isHide = (control.GetExtended("hide") as bool?) ?? false;
        if (!isHide)
          elements.Add(element);
      }

      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray()));
    }

    //public HElement ToHtml(string cssClassName, StringBuilder css)
    //{
    //  HtmlHlp.AddCssClass(css, cssClassName, CssExtensions);
    //  HtmlHlp.AddHoverToCss(css, this, cssClassName);

    //  List<HElement> tableElements = new List<HElement>();

    //  // Создаем css класс для вложенной таблицы
    //  string innerCssClassName = string.Format("{0}_inner", cssClassName);
    //  {
    //    HtmlHlp.AddCssClass(css, innerCssClassName, new CssExtensionAttribute[] {
    //      new CssExtensionAttribute("width", "100%"), new CssExtensionAttribute("height", "100%")
    //    });
    //  }
      
    //  int index = -1;
    //  foreach (IHtmlControl control in controls)
    //  {
    //    index++;
    //    bool? isHide = control.GetExtended("hide") as bool?;
    //    if (isHide == true)
    //      continue;

    //    string childCssClassName = control.Name;
    //    if (StringHlp.IsEmpty(childCssClassName))
    //      childCssClassName = string.Format("{0}_{1}", cssClassName, index + 1);

    //    HElement element = control.ToHtml(childCssClassName, css);
    //    tableElements.Add(h.Td(element));
    //  }

    //  return h.Div(h.@class(cssClassName), 
    //    h.Table(h.@class(innerCssClassName),
    //      h.TBody(
    //        h.Tr(tableElements.ToArray()
    //        )
    //      )
    //    )
    //  );
    //}

  }
}
