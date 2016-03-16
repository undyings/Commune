using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;
using System.Diagnostics;

namespace Commune.Data
{
  public interface ITableProvider
  {
    DataTable Table { get; }
    void Update();
  }

  public class ReadonlyTableProvider : ITableProvider
  {
    readonly DataTable table;
    public ReadonlyTableProvider(DataTable table)
    {
      this.table = table;
    }

    public DataTable Table
    {
      get { return table; }
    }

    public void Update()
    {
      throw new Exception("ReadonlyTableProvider не поддерживает обновление таблицы");
    }
  }

  public class DatabaseTableProvider : ITableProvider
  {
    readonly IDataLayer dbConnection;
    readonly string database;
    readonly string updateQuery;
    readonly DataTable table;

    public DatabaseTableProvider(IDataLayer dbConnection, string database,
      string select, string conditionWithoutWhere, params DbParameter[] conditionParameters) :
      this(dbConnection, database, dbConnection.GetTable(database, 
        StringHlp.IsEmpty(conditionWithoutWhere) ? select : string.Format("{0} Where {1}", select, conditionWithoutWhere),
        conditionParameters), select)
    {
    }

    public DatabaseTableProvider(IDataLayer dbConnection, string database, DataTable table, string updateQuery)
    {
      this.dbConnection = dbConnection;
      this.database = database;
      this.updateQuery = updateQuery;
      this.table = table;
    }

    public DataTable Table
    {
      get { return table; }
    }

    public void Update()
    {
      dbConnection.UpdateTable(database, updateQuery, table);
    }
  }

  public class SourceTableProvider : ITableProvider
  {
    readonly TableLink sourceTableLink;
    readonly DataTable table;
    public SourceTableProvider(TableLink sourceTableLink, RowLink[] sourceRows)
    {
      this.sourceTableLink = sourceTableLink;
      //this.table = sourceTableLink.Table.Copy();
      this.table = sourceTableLink.Table.Clone();
      foreach (RowLink sourceRow in sourceRows)
      {
        table.ImportRow(sourceRow.DataRow);
      }
    }

     public SourceTableProvider(TableLink sourceTableLink, IndexBlank index, params object[] keyParts) :
      this(sourceTableLink, sourceTableLink.FindRows(index, keyParts))
    {
    }

    public DataTable Table
    {
      get { return table; }
    }

    public void Update()
    {
      DataTable sourceTable = sourceTableLink.Table;

      if (sourceTable.PrimaryKey == null || sourceTable.PrimaryKey.Length == 0)
        throw new Exception("TableLink не может объединять таблицы не имеющие первичного ключа");

      //Stopwatch watch = new Stopwatch();
      //watch.Start();

      //foreach (RowLink rowLink in sourceTableLink.AllRows)
      //  sourceTableLink.RemoveIndexForRow(rowLink);

      sourceTableLink.RemoveAllIndices();

      //watch.Stop();
      //TraceHlp2.AddMessage("RemoveIndex: {0}, {1}, {2}", table.Columns.Count,
      //  sourceTableLink.AllRows.Length, watch.ElapsedMilliseconds);

      //watch.Reset();
      //watch.Start();

      //List<DataRow> addedRows = new List<DataRow>();
      //foreach (DataRow row in sourceTable.Rows)
      //{
      //  if (row.RowState == DataRowState.Added)
      //    addedRows.Add(row);
      //}

      sourceTable.Merge(table);

      //watch.Stop();
      //TraceHlp2.AddMessage("Merge: {0}", watch.ElapsedMilliseconds);

      //watch.Reset();
      //watch.Start();

      //foreach (DataRow row in addedRows)
      //{
      //  if (row.RowState == DataRowState.Modified)
      //  {
      //    row.AcceptChanges();
      //    row.SetAdded();
      //  }
      //}

      table.AcceptChanges();
      //sourceTable.AcceptChanges();

      //watch.Stop();
      //TraceHlp2.AddMessage("AcceptChanges: {0}", watch.ElapsedMilliseconds);

      //watch.Reset();
      //watch.Start();

      foreach (DataRow row in sourceTable.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
          sourceTableLink.CreateIndexForRow(new RowLink(sourceTableLink, row));
      }

      //watch.Stop();
      //TraceHlp2.AddMessage("CreateIndex: {0}, {1}",
      //  sourceTable.Rows.Count, watch.ElapsedMilliseconds);

    }
  }
}
