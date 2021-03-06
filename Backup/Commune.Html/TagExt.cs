﻿using System;
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

    public static T ToolTip<T>(this T control, string toolTip) where T : IEditExtension
    {
      return TagAttribute(control, "title", toolTip);
    }

    public static T Placeholder<T>(this T control, string placeholder) where T : IEditExtension
    {
      return TagAttribute(control, "placeholder", placeholder);
    }

    public static T OnClick<T>(this T control, string onClick) where T : IEditExtension
    {
      return TagAttribute(control, "onclick", onClick);
    }
  }
}
