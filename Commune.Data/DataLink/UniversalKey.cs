using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public struct UniversalKey
  {
    public readonly object[] KeyParts;

    public UniversalKey(params object[] keyParts)
    {
      this.KeyParts = keyParts;
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (object key in KeyParts)
      {
        int keyHashCode = -13890;
        if (key != null)
          keyHashCode = key.GetHashCode();
        hashCode = hashCode ^ keyHashCode;
      }
      return hashCode;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is UniversalKey))
        return false;

      UniversalKey universalKey = (UniversalKey)obj;

      //UniversalKey universalKey = obj as UniversalKey;
      //if (universalKey == null)
      //  return false;

      object[] otherKeyParts = universalKey.KeyParts;
      if (KeyParts.Length != otherKeyParts.Length)
        return false;

      for (int i = 0; i < KeyParts.Length; ++i)
      {
        if (!_.Equals(KeyParts[i], otherKeyParts[i]))
          return false;
      }

      return true;
      //try
      //{
      //  int cmp = _.ArrayComparison(KeyParts, universalKey.KeyParts);
      //  if (cmp == 0)
      //    return true;
      //}
      //catch
      //{
      //  TraceHlp2.AddMessage("ErrorComparison: {0}, {1}", StringHlp2.Join(", ", "'{0}'", KeyParts),
      //    StringHlp2.Join(", ", "'{0}'", universalKey.KeyParts));
      //  foreach (object[] parts in new object[][] { KeyParts, universalKey.KeyParts })
      //  {
      //    foreach (object part in parts)
      //    {
      //      TraceHlp2.AddMessage("Part: {0}, {1}", part, part.GetType());
      //    }
      //  }
      //  throw;
      //}
      //return false;
    }

    public override string ToString()
    {
      return StringHlp.Join(", ", "{0}", KeyParts);
    }
  }

  public class SingleIndexBlank : IndexBlank
  {
    public SingleIndexBlank(string indexName, params string[] indexColumns) :
      base (false, indexName, indexColumns)
    {
    }

    public SingleIndexBlank(string indexName, params FieldBlank[] fieldBlanks) :
      this (indexName, _.ToArray(_.Convert(fieldBlanks, delegate(FieldBlank fieldBlank)
    {
      return fieldBlank.FieldName;
    })))
    {
    }
  }

  public class MultiIndexBlank : IndexBlank
  {
    public MultiIndexBlank(string indexName, params string[] indexColumns)
      :
      base(true, indexName, indexColumns)
    {
    }

    public MultiIndexBlank(string indexName, params FieldBlank[] fieldBlanks) :
      this (indexName, _.ToArray(_.Convert(fieldBlanks, delegate(FieldBlank fieldBlank)
    {
      return fieldBlank.FieldName;
    })))
    {
    }
  }

  public abstract class IndexBlank
  {
    public readonly string IndexName;
    public readonly string[] IndexColumns;
    public readonly bool IsMultiIndex;

    public UniversalKey CreateKey(TableLink tableLink, IRowLink rowLink)
    {
      object[] partKeys = new object[IndexColumns.Length];
      for (int i = 0; i < partKeys.Length; ++i)
      {
        FieldLink fieldLink = tableLink.GetFieldLink(IndexColumns[i]);
        partKeys[i] = fieldLink.FieldBlank.GetValue(rowLink);
      }
      return new UniversalKey(partKeys);
    }

    protected IndexBlank(bool isMultiIndex, string indexName, params string[] indexColumns)
    {
      this.IsMultiIndex = isMultiIndex;
      this.IndexName = indexName;
      this.IndexColumns = indexColumns;
    }

    protected IndexBlank(string indexName, params string[] indexColumns)
      :
      this(false, indexName, indexColumns)
    {
    }
  }
}
