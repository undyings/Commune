using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Commune.Basis;
using System.Globalization;

namespace Commune.Html
{
  public static class CssExt
  {
    public static T Margin<T>(this T control, string margin) where T : IEditExtension
    {
      return CssAttribute(control, "margin", margin);
    }

    public static T Margin<T>(this T control, int margin) where T : IEditExtension
    {
      return Margin(control, string.Format("{0}px", margin));
    }

    public static T Margin<T>(this T control, int vertMargin, int horMargin) where T : IEditExtension
    {
      return Margin(control, string.Format("{0}px {1}px", vertMargin, horMargin));
    }

    public static T Margin<T>(this T control, 
      int topMargin, int rightMargin, int bottomMargin, int leftMargin) where T : IEditExtension
    {
      return Margin(control, string.Format("{0}px {1}px {2}px {3}px", 
        topMargin, rightMargin, bottomMargin, leftMargin));
    }

    public static T MarginLeft<T>(this T control, string marginLeft) where T : IEditExtension
    {
      return CssAttribute(control, "margin-left", marginLeft);
    }

    public static T MarginLeft<T>(this T control, int marginLeft) where T : IEditExtension
    {
      return MarginLeft(control, string.Format("{0}px", marginLeft));
    }

    public static T MarginRight<T>(this T control, string marginRight) where T : IEditExtension
    {
      return CssAttribute(control, "margin-right", marginRight);
    }

    public static T MarginRight<T>(this T control, int marginRight) where T : IEditExtension
    {
      return MarginRight(control, string.Format("{0}px", marginRight));
    }

    public static T Padding<T>(this T control, string padding) where T : IEditExtension
    {
      return CssAttribute(control, "padding", padding);
    }

    public static T Padding<T>(this T control, int padding) where T : IEditExtension
    {
      return Padding(control, string.Format("{0}px", padding));
    }

    public static T Padding<T>(this T control, int vertPadding, int horPadding) where T : IEditExtension
    {
      return Padding(control, string.Format("{0}px {1}px", vertPadding, horPadding));
    }

    public static T Padding<T>(this T control,
      int topPadding, int rightPadding, int bottomPadding, int leftPadding) where T : IEditExtension
    {
      return Padding(control, string.Format("{0}px {1}px {2}px {3}px",
        topPadding, rightPadding, bottomPadding, leftPadding));
    }

