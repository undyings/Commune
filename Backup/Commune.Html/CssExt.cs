using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Commune.Basis;

namespace Commune.Html
{
  public static class CssExt
  {
    public static T Margin<T>(this T control, string margin) where T : IEditExtension
    {
      return CssAttribute(control, "margin", margin);
    }

    public static T Margin<T>(this T control, string vertMargin, string horMargin) where T : IEditExtension
    {
      return Margin(control, string.Format("{0} {1}", vertMargin, horMargin));
    }

    public static T Margin<T>(this T control,
      string topMargin, string rightMargin, string bottomMargin, string leftMargin) where T : IEditExtension
    {
      return Margin(control, string.Format("{0} {1} {2} {3}",
        topMargin, rightMargin, bottomMargin, leftMargin));
    }

    public static T Padding<T>(this T control, string padding) where T : IEditExtension
    {
      return CssAttribute(control, "padding", padding);
    }

    public static T Padding<T>(this T control, string vertPadding, string horPadding) where T : IEditExtension
    {
      return Padding(control, string.Format("{0} {1}", vertPadding, horPadding));
    }

    public static T Padding<T>(this T control,
      string topPadding, string rightPadding, string bottomPadding, string leftPadding) where T : IEditExtension
    {
      return Padding(control, string.Format("{0} {1} {2} {3}",
        topPadding, rightPadding, bottomPadding, leftPadding));
    }

    public static T Border<T>(this T control,
      string width, string style, Color color, string radius) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(radius))
        CssAttribute(control, "border-radius", radius);
      return CssAttribute(control, "border",
        string.Format("{0} {1} {2}", width, style, HtmlHlp.ColorToHtml(color))
      );
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

    public static T Background<T>(this T control, Color color) where T : IEditExtension
    {
      return CssAttribute(control, "background", HtmlHlp.ColorToHtml(color));
    }

    public static T Background<T>(this T control, Color color,
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

    public static T VAlign<T>(this T control, bool? isTop) where T : IEditExtension
    {
      string vAlign = "middle";
      if (isTop == true)
        vAlign = "top";
      else if (isTop == false)
        vAlign = "bottom";
      return CssAttribute(control, "vertical-align", vAlign);
    }

    public static T Align<T>(this T control, bool? isLeft, bool? isTop) where T : IEditExtension
    {
      VAlign(control, isTop);
      return Align(control, isLeft);
    }

    public static T Display<T>(this T control, string display) where T : IEditExtension
    {
      return CssAttribute(control, "display", display);
    }

    public static T LinearGradient<T>(this T control, string direction, Color beginColor, Color endColor)
      where T : IEditExtension
    {
      return CssAttribute(control, "background", string.Format("linear-gradient({0},{1},{2})",
        direction, HtmlHlp.ColorToHtml(beginColor), HtmlHlp.ColorToHtml(endColor))
      );
    }

    public static T Cursor<T>(this T control, string cursor) where T : IEditExtension
    {
      return CssAttribute(control, "cursor", cursor);
    }

  }
}
