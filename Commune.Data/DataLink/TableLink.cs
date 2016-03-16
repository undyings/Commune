using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;
using Commune.Diagnostics;

namespace Commune.Data
{
  public class TableLink
  {
    public static TableLink Load(IDataLayer dbConnection, IPrimaryKeyCreator primaryKeyCreator,
      FieldBlank[] fields, IndexBlank[] indicies,
      string database, string select, string conditionWithoutWhere, params DbParameter[] conditionParameters)
    {
      TableLink link = new TableLink(
        new DatabaseTableProvider(dbConnection, database, select, conditionWithoutWhere, conditionParameters),
        primaryKeyCreator, fields, indicies, new LinkConstraint[0]);

      if (primaryKeyCreator != null)
        link.Table.PrimaryKey = new DataColumn[] { link.Table.Columns[0] };

      return link;
    }

    public static TableLink Load(IDataLayer dbConnection, FieldBlank[] fields, IndexBlank[] indices,
      string database, string select, string conditionWithoutWhere, params DbParameter[] conditionParameters)
    {
      return new TableLink(
        new ReadonlyTableProvider(
          dbConnection.GetTable(database,
            StringHlp.IsEmpty(conditionWithoutWhere) ? select : string.Format("{0} Where {1}", select, conditionWithoutWhere),
            conditionParameters
          )
        ), null, fields, indices
      );
    }

    public DataTable Table
    {
      get { return tableProvider.Table; }
    }

    readonly IPrimaryKeyCreator primaryKeyCreator;
    readonly Dictionary<string, FieldLink> fieldsByName = new Dictionary<string, FieldLink>();
    readonly IndexBlank[] indicies;
    readonly LinkConstraint[] constraints;

    public FieldLink[] Fields
    {
      get { return _.ToArray(fieldsByName.Values); }
    }

    readonly Dictionary<string, Dictionary<UniversalKey, RowLink>> singleIndicesByName =
      new Dictionary<string, Dictionary<UniversalKey, RowLink>>();
    readonly Dictionary<string, Dictionary<UniversalKey, List<RowLink>>> multiIndicesByName =
      new Dictionary<string, Dictionary<UniversalKey, List<RowLink>>>();

    readonly ITableProvider tableProvider;
    public TableLink(ITableProvider tableProvider, IPrimaryKeyCreator primaryKeyCreator,
      FieldBlank[] fields, IndexBlank[] indicies, params LinkConstraint[] constraints)
    {
      this.tableProvider = tableProvider;

      if (tableProvider.Table.Columns.Count != fields.Length)
        throw new Exception("Ошибка создания TableLink: число колонок таблицы не равно числу полей");

      this.primaryKeyCreator = primaryKeyCreator;
      this.indicies = indicies;
      this.constraints = constraints;

      Dictionary<string, bool> isIndicesPartByName = new Dictionary<string, bool>();
      foreach (IndexBlank index in indicies)
      {
        if (index.IsMultiIndex)
          multiIndicesByName[index.IndexName] = new Dictionary<UniversalKey, List<RowLink>>();
        else
          singleIndicesByName[index.IndexName] = new Dictionary<UniversalKey, RowLink>();

        foreach (string fieldName in index.IndexColumns)
          isIndicesPartByName[fieldName] = true;
          //fieldsByName[fieldName].IsIndicesPart = true;
      }

      int columnIndex = 0;
      foreach (FieldBlank fieldBlank in fields)
      {
        fieldsByName[fieldBlank.FieldName] = new FieldLink(fieldBlank, columnIndex, 
          DictionaryHlp.GetValueOrDefault(isIndicesPartByName, fieldBlank.FieldName));
        columnIndex++;
      }

      foreach (DataRow dataRow in tableProvider.Table.Rows)
      {
        if (dataRow.RowState != DataRowState.Deleted)
          CreateIndexForRow(new RowLink(this, dataRow));
      }
    }

    public TableLink Clone(RowLink[] sourceRows)
    {
      List<FieldBlank> fieldBlanks = new List<FieldBlank>();
      foreach (FieldLink fieldLink in Fields)
        fieldBlanks.Add(fieldLink.FieldBlank);

      return new TableLink(new SourceTableProvider(this, sourceRows),
        primaryKeyCreator, fieldBlanks.ToArray(), indicies, constraints);
    }

    public FieldLink GetFieldLink(string fieldName)
    {
      FieldLink fieldLink;
      if (fieldsByName.TryGetValue(fieldName, out fieldLink))
        return fieldLink;
      return null;
      //return DictionaryHlp.GetValueOrDefault(fieldsByName, fieldName);
    }

