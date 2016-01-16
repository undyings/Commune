using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public interface IPrimaryKeyCreator
  {
    void SetPrimaryKey(IRowLink newRow);
  }

  public class StandartPrimaryKeyCreator : IPrimaryKeyCreator
  {
    readonly IDataLayer dbConnection;
    readonly string primaryKeyTableName;
    readonly int reservationStep;
    readonly string tableName;
    readonly FieldBlank<int> primaryKeyField;

    public StandartPrimaryKeyCreator(IDataLayer dbConnection, string primaryKeyTableName, int reservationStep,
      string tableName, FieldBlank<int> primaryKeyField)
    {
      this.dbConnection = dbConnection;
      this.primaryKeyTableName = primaryKeyTableName;
      this.reservationStep = reservationStep;
      this.tableName = tableName;
      this.primaryKeyField = primaryKeyField;
    }

    int maxPrimaryKey = 0;
    int currentPrimaryKey = 0;
    public void SetPrimaryKey(IRowLink newRow)
    {
      if (currentPrimaryKey >= maxPrimaryKey)
      {
        object rawNewPrimaryKey = dbConnection.GetScalar("", string.Format(@"
        update {0} set max_primary_key = max_primary_key + {1} where table_name = '{2}';
        select max_primary_key from {0} where table_name = '{2}';",
          primaryKeyTableName, reservationStep, tableName));
        int? newPrimaryKey = DatabaseHlp.ConvertToInt(rawNewPrimaryKey);
        if (newPrimaryKey == null)
          throw new Exception(string.Format("Ошибка получения нового primary_key для таблицы '{0}'", tableName));
        maxPrimaryKey = newPrimaryKey.Value;
        currentPrimaryKey = maxPrimaryKey - reservationStep + 1;
      }

      primaryKeyField.Set(newRow, currentPrimaryKey);
      currentPrimaryKey++;
    }
  }

  public class MaxPrimaryKeyCreator : IPrimaryKeyCreator
  {
    readonly IDataLayer dbConnection;
    readonly string database;
    readonly string tableName;
    readonly string fieldName;
    readonly FieldBlank<int> primaryKeyField;

    public MaxPrimaryKeyCreator(IDataLayer dbConnection, string database, 
      string tableName, string fieldName, FieldBlank<int> primaryKeyField)
    {
      this.dbConnection = dbConnection;
      this.database = database;
      this.tableName = tableName;
      this.fieldName = fieldName;
      this.primaryKeyField = primaryKeyField;
    }

    int currentPrimaryKey = -1;
    public void SetPrimaryKey(IRowLink newRow)
    {
      if (currentPrimaryKey == -1)
      {
        object rawMaxPrimaryKey = dbConnection.GetScalar(database,
          string.Format("Select Max({0}) From {1}", fieldName, tableName));
        int? maxPrimaryKey = DatabaseHlp.ConvertToInt(rawMaxPrimaryKey);
        currentPrimaryKey = maxPrimaryKey != null ? maxPrimaryKey.Value : 0;
      }

      currentPrimaryKey++;
      primaryKeyField.Set(newRow, currentPrimaryKey);
    }
  }
}
