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

    /// <summary>
    /// Для использования подключите fileuploader.css и fileuploader.js.
    /// Реализуйте FileUploader : IHttpHandler
    /// Пропишите в web.config:
    /// <add name="fileUploader" verb="*" path="<<fileUploadJsPath>>" type="<<YourNamespace.FileUploader>>"/>
    /// </summary>
    /// <param name="fileUploadJsPath">Например, fileupload.js</param>
    /// <param name="objectId">Идентификатор объекта, к которому относятся загружаемые файлы</param>
    public HFileUploader(string fileUploadJsPath, string caption, object objectId) :
      base("HFileUploader", "")
    {
      this.fileUploadJsPath = fileUploadJsPath;
      this.caption = caption;
      this.objectId = objectId;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      List<object> content = new List<object>();
      content.Add(h.Attribute("js-init", string.Format(
        "new qq.FileUploader({{element: this, action: '{0}', encoding: 'multipart', uploadButtonText: '{1}', params: {{objectId: '{2}'}} }})",
        fileUploadJsPath, caption, objectId
        ))
      );
      foreach (TagExtensionAttribute extension in TagExtensions)
        content.Add(new HAttribute(extension.Name, extension.Value));

      return h.Div(content.ToArray());
    }
  }
}
