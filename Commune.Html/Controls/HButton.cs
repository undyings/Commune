using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Drawing;

namespace Commune.Html
{
  public class HButton : ExtensionContainer, IHtmlControl, IEventEditExtension
  {
    readonly string caption;
    readonly HStyle[] pseudoClasses;
    public HButton(string caption, params HStyle[] pseudoClasses) :
      this("", caption, pseudoClasses)
    {
    }

    public HButton(string name, string caption, params HStyle[] pseudoClasses) :
      base("HButton", name)
    {
      this.caption = caption;
      this.pseudoClasses = pseudoClasses;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      {
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
        defaults.Display("inline-block");
        defaults.Cursor(CursorStyle.Pointer);
        CssExt.CssAttribute(defaults, "white-space", "nowrap");
        defaults.OnClick(";");
      }

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      foreach (HStyle pseudo in pseudoClasses)
        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);
       
      List<object> elements = new List<object>(3);
      {
        elements.Add(h.Span(caption));

        hevent onevent = GetExtended("onevent") as hevent;
        if (onevent != null)
          elements.Add(onevent);
      }

      return new HEventElement("div", HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      );

      //return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
      //);
    }
  }

  //public class HButton : ExtensionContainer, IHtmlControl
  //{
  //  readonly string caption;
  //  public HButton(string caption) :
  //    base("HButton", "")
  //  {
  //    this.caption = caption;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    {
  //      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
  //      //defaults.Align(null, null);
  //      defaults.Padding("6px 12px");
  //      defaults.Display("inline-block");
  //      defaults.Border("1px", "solid", Color.FromArgb(187, 187, 187), "2px");
  //      defaults.LinearGradient("to top right", Color.FromArgb(221, 221, 221), Color.FromArgb(241, 241, 241));
  //      defaults.Cursor(CursorStyle.Pointer);
  //      CssExt.CssAttribute(defaults, "white-space", "nowrap");

  //      defaults.OnClick(";");
  //    }

  //    {
  //      HStyle hover = GetExtended("hover") as HStyle;
  //      if (hover == null)
  //      {
  //        hover = new HStyle(".{0}:hover");
  //        this.Hover(hover);
  //      }

  //      DefaultExtensionContainer defaults = new DefaultExtensionContainer(hover);
  //      //defaults.Border("1px", "solid", Color.FromArgb(60, 127, 177), "2px");
  //      //defaults.LinearGradient("to top right", Color.FromArgb(167, 217, 177), Color.FromArgb(232, 246, 253));
  //      defaults.Border("1px", "solid", Color.FromArgb(170, 170, 170), "2px");
  //      defaults.LinearGradient("to top right", Color.FromArgb(204, 204, 204), Color.FromArgb(234, 234, 234));
  //    }

  //    {
  //      HStyle active = GetExtended("active") as HStyle;
  //      if (active == null)
  //      {
  //        active = new HStyle(".{0}:active");
  //        this.WithExtension(new ExtensionAttribute("active", active));
  //      }

  //      DefaultExtensionContainer defaults = new DefaultExtensionContainer(active);
  //      defaults.Border("2px", "double", Color.FromArgb(44, 98, 139), "2px");
  //      defaults.Padding("5px", "12px", "5px", "10px");
  //      defaults.LinearGradient("to top right", Color.FromArgb(104, 179, 219), Color.FromArgb(229, 244, 252));
  //    }


  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
 
  //    List<object> elements = new List<object>();
  //    {
  //      HImage image = GetExtended("innerImage") as HImage;
  //      if (image != null)
  //      {
  //        DefaultExtensionContainer defaults = new DefaultExtensionContainer(image);
  //        defaults.VAlign(null);
  //        defaults.Margin("0px", "4px", "8px", "0px");
  //        defaults.Display("inline");
  //        //defaults.Align(true);

  //        elements.Add(image.ToHtml(string.Format("{0}_image", cssClassName), css));

  //        elements.Add(h.Span(caption));

  //        //string captionClassName = string.Format("{0}_caption", cssClassName);
  //        //elements.Add(h.Span(caption, h.@class(captionClassName))
  //        //);
  //        //HtmlHlp.AddClassToCss(css, captionClassName,
  //        //  new CssExtensionAttribute[] { new CssExtensionAttribute("display", "table-cell") });
  //      }
  //      else
  //        elements.Add(caption);

  //      hevent onevent = GetExtended("onevent") as hevent;
  //      if (onevent != null)
  //        elements.Add(onevent);
  //    }

  //    return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
  //    );
  //  }
  //}
}
