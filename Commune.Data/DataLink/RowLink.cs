using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Data;

namespace Commune.Data
{
  public class RowLink : IRowLink
  {
    readonly TableLink table;
    public readonly DataRow DataRow;

    public RowLink(TableLink table, DataRow dataRow)
    {
      this.table = table;
      this.DataRow = dataRow;
    }

    object IRowLink.GetValue(string name)
    {
      FieldLink field = table.GetFieldLink(name);
      if (field == null)
        throw new Exception(string.Format("Не найден FieldLink для поля '{0}'", name));

      return DataRow[field.ColumnIndex];
    }

    // Extension
    public TField Get<TField>(FieldBlank<TField> field)
    {
      return field.Get(this);
    }

    // Extension
    public void Set<TField>(FieldBlank<TField> field, TField value)
    {
      field.Set(this, value);
    }

    void IRowLink.SetValue(string name, object value)
    {
      FieldLink field = table.GetFieldLink(name);
      if (field == null)
        throw new Exception(string.Format("Не найден FieldLink для поля '{0}'", name));

      DataRowState rowState = DataRow.RowState;
      bool isIndexPart = rowState != DataRowState.Detached && field.IsIndicesPart;
      if (isIndexPart)
        table.RemoveIndexForRow(this);
      table.IncrementDataChangeTick();
      DataRow[field.ColumnIndex] = value;
      if (isIndexPart)
        table.CreateIndexForRow(this);
    }

    bool IRowLink.IsContainsValue(string name)
    {
      return table.GetFieldLink(name) != null;
    }

    public void Fill(RowLink sourceRow)
    {
      foreach (FieldLink field in table.Fields)
        field.FieldBlank.Copy(this, sourceRow);
    }
  }

  public class VirtualRowLink : IRowLink
  {
    readonly Dictionary<string, object> fieldByName = new Dictionary<string, object>();

    object IRowLink.GetValue(string name)
    {
      return DictionaryHlp.GetValueOrDefault(fieldByName, name);
    }

    void IRowLink.SetValue(string name, object value)
    {
      dataChangeTick++;
      fieldByName[name] = value;
    }

    // Extension
    public TField Get<TField>(FieldBlank<TField> field)
    {
      return field.Get(this);
    }

    // Extension
    public void Set<TField>(FieldBlank<TField> field, TField value)
    {
      field.Set(this, value);
    }

    bool IRowLink.IsContainsValue(string name)
    {
      return fieldByName.ContainsKey(name);
    }

    long dataChangeTick = 0;
    public long DataChangeTick
    {
      get { return dataChangeTick; }
    }

    public void ResetDataChangeTick()
    {
      dataChangeTick = 0;
    }

    public bool IsModify
    {
      get { return dataChangeTick != 0; }
    }
  }
}