    public RowLink[] AllRows
    {
      get
      {
        foreach (Dictionary<UniversalKey, RowLink> index in singleIndicesByName.Values)
          return _.ToArray<RowLink>(index.Values);

        foreach (Dictionary<UniversalKey, List<RowLink>> index in multiIndicesByName.Values)
        {
          List<RowLink> allRows = new List<RowLink>();
          foreach (List<RowLink> rows in index.Values)
            allRows.AddRange(rows);
          return allRows.ToArray();
        }

        return new RowLink[0];
      }
    }

    public ICollection<UniversalKey> KeysForIndex(IndexBlank index)
    {
      if (index.IsMultiIndex)
        return multiIndicesByName[index.IndexName].Keys;
      else
        return singleIndicesByName[index.IndexName].Keys;
    }

    void PrepareDataTableForUpdate()
    {
      if (constraints.Length == 0)
        return;

      foreach (RowLink rowLink in AllRows)
        foreach (LinkConstraint constraint in constraints)
        {
          if (!constraint.IsValidRow(rowLink))
          {
            rowLink.DataRow.Delete();
            rowLink.DataRow.RowError = "Error";
            RemoveIndexForRow(rowLink);
            break;
          }
        }
    }

    public void UpdateTable()
    {
      PrepareDataTableForUpdate();
      tableProvider.Update();
    }

    public void CreateIndexForRow(RowLink row)
    {
      foreach (IndexBlank index in indicies)
      {
        UniversalKey key = index.CreateKey(this, row);
        if (index.IsMultiIndex)
        {
          Dictionary<UniversalKey, List<RowLink>> rowsForKey = multiIndicesByName[index.IndexName];
          List<RowLink> rows;
          if (!rowsForKey.TryGetValue(key, out rows))
          {
            rows = new List<RowLink>();
            rowsForKey[key] = rows;
          }
          rows.Add(row);
        }
        else
        {
          Dictionary<UniversalKey, RowLink> singleIndex = singleIndicesByName[index.IndexName];
          if (singleIndex.ContainsKey(key))
            Logger.AddMessage("Дубль значения '{0}' по индексу '{1}'", key, index.IndexName);
          singleIndex[key] = row;
        }
      }
    }

    internal void RemoveAllIndices()
    {
      foreach (IndexBlank index in indicies)
      {
        if (index.IsMultiIndex)
          multiIndicesByName[index.IndexName].Clear();
        else
          singleIndicesByName[index.IndexName].Clear();
      }
    }

    public void RemoveIndexForRow(RowLink row)
    {
      foreach (IndexBlank index in indicies)
      {
        UniversalKey key = index.CreateKey(this, row);
        if (index.IsMultiIndex)
          multiIndicesByName[index.IndexName][key].Remove(row);
        else
          singleIndicesByName[index.IndexName].Remove(key);
      }
    }

    public RowLink FindRow(SingleIndexBlank index, params object[] keyParts)
    {
      RowLink row;
      if (singleIndicesByName[index.IndexName].TryGetValue(new UniversalKey(keyParts), out row))
        return row;
      return null;
    }

    public RowLink FindRow(MultiIndexBlank index, params object[] keyParts)
    {
      RowLink[] rows = FindRows(index, keyParts);
      return rows.Length != 0 ? rows[0] : null;
    }

    public RowLink[] FindRows(MultiIndexBlank index, params object[] keyParts)
    {
      List<RowLink> rows;
      if (multiIndicesByName[index.IndexName].TryGetValue(new UniversalKey(keyParts), out rows))
        return rows.ToArray();
      return new RowLink[0];
    }

    public RowLink[] FindRows(IndexBlank index, params object[] keyParts)
    {
      if (index.IsMultiIndex)
        return FindRows((MultiIndexBlank)index, keyParts);

      RowLink row = FindRow((SingleIndexBlank)index, keyParts);
      return row != null ? new RowLink[] { row } : new RowLink[0];
    }

    public RowLink NewRow()
    {
      DataRow dataRow = tableProvider.Table.NewRow();
      RowLink row = new RowLink(this, dataRow);

      foreach (FieldLink field in fieldsByName.Values)
        field.FieldBlank.SetDefaultValue(row);

      if (primaryKeyCreator != null)
        primaryKeyCreator.SetPrimaryKey(row);

      return row;
    }

    long rowListChangeTick = 0;
    public long RowListChangeTick
    {
      get { return rowListChangeTick; }
    }
    long dataChangeTick = 0;
    public long DataChangeTick
    {
      get { return dataChangeTick; }
    }

    public void IncrementDataChangeTick()
    {
      dataChangeTick++;
    }

    public void AddRow(RowLink row)
    {
      rowListChangeTick++;
      tableProvider.Table.Rows.Add(row.DataRow);
      CreateIndexForRow(row);
    }

    public void RemoveRow(RowLink row)
    {
      rowListChangeTick++;
      RemoveIndexForRow(row);
      row.DataRow.Delete();
    }
  }
}
