using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HFileUploader : ExtensionContainer, IHtmlControl
  {
    readonly string fileUploadJsPath;
    readonly string caption;
    readonly object objectId;
    readonly string[] gauges;

    /// <summary>
    /// Для использования подключите fileuploader.css и fileuploader.js.
    /// Реализуйте FileUploader : IHttpHandler
    /// Пропишите в web.config:
    /// <add name="fileUploader" verb="*" path="<<fileUploadJsPath>>" type="<<YourNamespace.FileUploader>>"/>
    /// </summary>
    /// <param name="fileUploadJsPath">Например, fileupload.js</param>
    /// <param name="objectId">Идентификатор объекта, к которому относятся загружаемые файлы</param>
    public HFileUploader(string fileUploadJsPath, string caption, object objectId, params string[] gauges) :
      base("HFileUploader", "")
    {
      this.fileUploadJsPath = fileUploadJsPath;
      this.caption = caption;
      this.objectId = objectId;
      this.gauges = gauges;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      List<object> content = new List<object>();
      {
        StringBuilder builder = new StringBuilder();
        builder.Append("new qq.FileUploader({element: this");
        builder.AppendFormat(", action: '{0}'", fileUploadJsPath);
        builder.Append(", encoding: 'multipart'");
        builder.AppendFormat(", uploadButtonText: '{0}'", caption);
        builder.AppendFormat(", params: {{objectId: '{0}'}}", objectId);
        foreach (string gauge in gauges)
        {
          builder.Append(", ");
          builder.Append(gauge);
        }
        builder.Append("})");

        content.Add(h.Attribute("js-init", builder.ToString())
        );

        //content.Add(h.Attribute("js-init", string.Format(
        //  "new qq.FileUploader({{element: this, action: '{0}', encoding: 'multipart', uploadButtonText: '{1}', params: {{objectId: '{2}'}} }})",
        //  fileUploadJsPath, caption, objectId
        //  ))
        //);
      }
      foreach (TagExtensionAttribute extension in TagExtensions)
        content.Add(new HAttribute(extension.Name, extension.Value));

      return h.Div(content.ToArray());
    }
  }
}
