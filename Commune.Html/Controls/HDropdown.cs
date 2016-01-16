using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Drawing;

namespace Commune.Html
{
  public class HHoverDropdown : ExtensionContainer, IHtmlControl
  {
    readonly HDropStyle dropStyle;
    readonly IHtmlControl rootControl;
    readonly IHtmlControl[] listControls;
    public HHoverDropdown(HDropStyle dropStyle, 
      IHtmlControl rootControl, params IHtmlControl[] listControls) :
      base("HHoverDropdown", "")
    {
      this.dropStyle = dropStyle;
      this.rootControl = rootControl;
      this.listControls = listControls;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string rootCssName = string.Format("{0}_root", cssClassName);
      string dropListCssName = string.Format("{0}_list", cssClassName);

      this.CssAttribute("position", "relative");
      this.CssAttribute("display", "inline-block");
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      HTone dropListStyle = dropStyle.GetExtended<HTone>("dropListStyle") ?? new HTone();
      dropListStyle.CssAttribute("position", "absolute");
      dropListStyle.Display("none");
      HtmlHlp.AddClassToCss(css, dropListCssName, dropListStyle.CssExtensions);

      HTone hoverDropListStyle = new HTone().Display("inline-block");
      HtmlHlp.AddExtensionsToCss(css, hoverDropListStyle.CssExtensions,
        ".{0}:hover .{1}", cssClassName, dropListCssName);

      HTone rootWhenDropped = dropStyle.GetExtended<HTone>("rootWhenDroppedStyle");
      if (rootWhenDropped != null)
      {
        HtmlHlp.AddExtensionsToCss(css, rootWhenDropped.CssExtensions,
          ".{0}:hover", rootCssName);
      }

      string anyItemCssName = string.Format("{0}_any_item", cssClassName);
      HTone anyItemStyle = dropStyle.GetExtended<HTone>("anyItemStyle");
      if (anyItemStyle != null)
        HtmlHlp.AddClassToCss(css, anyItemCssName, anyItemStyle.CssExtensions);

      HTone selectedItemStyle = dropStyle.GetExtended<HTone>("selectedItemStyle");
      if (selectedItemStyle != null)
        HtmlHlp.AddExtensionsToCss(css, selectedItemStyle.CssExtensions, ".{0}:hover", anyItemCssName);

      string separatorCssName = string.Format("{0}_separator", cssClassName);
      HTone separatorStyle = dropStyle.GetExtended<HTone>("separatorStyle");
      if (separatorStyle != null)
        HtmlHlp.AddClassToCss(css, separatorCssName, separatorStyle.CssExtensions);

      List<object> content = new List<object>(listControls.Length + 1);
      content.Add(h.@class(dropListCssName));
      int index = 0;
      foreach (IHtmlControl child in listControls)
      {
        index++;
        ((IEditExtension)child).Display("block");
        ((IEditExtension)child).ExtraClassNames(child is HSeparator ? separatorCssName : anyItemCssName);
        content.Add(child.ToHtml(string.Format("{0}_{1}", cssClassName, index), css));
      }

      return h.Div(h.@class(cssClassName),
        rootControl.ToHtml(rootCssName, css),
        h.Div(content.ToArray())
      );


      //string blockCssName = string.Format("{0}_block", cssClassName);
      //string rootCssName = string.Format("{0}_root", cssClassName);

      //HtmlHlp.AddClassToCss(css, blockCssName,
      //  new CssExtensionAttribute[] { 
      //    new CssExtensionAttribute("position", "relative"),
      //    new CssExtensionAttribute("display", "inline-block")
      //  }
      //);

      //HStyle blockFocusStyle = new HStyle(string.Format(".{0}:focus", blockCssName)).
      //  CssAttribute("outline", "none").Background(Color.LightPink);
      //HtmlHlp.AddStyleToCss(css, "", blockFocusStyle);

      //this.CssAttribute("position", "absolute");
      //this.Display("none");

      //HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      //HStyle dropStyle = new HStyle(string.Format(".{0}:{1} .{2}",
      //  blockCssName, dropByHover ? "hover" : "focus", cssClassName))
      //  .Display("inline-block");
      //  //.CssAttribute("outline", "none");
      //HtmlHlp.AddStyleToCss(css, "", dropStyle);

      //List<object> content = new List<object>(listControls.Length + 1);
      //content.Add(h.@class(cssClassName, css));
      //int index = 0;
      //foreach (IHtmlControl child in listControls)
      //{
      //  index++;
      //  ((IEditExtension)child).Display("block");
      //  content.Add(child.ToHtml(string.Format("{0}_{1}", cssClassName, index), css));
      //}

      //return h.Div(h.@class(blockCssName), new HAttribute("tabindex", "1"),
      //  rootControl.ToHtml(rootCssName, css),
      //  h.Div(content.ToArray())
      //);
    }
  }

