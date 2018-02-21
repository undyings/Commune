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

        //Logger.AddMessage("IsEventElement: {0}, {1}", id, jsonId);
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

    public static HElement LinkShortcutIcon(this HBuilder h, string iconUrl)
    {
      bool isPng = iconUrl.EndsWith("png", StringComparison.InvariantCultureIgnoreCase);
      string type = isPng ? "image / png" : "image / x - icon";
      return new HElement("link", h.Rel("shortcut icon"), h.href(iconUrl), h.type(type));
    }

    public static HAttribute Rel(this HBuilder h, string rel)
    {
      return new HAttribute("rel", rel);
    }

    public static HElement LinkScript(this HBuilder h, string jsUrl)
    {
      return new HElement("script", h.src(jsUrl), "");
    }

    public static HElement MetaKeywords(this HBuilder h, string keywords)
    {
      return new HElement("meta", new HAttribute("name", "keywords"), new HAttribute("content", keywords));
    }

    public static HElement MetaDescription(this HBuilder h, string description)
    {
      return new HElement("meta", new HAttribute("name", "description"), new HAttribute("content", description));
    }

    public static HElement Meta(this HBuilder h, string name, string content)
    {
      return new HElement("meta", new HAttribute("name", name), new HAttribute("content", content));
    }

    public static HElement OpenGraph(this HBuilder h, string ogType, string content)
    {
      return new HElement("meta", 
        new HAttribute("property", string.Format("og:{0}", ogType)),
        new HAttribute("content", content)
      );
    }
  }
}