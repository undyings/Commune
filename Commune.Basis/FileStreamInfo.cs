using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Commune.Basis
{
  //тип данных: binary text xml
  //тип доступа: чтение запись (чтение+запись?)
  //(?)хранение: диск, память
  //"технические" типы: filename, Stream, TextReader/TextWriter, XmlReader/XmlWriter
  //(?)BinaryReader/BinaryWriter
  //вспомогательные параметры: 
  //  text и xml: encoding
  //  xml: всякие xml настройки (indent, заголовок нужен/нет и т.д.)
  //
  //это просто фабрика, созданные writer-ы/reader-ы внутри не сохраняются
  public class FileStreamInfo
  {
    public static FileStreamInfo WithFilename(string filename)
    {
      return new FileStreamInfo(filename, null, null, null, null);
    }
    //public static FileStreamInfo WithMemoryStream()
    //{
    //}
    public static FileStreamInfo WithString()
    {
      return new FileStreamInfo(null, null, new StringBuilder(), null, null);
    }
    public static FileStreamInfo WithTextWriter(TextWriter textWriter)
    {
      return new FileStreamInfo(null, null, null, textWriter, null);
    }
    public static FileStreamInfo WithXmlWriter(XmlWriter xmlWriter)
    {
      return new FileStreamInfo(null, null, null, null, xmlWriter);
    }
    public FileStreamInfo WithFormating()
    {
      this.formating = true;
      return this;
    }
    private FileStreamInfo
      (string filename, Encoding encoding, StringBuilder builder, TextWriter textWriter, XmlWriter xmlWriter)
    {
      this.Filename = filename;
      this.Encoding = encoding ?? Encoding.UTF8;
      this.Builder = builder;
      this.TextWriter = textWriter;
      this.XmlWriter = xmlWriter;
    }
    public readonly string Filename;
    public readonly Encoding Encoding;
    public readonly StringBuilder Builder;
    public readonly TextWriter TextWriter;
    public readonly XmlWriter XmlWriter;
    bool formating;

    public XmlWriter GetXmlWriter()
    {
      XmlTextWriter writer = null;
      if (Filename != null)
        writer = new XmlTextWriter(Filename, Encoding);
      else if (Builder != null)
        writer = new XmlTextWriter(new StringWriter(Builder));
      else if (TextWriter != null)
        writer = new XmlTextWriter(TextWriter);
      else if (XmlWriter != null)
        return XmlWriter;
      else
        throw new Exception(string.Format("Из заданного на вход '{0}' не умею создавать XmlWriter, дайте на вход что-нибудь другое", MakeInputDescription()));

      if (writer != null && formating)
        writer.Formatting = Formatting.Indented;
      return writer;
    }
    //class InputDescription
    //{
    //  public InputDescription(object input, Executter generate)
    //  {
    //    this.Input = input;
    //    this.Generate = generate;
    //  }
    //  public readonly object Input;
    //  public readonly Executter Generate;
    //}
    private string MakeInputDescription()
    {
      List<string> descriptions = new List<string>();
      if (Filename != null)
        descriptions.Add(string.Format("название файла({0})", Filename));
      if (Encoding != null)
        descriptions.Add(string.Format("кодировка - ({0})", Encoding));
      if (Builder != null)
        descriptions.Add("StringBuilder");

      return StringHlp.JoinNotEmpty(", ", descriptions.ToArray());
    }

    public string GetResultAsString()
    {
      if (Builder != null)
        return Builder.ToString();
      throw new Exception(string.Format("Из заданного на вход '{0}' не умею возвращать результат как строку, дайте на вход что-нибудь другое", MakeInputDescription()));
    }
  }
}
