using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public static class HtmlExt
  {
    public static object GetData(this JsonData json, string dataName)
    {
      return json.JPath("data", dataName);
    }

    public static string GetText(this JsonData json, string dataName)
    {
      object data = json.GetData(dataName);
      if (data == null)
        return null;
      return data.ToString();
    }

    public static bool GetBool(this JsonData json, string dataName)
    {
      string rawData = json.GetText(dataName);
      return rawData?.ToLower() == "true";
    }

    public static int? GetInt(this JsonData json, string dataName)
    {
      return ConvertHlp.ToInt(json.GetData(dataName));
    }

    public static T Media<T>(this T control, string queryWithBrackets, params HStyle[] styles)
      where T : IEditExtension
    {
      control.WithExtension(new MediaExtensionAttribute(queryWithBrackets, styles));
      return control;
    }

    public static T Media<T>(this T control, int maxWidth, params HStyle[] styles)
      where T : IEditExtension
    {
      return control.Media(string.Format("(max-width: {0}px)", maxWidth), styles);
    }

    public static T Hide<T>(this T control, bool hide) where T : IEditExtension
    {
      control.WithExtension(new ExtensionAttribute("hide", hide));
      return control;
    }

    public static T Value<T>(this T control, object value) where T : IEditExtension
    {
      control.WithExtension(new ExtensionAttribute("value", value));
      return control;
    }

    public static T EditContainer<T>(this T control, string container) where T : IEditExtension
    {
      control.WithExtension(new ExtensionAttribute("container", container));
      return control;
    }

    public static T Event<T>(this T control, hevent onevent) where T : IEventEditExtension
    {
      control.WithExtension(new ExtensionAttribute("onevent", onevent));
      return control;
    }

    public static T Event<T>(this T control, string command, string editContainer, 
      Action<JsonData> eventHandler, params object[] extraIds) where T : IEventEditExtension
    {
      hevent onevent = InnerEvent(command, editContainer, eventHandler, extraIds);

      return Event(control, onevent);
    }

    public static hevent InnerEvent(string command, string editContainer,
      Action<JsonData> eventHandler, params object[] extraIds)
    {
      hevent onevent = new hevent(delegate (object[] ids, JsonData json)
      {
        eventHandler(json);
        return null;
      }) { { "command", command } };

      if (!StringHlp.IsEmpty(editContainer))
        onevent.Add("container", editContainer);

      int i = -1;
      foreach (object id in extraIds)
      {
        ++i;
        onevent.Add(string.Format("id{0}", i + 1), id);
      }

      return onevent;
    }

    public static T ExtraClassNames<T>(this T control, params string[] classNames) where T : IEditExtension
    {
      control.WithExtension(new ExtensionAttribute("extraClassNames", classNames));
      return control;
    }
    
    /// <summary>
    /// Поддерживается элементами выпадающего списка DropDown.
    /// Клик на невыбираемый элемент не закрывает список. Невыбираемые элементы не используют стили AnyItem.
    /// </summary>
    public static T Unselectable<T>(this T control) where T : IEditExtension
    {
      control.WithExtension(new ExtensionAttribute("unselectable", true));
      return control;
    }

    public static bool Validate(this WebOperation operation, bool isError, string errorMessage)
    {
      if (isError)
      {
        operation.Message = errorMessage;
        return false;
      }

      return true;
    }

    public static bool Validate(this WebOperation operation, string value, string errorMessage)
    {
      return Validate(operation, StringHlp.IsEmpty(value), errorMessage);
    }
  }

  public class WebOperation
  {
    public string Status = "";
    public string Message = "";
    public bool Completed = false;
    public string ReturnUrl = "";

    public void Reset()
    {
      Status = "";
      Message = "";
      Completed = false;
      ReturnUrl = "";
    }

    public void Complete(string message, string returnUrl)
    {
      this.Completed = true;
      this.Message = message;
      this.ReturnUrl = returnUrl;
    }

    public WebOperation()
    {
    }
  }

  public class DialogIcon
  {
    public const string Info = "Info";
    public const string Warning = "Warning";
    public const string Question = "Question";
    public const string Error = "Error";
    public const string Success = "Success";
  }
}