  public class HClickDropdown : ExtensionContainer, IHtmlControl
  {
    readonly HDropStyle dropStyle;
    readonly IHtmlControl rootControl;
    readonly IHtmlControl[] listControls;
    public HClickDropdown(HDropStyle dropStyle,
      IHtmlControl rootControl, params IHtmlControl[] listControls) :
      base("HClickDropdown", "")
    {
      this.dropStyle = dropStyle;
      this.rootControl = rootControl;
      this.listControls = listControls;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      string rootCssName = string.Format("{0}_root", cssClassName);
      string dropListCssName = string.Format("{0}_list", cssClassName);

      this.CssAttribute("position", "relative");
      this.CssAttribute("display", "inline-block");
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      HTone dropListStyle = dropStyle.GetExtended<HTone>("dropListStyle") ?? new HTone();
      dropListStyle.CssAttribute("position", "absolute");
      dropListStyle.Display("none");
      HtmlHlp.AddClassToCss(css, dropListCssName, dropListStyle.CssExtensions);

      HtmlHlp.AddExtensionsToCss(css,
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") },
        ".{0} input[type=checkbox]", cssClassName
      );

      HtmlHlp.AddExtensionsToCss(css,
        new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") },
        ".{0} input[type=checkbox]:checked ~ .{1}", cssClassName, dropListCssName
      );

      HTone rootWhenDropped = dropStyle.GetExtended<HTone>("rootWhenDroppedStyle");
      if (rootWhenDropped != null)
      {
        HtmlHlp.AddExtensionsToCss(css, rootWhenDropped.CssExtensions,
          ".{0} input[type=checkbox]:checked ~ .{1} ", cssClassName, rootCssName);
      }

      string anyItemCssName = string.Format("{0}_any_item", cssClassName);
      HTone anyItemStyle = dropStyle.GetExtended<HTone>("anyItemStyle");
      if (anyItemStyle != null)
        HtmlHlp.AddClassToCss(css, anyItemCssName, anyItemStyle.CssExtensions);

      HTone selectedItemStyle = dropStyle.GetExtended<HTone>("selectedItemStyle");
      if (selectedItemStyle != null)
        HtmlHlp.AddExtensionsToCss(css, selectedItemStyle.CssExtensions, ".{0}:hover", anyItemCssName);

      string checkBoxId = string.Format("{0}_check", cssClassName);

      rootControl.OnClick(string.Format(
        "$('#{0}').is(':checked') ? $('#{0}').prop('checked', false) : $('#{0}').prop('checked', true);",
        checkBoxId));

      List<object> content = new List<object>(listControls.Length + 1);
      content.Add(h.@class(dropListCssName));
      int index = 0;
      foreach (IHtmlControl child in listControls)
      {
        IEditExtension childItem = (IEditExtension)child;
        index++;
        childItem.Display("block");
        if (!(child is HSeparator) && (child.GetExtended("unselectable") as bool?) != true)
        {
          childItem.ExtraClassNames(anyItemCssName);
          childItem.OnClick(string.Format("$('#{0}').prop('checked', false);", checkBoxId));
        }
        content.Add(child.ToHtml(string.Format("{0}_{1}", cssClassName, index), css));
      }

      return h.Div(h.@class(cssClassName), new HAttribute("tabindex", "1"),
        h.onblur(string.Format("$('#{0}').prop('checked', false);", checkBoxId)),
        h.Input(h.type("checkbox"), new HAttribute("id", checkBoxId)),
        rootControl.ToHtml(rootCssName, css),
        h.Div(content.ToArray())
      );
    }
  }

  //public class HClickDropdown : ExtensionContainer, IHtmlControl
  //{
  //  readonly HDropStyle dropStyle;
  //  readonly IHtmlControl rootControl;
  //  readonly IHtmlControl[] listControls;
  //  public HClickDropdown(HDropStyle dropStyle,
  //    IHtmlControl rootControl, params IHtmlControl[] listControls) :
  //    base("HClickDropdown", "")
  //  {
  //    this.dropStyle = dropStyle;
  //    this.rootControl = rootControl;
  //    this.listControls = listControls;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    string rootCssName = string.Format("{0}_root", cssClassName);
  //    string dropListCssName = string.Format("{0}_list", cssClassName);