    public static T Border<T>(this T control,
      string width, string style, string color, string radius) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(radius))
        CssAttribute(control, "border-radius", radius);
      return CssAttribute(control, "border",
        string.Format("{0} {1} {2}", width, style, color)
      );
    }

    public static T Border<T>(this T control,
      string width, string style, Color color, string radius) where T : IEditExtension
    {
      return Border(control, width, style, HtmlHlp.ColorToHtml(color), radius);
    }


    public static T NoWrap<T>(this T control) where T : IEditExtension
    {
      return CssAttribute(control, "white-space", "nowrap");
    }

    public static T CssAttribute<T>(this T control, string extensionName, object extensionValue) where T : IEditExtension
    {
      control.WithExtension(new CssExtensionAttribute(extensionName, extensionValue));
      return control;
    }

    public static T FontSize<T>(this T control, string fontSize) where T : IEditExtension
    {
      return CssAttribute(control, "font-size", fontSize);
    }

    public static T Color<T>(this T control, Color color) where T : IEditExtension
    {
      return CssAttribute(control, "color", HtmlHlp.ColorToHtml(color));
    }

    public static T Color<T>(this T control, string color) where T : IEditExtension
    {
      return CssAttribute(control, "color", color);
    }

    public static T Background<T>(this T control, Color color) where T : IEditExtension
    {
      return CssAttribute(control, "background", HtmlHlp.ColorToHtml(color));
    }

    public static T Background<T>(this T control, string color) where T : IEditExtension
    {
      return CssAttribute(control, "background", color);
    }

    public static T Background<T>(this T control, string color,
      string imagePath, string repeat, string position) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(imagePath))
        CssAttribute(control, "background-image", string.Format("url({0})", imagePath));
      if (!StringHlp.IsEmpty(repeat))
        CssAttribute(control, "background-repeat", repeat);
      if (!StringHlp.IsEmpty(position))
        CssAttribute(control, "backround-position", position);
      return Background(control, color);
    }

    public static T Overflow<T>(this T control, string overflow) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(overflow))
        CssAttribute(control, "overflow", overflow);
      return control;
    }

    public static T Overflow<T>(this T control, string overflowX, string overflowY) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(overflowX))
        CssAttribute(control, "overflow-x", overflowX);
      if (!StringHlp.IsEmpty(overflowY))
        CssAttribute(control, "overflow-y", overflowY);
      return control;
    }

    public static T Overflow<T>(this T control) where T : IEditExtension
    {
      CssAttribute(control, "overflow", "hidden");
      CssAttribute(control, "text-overflow", "ellipsis");
      CssAttribute(control, "white-space", "nowrap");
      return control;
    }

    public static T Size<T>(this T control, int width, int height) where T : IEditExtension
    {
      Width(control, string.Format("{0}px", width));
      Height(control, string.Format("{0}px", height));
      return control;
    }

    public static T Width<T>(this T control, string width) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(width))
        CssAttribute(control, "width", width);
      return control;
    }

    public static T WidthLimit<T>(this T control, string minWidth, string maxWidth) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(minWidth))
        CssAttribute(control, "min-width", minWidth);
      if (!StringHlp.IsEmpty(maxWidth))
        CssAttribute(control, "max-width", maxWidth);
      return control;
    }

    public static T WidthFill<T>(this T control, int minWidth, int maxWidth) where T : IEditExtension
    {
      Width(control, string.Format("{0}px", maxWidth));
      WidthLimit(control, string.Format("{0}px", minWidth), string.Format("{0}px", maxWidth));
      return control;
    }

    public static T WidthFill<T>(this T control) where T : IEditExtension
    {
      Width(control, "2000px");
      CssAttribute(control, "max-width", "2000px");
      return control;
    }

    public static T Height<T>(this T control, string height) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(height))
        CssAttribute(control, "height", height);
      return control;
    }

    public static T HeightLimit<T>(this T control, string minHeight, string maxHeight) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(minHeight))
        CssAttribute(control, "min-height", minHeight);
      if (!StringHlp.IsEmpty(maxHeight))
        CssAttribute(control, "max-height", maxHeight);
      return control;
    }

    public static T Align<T>(this T control, bool? isLeft) where T : IEditExtension
    {
      string align = "center";
      if (isLeft == true)
        align = "left";
      else if (isLeft == false)
        align = "right";
      return CssAttribute(control, "text-align", align);
    }

    public static T Align<T>(this T control, bool? isLeft, bool? isTop) where T : IEditExtension
    {
      VAlign(control, isTop);
      return Align(control, isLeft);
    }

    public static T VAlign<T>(this T control, bool? isTop) where T : IEditExtension
    {
      string vAlign = "middle";
      if (isTop == true)
        vAlign = "top";
      else if (isTop == false)
        vAlign = "bottom";
      return CssAttribute(control, "vertical-align", vAlign);
    }

    public static T VAlign<T>(this T control, int vAlignInPixel) where T : IEditExtension
    {
      return CssAttribute(control, "vertical-align", string.Format("{0}px", vAlignInPixel));
    }

    public static T Display<T>(this T control, string display) where T : IEditExtension
    {
      return CssAttribute(control, "display", display);
    }

    public static T LinearGradient<T>(this T control, string direction, string beginColor, string endColor)
      where T : IEditExtension
    {
      CssAttribute(control, "background-image", string.Format("-webkit-linear-gradient({0},{1},{2})",
        direction, beginColor, endColor));
      return CssAttribute(control, "background-image", string.Format("linear-gradient({0},{1},{2})",
        direction, beginColor, endColor)
      );
    }

    public static T Cursor<T>(this T control, string cursor) where T : IEditExtension
    {
      return CssAttribute(control, "cursor", cursor);
    }

    public static T BoxSizing<T>(this T control, string boxSizing) where T : IEditExtension
    {
      return CssAttribute(control, "box-sizing", boxSizing);
    }

    public static T BoxSizing<T>(this T control) where T : IEditExtension
    {
      return BoxSizing(control, "border-box");
    }

    public static T FontWeight<T>(this T control, string fontWeight) where T : IEditExtension
    {
      return CssAttribute(control, "font-weight", fontWeight);
    }

    public static T FontBold<T>(this T control, bool isBold) where T : IEditExtension
    {
      return FontWeight(control, isBold ? "bold" : "normal");
    }

    public static T FontFamily<T>(this T control, string fontFamily) where T : IEditExtension
    {
      return CssAttribute(control, "font-family", string.Format("'{0}'", fontFamily));
    }

    public static T Content<T>(this T control, string content) where T : IEditExtension
    {
      return CssAttribute(control, "content", string.Format("'{0}'", content));
    }

    public static T Left<T>(this T control, string left) where T : IEditExtension
    {
      return CssAttribute(control, "left", left);
    }

    public static T Right<T>(this T control, string right) where T : IEditExtension
    {
      return CssAttribute(control, "right", right);
    }

    public static T Top<T>(this T control, string top) where T : IEditExtension
    {
      return CssAttribute(control, "top", top);
    }

    public static T Bottom<T>(this T control, string bottom) where T : IEditExtension
    {
      return CssAttribute(control, "bottom", bottom);
    }

    public static T Float<T>(this T control, string floats) where T : IEditExtension
    {
      return CssAttribute(control, "float", floats);
    }

    public static T FloatRight<T>(this T control) where T : IEditExtension
    {
      return Float(control, "right");
    }

    public static T Delay<T>(this T control, int delayInMilliseconds) where T : IEditExtension
    {
      return CssAttribute(control, "transition-delay", string.Format("{0}ms", delayInMilliseconds));
    }

  }
}
