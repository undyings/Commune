using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public static class TagExt
  {
    public static T TagAttribute<T>(this T control, string extensionName, object extensionValue) where T : IEditExtension
    {
      control.WithExtension(new TagExtensionAttribute(extensionName, extensionValue));
      return control;
    }

		public static T Disabled<T>(this T control, bool disabled) where T : IEditExtension
		{
			if (!disabled)
				return control;

			return TagAttribute(control, "disabled", "disabled");
		}

    public static T Title<T>(this T control, string toolTip) where T : IEditExtension
    {
      return TagAttribute(control, "title", toolTip);
    }

    public static T Alt<T>(this T control, string altText) where T : HImage
    {
      return TagAttribute(control, "alt", altText);
    }

    public static T Placeholder<T>(this T control, string placeholder) where T : IEditExtension
    {
      return TagAttribute(control, "placeholder", placeholder);
    }

    public static T OnClick<T>(this T control, string onClick) where T : IEditExtension
    {
      return TagAttribute(control, "onclick", onClick);
    }

    public static T OnClickWithStopPropagation<T>(this T control) where T : IEditExtension
    {
      return OnClick(control, "e.stopPropagation();");
    }

    public static T OnChange<T>(this T control, string onChange) where T : IEditExtension
    {
      return TagAttribute(control, "onchange", onChange);
    }

    public static T OnKeyDown<T>(this T control, string onKeyDown) where T : IEditExtension
    {
      return TagAttribute(control, "onkeydown", onKeyDown);
    }

    public static T TabIndex<T>(this T control, int tabIndex) where T : IEditExtension
    {
      return TagAttribute(control, "tabindex", tabIndex.ToString());
    }

    public static HLink TargetBlank(this HLink control)
    {
      return TagAttribute(control, "target", "_blank");
    }

    public static T Autofocus<T>(this T control) where T : IEditExtension
    {
      return TagAttribute(control, "autofocus", "autofocus");
    }

    public static T Nofollow<T>(this T control) where T : HLink
    {
      return TagAttribute(control, "rel", "nofollow");
    }

    public static T PagingTouch<T>(this T control) where T : IEditExtension
    {
      return PagingTouch(control, "prev", "next");
    }

    public static T PagingTouch<T>(this T control, 
      string prevButtonClassName, string nextButtonClassName) where T : IEditExtension
    {
      return control
        .TagAttribute("ontouchstart", "window.touchStart = event.touches[0];")
        .TagAttribute("ontouchend", "window.touchStart = null;")
        .TagAttribute("ontouchmove", string.Format(@"
if (window.touchStart == null) return;
var p1 = window.touchStart;
var p2 = event.changedTouches[0];
var x = p2.screenX - p1.screenX;
var y = Math.abs(p2.screenY - p1.screenY);
if (x > 30 && y < x) {{ var b = $('.{0}'); if (b.length > 0) b[0].click(); window.touchStart = null; return; }}
if (x < -30 && y < -x ) {{ var b = $('.{1}'); if (b.length > 0) b[0].click(); window.touchStart = null; return; }}
", prevButtonClassName, nextButtonClassName));
    }

    public static T PagingClick<T>(this T control) where T : IEditExtension
    {
      return control.OnClick(@"
var width = document.documentElement.clientWidth;
var x = e.clientX;
if (x > width / 2) { var b = $('.next'); if (b.length > 0) b[0].click(); }
else { var b = $('.prev'); if (b.length > 0) b[0].click(); }
e.stopPropagation();
");
    }
  }
}
