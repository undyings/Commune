using System;
using System.Collections.Generic;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
  public class ExtensionAttribute : Attribute
  {
    public ExtensionAttribute()
    {
    }
    public ExtensionAttribute(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    public readonly string Name;
    public readonly object Value;
  }

  public class CssExtensionAttribute : ExtensionAttribute
  {
    public CssExtensionAttribute(string name, object value) :
      base(name, value)
    {
    }
  }

  public class TagExtensionAttribute : ExtensionAttribute
  {
    public TagExtensionAttribute(string name, object value) :
      base(name, value)
    {
    }
  }

  public interface IHtmlControl : IReadExtension
  {
    string Name { get; }
    HElement ToHtml(string cssClassName, StringBuilder css);
  }

  public interface IReadExtension
  {
    object GetExtended(string ext);
    IEnumerable<CssExtensionAttribute> CssExtensions { get; }
    IEnumerable<TagExtensionAttribute> TagExtensions { get; }
  }

  public interface IEditExtension
  {
    void WithExtension(ExtensionAttribute extension);
  }

  public class PseudoCssClass : ExtensionContainer
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="formatName">Например '.{0} tr:hover'. Вместо {0} автоматически подставляется наименование html контрола, который содержит псевдокласс</param>
    public PseudoCssClass(string formatName) :
      base("PseudoCssClass", formatName)
    {
    }
  }

  public class DefaultExtensionContainer : IEditExtension
  {
    readonly IEditExtension control;
    readonly IReadExtension controlAsRead;
    public DefaultExtensionContainer(IEditExtension control)
    {
      this.control = control;
      this.controlAsRead = control as IReadExtension;
    }

    public void WithExtension(ExtensionAttribute extension)
    {
      if (controlAsRead == null || controlAsRead.GetExtended(extension.Name) == null)
        control.WithExtension(extension);
    }
  }

  public class ExtensionContainer : IReadExtension, IEditExtension
  {
    protected readonly Dictionary<string, ExtensionAttribute> extensionByName = new Dictionary<string, ExtensionAttribute>();
    readonly string containerType;
    string name;
    public string Name
    {
      get { return name; }
    }
    public ExtensionContainer(string containerType, string name)
    {
      this.containerType = containerType;
      this.name = name;
    }

    public IEnumerable<CssExtensionAttribute> CssExtensions
    {
      get
      {
        foreach (ExtensionAttribute extension in extensionByName.Values)
        {
          if (extension is CssExtensionAttribute)
            yield return (CssExtensionAttribute)extension;
        }
      }
    }

    public IEnumerable<TagExtensionAttribute> TagExtensions
    {
      get
      {
        foreach (ExtensionAttribute extension in extensionByName.Values)
        {
          if (extension is TagExtensionAttribute)
            yield return (TagExtensionAttribute)extension;
        }
      }
    }

    public object GetExtended(string extensionName)
    {
      ExtensionAttribute extensionAttr;
      if (extensionByName.TryGetValue(extensionName, out extensionAttr))
        return extensionAttr.Value;

      return null;
    }

    public void WithExtension(ExtensionAttribute extension)
    {
      if (extension == null)
        return;
      if (extensionByName.ContainsKey(extension.Name))
        Logger.AddMessage("В {0} '{1}' расширение '{2}' задается повторно", containerType, Name, extension.Name);
      extensionByName[extension.Name] = extension;
    }
  }
  

 
}
