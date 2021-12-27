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
    public static T AnimationName<T>(this T control, string name) where T : IEditExtension
    {
      return CssAttribute(control, "animation-name", name);
    }

    public static T AnimationDelay<T>(this T control, string delay) where T : IEditExtension
    {
      return CssAttribute(control, "animation-delay", delay);
    }

    public static T AnimationDuration<T>(this T control, string duration) where T : IEditExtension
    {
      return CssAttribute(control, "animation-duration", duration);
    }

    public static T AnimationIterationCount<T>(this T control, string iterationCount) where T : IEditExtension
    {
      return CssAttribute(control, "animation-iteration-count", iterationCount);
    }

    public static T AnimationTiming<T>(this T control, string timingFunction) where T : IEditExtension
    {
      return CssAttribute(control, "animation-timing-function", timingFunction);
    }

    public static T AnimationFillMode<T>(this T control, string fillMode) where T : IEditExtension
    {
      return CssAttribute(control, "animation-fill-mode", fillMode);
    }

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

    public static T MarginTop<T>(this T control, int marginTop) where T : IEditExtension
    {
      return MarginTop(control, string.Format("{0}px", marginTop));
    }

    public static T MarginTop<T>(this T control, string marginTop) where T : IEditExtension
    {
      return CssAttribute(control, "margin-top", marginTop);
    }

    public static T MarginBottom<T>(this T control, int marginBottom) where T : IEditExtension
    {
      return MarginBottom(control, string.Format("{0}px", marginBottom));
    }

    public static T MarginBottom<T>(this T control, string marginBottom) where T : IEditExtension
    {
      return CssAttribute(control, "margin-bottom", marginBottom);
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

    public static T PaddingLeft<T>(this T control, string paddingLeft) where T : IEditExtension
    {
      return CssAttribute(control, "padding-left", paddingLeft);
    }

    public static T PaddingLeft<T>(this T control, int paddingLeft) where T : IEditExtension
    {
      return PaddingLeft(control, string.Format("{0}px", paddingLeft));
    }

    public static T PaddingRight<T>(this T control, string paddingRight) where T : IEditExtension
    {
      return CssAttribute(control, "padding-right", paddingRight);
    }

    public static T PaddingRight<T>(this T control, int paddingRight) where T : IEditExtension
    {
      return PaddingRight(control, string.Format("{0}px", paddingRight));
    }

    public static T PaddingTop<T>(this T control, int paddingTop) where T : IEditExtension
    {
      return PaddingTop(control, string.Format("{0}px", paddingTop));
    }

    public static T PaddingTop<T>(this T control, string paddingTop) where T : IEditExtension
    {
      return CssAttribute(control, "padding-top", paddingTop);
    }

    public static T PaddingBottom<T>(this T control, int paddingBottom) where T : IEditExtension
    {
      return PaddingBottom(control, string.Format("{0}px", paddingBottom));
    }

    public static T PaddingBottom<T>(this T control, string paddingBottom) where T : IEditExtension
    {
      return CssAttribute(control, "padding-bottom", paddingBottom);
    }

    public static T Border<T>(this T control, string border) where T : IEditExtension
    {
      return CssAttribute(control, "border", border);
    }

    public static T BorderTop<T>(this T control, string borderTop) where T : IEditExtension
    {
      return CssAttribute(control, "border-top", borderTop);
    }

    public static T BorderBottom<T>(this T control, string borderBottom) where T : IEditExtension
    {
      return CssAttribute(control, "border-bottom", borderBottom);
    }

    public static T BorderLeft<T>(this T control, string borderLeft) where T : IEditExtension
    {
      return CssAttribute(control, "border-left", borderLeft);
    }

    public static T BorderRight<T>(this T control, string borderRight) where T : IEditExtension
    {
      return CssAttribute(control, "border-right", borderRight);
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

    public static T BorderRadius<T>(this T control, int radius) where T : IEditExtension
    {
      return BorderRadius(control, string.Format("{0}px", radius));
    }

    public static T BorderRadius<T>(this T control, string radius) where T : IEditExtension
    {
      return CssAttribute(control, "border-radius", radius);
    }

    public static T BorderWithRadius<T>(this T control, string border, int radius) where T : IEditExtension
    {
      return control.Border(border).BorderRadius(radius);
    }

    public static T Upper<T>(this T control) where T : IEditExtension
    {
      return CssAttribute(control, "text-transform", "uppercase");
    }

    public static T NoWrap<T>(this T control) where T : IEditExtension
    {
      return CssAttribute(control, "white-space", "nowrap");
    }

    public static T Wrap<T>(this T control) where T : IEditExtension
    {
      return CssAttribute(control, "white-space", "normal");
    }

    public static T CssAttribute<T>(this T control, string extensionName, object extensionValue) where T : IEditExtension
    {
      control.WithExtension(new CssExtensionAttribute(extensionName, extensionValue));
      return control;
    }

    public static T FontSize<T>(this T control, int fontSize) where T : IEditExtension
    {
      return FontSize(control, string.Format("{0}px", fontSize));
    }

    public static T FontSize<T>(this T control, string fontSize) where T : IEditExtension
    {
      return CssAttribute(control, "font-size", fontSize);
    }

    public static T Color<T>(this T control, string color) where T : IEditExtension
    {
      return CssAttribute(control, "color", color);
    }

    public static T Background<T>(this T control, string color) where T : IEditExtension
    {
      return CssAttribute(control, "background", color);
    }

    public static T Background<T>(this T control,
      string imagePath, string repeat, string position) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(imagePath))
        CssAttribute(control, "background-image", string.Format("url({0})", imagePath));
      if (!StringHlp.IsEmpty(repeat))
        CssAttribute(control, "background-repeat", repeat);
      if (!StringHlp.IsEmpty(position))
        CssAttribute(control, "background-position", position);
      return control;
    }

    public static T BackgroundImage<T>(this T control, string imagePath) where T : IEditExtension
    {
			string value = imagePath != "none" ? string.Format("url({0})", imagePath) : "none";
			return CssAttribute(control, "background-image", value);
    }

		public static T BackgroundPosition<T>(this T control, string position) where T : IEditExtension
		{
			return CssAttribute(control, "background-position", position);
		}

    public static T BackgroundSize<T>(this T control, string size) where T : IEditExtension
    {
      return CssAttribute(control, "background-size", size);
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

    public static T Width<T>(this T control, int width) where T : IEditExtension
    {
      return Width(control, string.Format("{0}px", width));
    }

    public static T Width<T>(this T control, string width) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(width))
        CssAttribute(control, "width", width);
      return control;
    }

    public static T WidthFull<T>(this T control) where T : IEditExtension
    {
      return Width(control, "100%");
    }

		public static T WidthAuto<T>(this T control) where T : IEditExtension
		{
			return Width(control, "auto");
		}

    public static T WidthLimit<T>(this T control, string minWidth, string maxWidth) where T : IEditExtension
    {
      if (!StringHlp.IsEmpty(minWidth))
        CssAttribute(control, "min-width", minWidth);
      if (!StringHlp.IsEmpty(maxWidth))
        CssAttribute(control, "max-width", maxWidth);
      return control;
    }

    public static T WidthFixed<T>(this T control, int width) where T : IEditExtension
    {
      string widthPx = string.Format("{0}px", width);
      return control.WidthLimit(widthPx, widthPx);
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

    /// <summary>
    /// Выставляет контролу width: <<widhtInPercent>>% display: inline-block и box-sizing: border-box
    /// </summary>
    public static T RelativeWidth<T>(this T control, float widthInPercent) where T : IEditExtension
    {
      return control.Width(widthInPercent.ToString("F3", CultureInfo.InvariantCulture) + "%")
        .Display("inline-block").BoxSizing();
    }

    public static T Height<T>(this T control, int height) where T : IEditExtension
    {
      return Height(control, string.Format("{0}px", height));
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

    public static T HeightFixed<T>(this T control, int height) where T : IEditExtension
    {
      string heightPx = string.Format("{0}px", height);
      return control.HeightLimit(heightPx, heightPx);
    }

    public static T Position<T>(this T control, string position) where T : IEditExtension
    {
      return control.CssAttribute("position", position);
    }

    public static T PositionAbsolute<T>(this T control) where T : IEditExtension
    {
      return control.CssAttribute("position", "absolute");
    }

    public static T PositionRelative<T>(this T control) where T : IEditExtension
    {
      return control.CssAttribute("position", "relative");
    }

    public static T PositionAbsoluteWithVAlign<T>(this T control) where T : IEditExtension
    {
      return control.PositionAbsolute().Top("50%").Transform("translateY(-50%)");
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

    public static T Block<T>(this T control) where T : IEditExtension
    {
      return Display(control, "block");
    }

    public static T InlineBlock<T>(this T control) where T : IEditExtension
    {
      return Display(control, "inline-block");
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

    public static T CursorPointer<T>(this T control) where T : IEditExtension
    {
      return Cursor(control, "pointer");
    }

    public static T CursorDefault<T>(this T control) where T : IEditExtension
    {
      return Cursor(control, "default");
    }

    public static T UserSelect<T>(this T control, string value) where T : IEditExtension
    {
      CssAttribute(control, "-moz-user-select", value);
      CssAttribute(control, "-webkit-user-select", value);
      CssAttribute(control, "-ms-user-select", value);
      return CssAttribute(control, "user-select", value);
    }

		public static T Outline<T>(this T control, string value) where T : IEditExtension
		{
			return CssAttribute(control, "outline", value);
		}

		public static T Appearance<T>(this T control, string value) where T : IEditExtension
		{
			CssAttribute(control, "-webkit-appearance", value);
			CssAttribute(control, "-moz-appearance", value);
			CssAttribute(control, "appearance", value);
			return control;
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

    public static T FontBold<T>(this T control) where T : IEditExtension
    {
      return FontBold(control, true);
    }

    public static T FontItalic<T>(this T control) where T : IEditExtension
    {
      return CssAttribute(control, "font-style", "italic");
    }

    public static T FontFamily<T>(this T control, string fontFamily) where T : IEditExtension
    {
      return CssAttribute(control, "font-family", string.Format("'{0}'", fontFamily));
    }

    public static T LetterSpacing<T>(this T control, string spacing) where T : IEditExtension
    {
      return CssAttribute(control, "letter-spacing", spacing);
    }

    public static T TextDecoration<T>(this T control, string decoration) where T : IEditExtension
    {
      return CssAttribute(control, "text-decoration", decoration);
    }

    public static T BoxShadow<T>(this T control, string shadow) where T : IEditExtension
    {
      return CssAttribute(control, "box-shadow", shadow);
    }

    public static T TextShadow<T>(this T control, string shadow) where T : IEditExtension
    {
      return CssAttribute(control, "text-shadow", shadow);
    }

    public static T TextShadow3D<T>(this T control, string color, int debth) where T : IEditExtension
    {
      string[] shadows = new string[debth];
      for (int i = 0; i < debth; ++i)
      {
        shadows[i] = string.Format("{0} {1}px {1}px 0", color, i + 1);
      }
      string shadow3d = string.Join(", ", shadows);
      return control.TextShadow(shadow3d);
    }

    public static T TextShadowContour<T>(this T control, string color) where T : IEditExtension
    {
      string[] shadows = new string[4];

      int[] shifts = new int[] { -1, 1 };
      for (int i = 0; i < 2; ++i)
        for (int j = 0; j < 2; ++j)
        {
          shadows[j*2 + i] = string.Format("{0} {1}px {2}px 0", color, shifts[i], shifts[j]);
        }
      string contour = string.Join(", ", shadows);
      return control.TextShadow(contour);
    }

    public static T Transform<T>(this T control, string transform) where T : IEditExtension
    {
      CssAttribute(control, "-webkit-transform", transform);
      CssAttribute(control, "-moz-transform", transform);
      CssAttribute(control, "-ms-transform", transform);
      return CssAttribute(control, "transform", transform);
    }

    public static T Opacity<T>(this T control, string opacity) where T : IEditExtension
    {
      return CssAttribute(control, "opacity", opacity);
    }

    public static T LineHeight<T>(this T control, int lineHeight) where T : IEditExtension
    {
      return LineHeight(control, string.Format("{0}px", lineHeight));
    }

    public static T LineHeight<T>(this T control, string lineHeight) where T : IEditExtension
    {
      return CssAttribute(control, "line-height", lineHeight);
    }

    public static T Content<T>(this T control, string content) where T : IEditExtension
    {
      return CssAttribute(control, "content", string.Format("'{0}'", content));
    }

    public static T ContentIcon<T>(this T pseudo, int width, int height) where T : IEditExtension
    {
      return pseudo.Content("").InlineBlock().Size(width, height);
    }

    public static T ContentIcon<T>(this T pseudo, int width, int height, string backgroundUrl) where T : IEditExtension
    {
      pseudo.BackgroundImage(backgroundUrl);
      return pseudo.ContentIcon(width, height);
    }

    public static T Left<T>(this T control, string left) where T : IEditExtension
    {
      return CssAttribute(control, "left", left);
    }

    public static T Left<T>(this T control, int left) where T : IEditExtension
    {
      return Left(control, string.Format("{0}px", left));
    }

    public static T Right<T>(this T control, string right) where T : IEditExtension
    {
      return CssAttribute(control, "right", right);
    }

    public static T Right<T>(this T control, int right) where T : IEditExtension
    {
      return Right(control, string.Format("{0}px", right));
    }

    public static T Top<T>(this T control, string top) where T : IEditExtension
    {
      return CssAttribute(control, "top", top);
    }

    public static T Top<T>(this T control, int top) where T : IEditExtension
    {
      return Top(control, string.Format("{0}px", top));
    }

    public static T Bottom<T>(this T control, string bottom) where T : IEditExtension
    {
      return CssAttribute(control, "bottom", bottom);
    }

    public static T Bottom<T>(this T control, int bottom) where T : IEditExtension
    {
      return Bottom(control, string.Format("{0}px", bottom));
    }

    public static T Float<T>(this T control, string floats) where T : IEditExtension
    {
      return CssAttribute(control, "float", floats);
    }

    public static T FloatLeft<T>(this T control) where T : IEditExtension
    {
      return Float(control, "left");
    }

    public static T FloatRight<T>(this T control) where T : IEditExtension
    {
      return Float(control, "right");
    }

    public static T Delay<T>(this T control, int delayInMilliseconds) where T : IEditExtension
    {
      return CssAttribute(control, "transition-delay", string.Format("{0}ms", delayInMilliseconds));
    }

    public static T MediaBlock<T>(this T control, bool? isLeft = null) where T : IEditExtension
    {
      return control.Block().Width("100%").BoxSizing().Align(isLeft);
    }

    public static T CenterBlock<T>(this T control) where T : IEditExtension
    {
      return control.PositionAbsolute().Top("50%").Left("50%").Transform("translate(-50%, -50%)");
    }

    public static T ZIndex<T>(this T control, int zindex) where T : IEditExtension
    {
      return CssAttribute(control, "z-index", zindex);
    }
  }
}
