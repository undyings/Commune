using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Commune.Basis
{
  public class XmlEmbeddingWriter : XmlWriter
  {
    public XmlEmbeddingWriter(XmlWriter writer, bool skipRoot)
    {
      this.writer = writer;
      this.skipRoot = skipRoot;
    }
    public XmlEmbeddingWriter(XmlWriter writer, string rootName)
    {
      this.writer = writer;
      this.skipRoot = string.IsNullOrEmpty(rootName);
      this.rootName = rootName;
    }
    XmlWriter writer;
    bool skipRoot;
    string rootName;

    public override void Close()
    {
      //writer.Close();
    }

    public override void Flush()
    {
      writer.Flush();
    }

    public override string LookupPrefix(string ns)
    {
      return writer.LookupPrefix(ns);
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      writer.WriteBase64(buffer, index, count);
    }

    public override void WriteCData(string text)
    {
      writer.WriteCData(text);
    }

    public override void WriteCharEntity(char ch)
    {
      writer.WriteCharEntity(ch);
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      writer.WriteChars(buffer, index, count);
    }

    public override void WriteComment(string text)
    {
      writer.WriteComment(text);
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      writer.WriteDocType(name, pubid, sysid, subset);
    }

    public override void WriteEndAttribute()
    {
      writer.WriteEndAttribute();
    }

    public override void WriteEndDocument()
    {
      //writer.WriteEndDocument();
      internalWriteState = WriteState.Start;
    }

    public override void WriteEndElement()
    {
      if (elementCount > 1 || !skipRoot)
        writer.WriteEndElement();
      --elementCount;
    }

    public override void WriteEntityRef(string name)
    {
      writer.WriteEntityRef(name);
    }

    public override void WriteFullEndElement()
    {
      writer.WriteFullEndElement();
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      writer.WriteProcessingInstruction(name, text);
    }

    public override void WriteRaw(string data)
    {
      writer.WriteRaw(data);
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      writer.WriteRaw(buffer, index, count);
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      writer.WriteStartAttribute(prefix, localName, ns);
    }

    public override void WriteStartDocument(bool standalone)
    {
      //writer.WriteStartDocument(standalone);
      internalWriteState = WriteState.Prolog;
    }

    public override void WriteStartDocument()
    {
      //writer.WriteStartDocument();
      internalWriteState = WriteState.Prolog;
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (elementCount > 0 || !skipRoot)
      {
        if (elementCount == 0 && !string.IsNullOrEmpty(rootName))
          localName = rootName;
        writer.WriteStartElement(prefix, localName, ns);
      }
      ++elementCount;
    }

    int elementCount = 0;
    WriteState internalWriteState = WriteState.Start;

    public override WriteState WriteState
    {
      get
      {
        if (elementCount == 0)
          return internalWriteState;
        return writer.WriteState;
      }
    }

    public override void WriteString(string text)
    {
      writer.WriteString(text);
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      writer.WriteSurrogateCharEntity(lowChar, highChar);
    }

    public override void WriteWhitespace(string ws)
    {
      writer.WriteWhitespace(ws);
    }
  }
}
