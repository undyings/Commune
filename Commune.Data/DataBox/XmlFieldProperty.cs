using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using Commune.Basis;

namespace Commune.Data
{
  public class XmlLogin : XmlUniqueProperty<string>
  {
    readonly static XmlFieldBlank xmlIds = new XmlFieldBlank(
      ObjectType.XmlObjectIds, "Auth", "Login");

    public readonly static XmlLogin Auth = new XmlLogin("Auth", null);
    public readonly static XmlLogin Login = new XmlLogin("Login", null);

    public XmlLogin(string propertyKind, Getter<string, string> propertyConverter) :
      base(xmlIds, propertyKind, propertyConverter)
    {
    }

    public string CreateXmlIds(string auth, string login)
    {
      return xmlField.Create(auth, login);
    }
  }

  public class XmlDisplayName : XmlUniqueName
  {
    public XmlDisplayName() :
      base(new XmlFieldBlank(ObjectType.XmlObjectIds, "DisplayName"), "DisplayName")
    {
    }

    public string CreateXmlIds(string displayName)
    {
      return xmlField.Create(displayName);
    }
  }


  public class XmlUniqueName : XmlUniqueProperty<string>
  {
    public XmlUniqueName(XmlFieldBlank xmlField, string propertyKind) :
      base(xmlField, propertyKind)
    {
    }

    public void SetUniqueName(ObjectHeadBox objectBox, RowLink rowLink, string exampleName)
    {
      //KeyUniqueChecker uniqueChecker = objectBox.ObjectUniqueChecker;

      if (exampleName == null)
        exampleName = "";

      for (int i = 0; i < 100; ++i)
      {
        string propertyValue = exampleName;
        if (i != 0)
          propertyValue += string.Format("({0})", i + 1);

        if (SetWithCheck(objectBox, rowLink, propertyValue))
          return;

        //string xmlValue = xmlField.CreateXmlValue(rowLink, propertyKind, propertyValue);
        //if (uniqueChecker.IsUniqueRow(rowLink, xmlField.Field, xmlValue))
        //{
        //  xmlField.Field.Set(rowLink, xmlValue);
        //  return;
        //}
      }
      string xmlUniqueValue = xmlField.Change(rowLink, propertyKind,
        string.Format("{0}({1})", exampleName, Guid.NewGuid().ToString()));
      xmlField.Field.Set(rowLink, xmlUniqueValue);
    }
  }

  public class XmlUniqueProperty<T>
  {
    protected readonly XmlFieldBlank xmlField;
    protected readonly string propertyKind;
    protected readonly Getter<T, string> propertyConverter;
    public string Kind
    {
      get { return propertyKind; }
    }

    public XmlUniqueProperty(XmlFieldBlank xmlField, string propertyKind) :
      this(xmlField, propertyKind, null)
    {
    }

    public XmlUniqueProperty(XmlFieldBlank xmlField, string propertyKind, Getter<T, string> propertyConverter)
    {
      this.xmlField = xmlField;
      this.propertyKind = propertyKind;
      this.propertyConverter = propertyConverter;
    }

    public T Get(string xmlIds)
    {
      string property = XmlFieldBlank.GetPropertyValue(xmlIds, propertyKind);
      if (propertyConverter == null)
        return (T)(object)property;
      return propertyConverter(property);
    }

    public T Get(IRowLink rowLink)
    {
      string property = xmlField.GetProperty(rowLink, propertyKind);
      if (propertyConverter == null)
        return (T)(object)property;
      return propertyConverter(property);
    }

    public T Get(ObjectHeadBox objectBox, object primaryKey)
    {
      RowLink rowLink = _.First(objectBox.ObjectById.Rows(primaryKey));
      if (rowLink == null)
        return default(T);

      return Get(rowLink);
    }

    public bool SetWithCheck(ObjectHeadBox objectBox, object primaryKey, T propertyValue)
    {
      RowLink rowLink = _.First(objectBox.ObjectById.Rows(primaryKey));
      return SetWithCheck(objectBox, rowLink, propertyValue);
    }

    public bool SetWithCheck(ObjectHeadBox objectBox, RowLink objectRow, T propertyValue)
    {
      if (objectRow == null)
        return false;

      UniqueChecker uniqueChecker = objectBox.ObjectUniqueChecker;

      string propertyValueAsStr = null;
      if (propertyValue != null)
        propertyValueAsStr = propertyValue.ToString();
      string xmlValue = xmlField.Change(objectRow, propertyKind, propertyValueAsStr);

      if (!uniqueChecker.IsUniqueKey(objectRow.Get(ObjectType.ObjectId), objectRow.Get(ObjectType.TypeId),
        xmlValue, objectRow.Get(ObjectType.ActFrom)))
        return false;

      xmlField.Field.Set(objectRow, xmlValue);
      return true;
    }

    public void SetWithoutCheck(IRowLink rowLink, T propertyValue)
    {
      string propertyValueAsStr = null;
      if (propertyValue != null)
        propertyValueAsStr = propertyValue.ToString();

      string xmlValue = xmlField.Change(rowLink, propertyKind, propertyValueAsStr);
      xmlField.Field.Set(rowLink, xmlValue);
    }
  }

  public class XmlFieldBlank
  {
    public readonly FieldBlank<string> Field;
    public readonly string[] propertyKindsOrder;

    public XmlFieldBlank(FieldBlank<string> field, params string[] propertyKindsOrder)
    {
      this.Field = field;
      this.propertyKindsOrder = propertyKindsOrder;
    }

    public static string GetPropertyValue(string xmlString, string propertyKind)
    {
      if (xmlString == null)
        return null;

      string beginXmlAttribute = string.Format("<{0}>", propertyKind);
      string endXmlAttribute = string.Format("</{0}>", propertyKind);

      int rawStartIndex = xmlString.IndexOf(beginXmlAttribute);
      if (rawStartIndex == -1)
        return null;

      int startIndex = rawStartIndex + beginXmlAttribute.Length;
      int endIndex = xmlString.IndexOf(endXmlAttribute, startIndex);
      if (endIndex == -1)
        return null;

      return xmlString.Substring(startIndex, endIndex - startIndex);
    }

    public string GetProperty(IRowLink rowLink, string propertyKind)
    {
      string xmlString = Field.Get(rowLink);
      return HttpUtility.HtmlDecode(GetPropertyValue(xmlString, propertyKind));
    }

    public string Create(params string[] propertyValues)
    {
      StringBuilder newXmlString = new StringBuilder();
      for (int i = 0; i < propertyKindsOrder.Length; ++i)
      {
        if (StringHlp.IsEmpty(propertyValues[i]))
          continue;
        newXmlString.AppendFormat("<{0}>{1}</{0}>", propertyKindsOrder[i], HttpUtility.HtmlEncode(propertyValues[i]));
      }
      return newXmlString.ToString();
    }

    public string Change(IRowLink rowLink, string propertyKind, string propertyValue)
    {
      string oldXmlString = Field.Get(rowLink);
      StringBuilder newXmlString = new StringBuilder();
      foreach (string attributeKind in propertyKindsOrder)
      {
        string databaseValue = null;
        if (attributeKind == propertyKind)
        {
          if (propertyValue != null)
            databaseValue = HttpUtility.HtmlEncode(propertyValue);
        }
        else
          databaseValue = GetPropertyValue(oldXmlString, attributeKind);

        if (databaseValue == null || databaseValue == "")
          continue;

        newXmlString.AppendFormat("<{0}>{1}</{0}>", attributeKind, databaseValue);
      }
      return newXmlString.ToString();
    }
  }
}