  //    this.CssAttribute("position", "relative");
  //    this.CssAttribute("display", "inline-block");
  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

  //    HTone dropListStyle = dropStyle.GetExtended<HTone>("dropListStyle") ?? new HTone();
  //    dropListStyle.CssAttribute("position", "absolute");
  //    dropListStyle.Display("none");
  //    HtmlHlp.AddClassToCss(css, dropListCssName, dropListStyle.CssExtensions);

  //    HtmlHlp.AddExtensionsToCss(css,
  //      new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") },
  //      ".{0} input[type=checkbox]", cssClassName
  //    );

  //    HtmlHlp.AddExtensionsToCss(css,
  //      new CssExtensionAttribute[] { new CssExtensionAttribute("display", "inline-block") },
  //      ".{0} input[type=checkbox]:checked ~ .{1}", cssClassName, dropListCssName
  //    );

  //    HTone rootWhenDropped = dropStyle.GetExtended<HTone>("rootWhenDroppedStyle");
  //    if (rootWhenDropped != null)
  //    {
  //      HtmlHlp.AddExtensionsToCss(css, rootWhenDropped.CssExtensions,
  //        ".{0} input[type=checkbox]:checked ~ .{1} ", cssClassName, rootCssName);
  //    }

  //    string anyItemCssName = string.Format("{0}_any_item", cssClassName);
  //    HTone anyItemStyle = dropStyle.GetExtended<HTone>("anyItemStyle");
  //    if (anyItemStyle != null)
  //      HtmlHlp.AddClassToCss(css, anyItemCssName, anyItemStyle.CssExtensions);

  //    HTone selectedItemStyle = dropStyle.GetExtended<HTone>("selectedItemStyle");
  //    if (selectedItemStyle != null)
  //      HtmlHlp.AddExtensionsToCss(css, selectedItemStyle.CssExtensions, ".{0}:hover", anyItemCssName);

  //    List<object> content = new List<object>(listControls.Length + 1);
  //    content.Add(h.@class(dropListCssName));
  //    int index = 0;
  //    foreach (IHtmlControl child in listControls)
  //    {
  //      index++;
  //      ((IEditExtension)child).Display("block");
  //      if (!(child is HSeparator) && (child.GetExtended("unselectable") as bool?) != true)
  //        ((IEditExtension)child).ExtraClassName(anyItemCssName);
  //      content.Add(child.ToHtml(string.Format("{0}_{1}", cssClassName, index), css));
  //    }

  //    string checkBoxId = string.Format("{0}_check", cssClassName);

  //    return h.Div(h.@class(cssClassName), new HAttribute("tabindex", "-1"),
  //      h.onblur(string.Format("$('#{0}').removeAttr('checked');", checkBoxId)),
  //      new HElement("label",
  //        h.Input(h.type("checkbox"), new HAttribute("id", checkBoxId)),
  //        rootControl.ToHtml(rootCssName, css),
  //        h.Div(content.ToArray())
  //      )
  //    );
  //  }
  //}

  public class HDropStyle : ToneContainer
  {
    public HDropStyle()
      : base()
    {
    }
  }

  public static class HDropStyleExt
  {
    public static HDropStyle DropList(this HDropStyle dropStyle, HTone dropListStyle)
    {
      dropStyle.WithExtension("dropListStyle", dropListStyle);
      return dropStyle;
    }

    public static HDropStyle RootWhenDropped(this HDropStyle dropStyle, HTone rootWhenDroppedStyle)
    {
      dropStyle.WithExtension("rootWhenDroppedStyle", rootWhenDroppedStyle);
      return dropStyle;
    }

    public static HDropStyle SelectedItem(this HDropStyle dropStyle, HTone selectedItemStyle)
    {
      dropStyle.WithExtension("selectedItemStyle", selectedItemStyle);
      return dropStyle;
    }

    public static HDropStyle AnyItem(this HDropStyle dropStyle, HTone anyItemStyle)
    {
      dropStyle.WithExtension("anyItemStyle", anyItemStyle);
      return dropStyle;
    }
  }
}
