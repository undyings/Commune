using Commune.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroBolt.Wui
{
  public static class HBuilderHelper2
  {
    public static IEnumerable<HElement> RoundPage(HElement page)
    {
      yield return page;
      foreach (HElement node in page.Elements())
      {
        foreach (HElement element in RoundPage(node))
          yield return element;
      }
    }

    public static hevent FindEvent(this HElement page, JsonData json, bool isStrongBinding)
    {
      foreach (HElement element in RoundPage(page))
      {
        HEventElement eventElement = element as HEventElement;
        if (eventElement == null || eventElement.handler == null)
          continue;

        if (IsEventElement(json, eventElement.handler, isStrongBinding))
          return eventElement.handler;
      }
      return null;
    }

    public static bool IsEventElement(JsonData json, hevent handler, bool isStrongBinding)
    {
      foreach (HAttribute id in handler)
      {
        object jsonId = json.JPath(id.Name.LocalName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
        if (jsonId == null)
          return false;

        if (isStrongBinding)
        {
          if (StringHlp.ToString(id.Value) != StringHlp.ToString(jsonId))
            return false;
        }
      }
      return true;
    }

    public static HElement LinkCss(this HBuilder h, string cssUrl)
    {
      return new HElement("link", h.Rel("stylesheet"), h.type("text/css"), h.href(cssUrl));
      //return Link(h, "stylesheet", cssUrl);
    }

    public static HAttribute Rel(this HBuilder h, string rel)
    {
      return new HAttribute("rel", rel);
    }

    public static HElement LinkScript(this HBuilder h, string jsUrl)
    {
      return new HElement("script", h.src(jsUrl), "");
    }
  }
}