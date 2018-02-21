using Commune.Basis;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Data
{
  public class MySqlDataLayer : IDataLayer, IDisposable
  {
    object lockObj = new object();

    readonly string connectionStringWithoutDatabase;
    public MySqlDataLayer(string connectionStringWithoutDatabase)
    {
      this.connectionStringWithoutDatabase = connectionStringWithoutDatabase;
    }

    MySqlConnection dbConnection = null;

    public object GetScalar(string database, string query, params DbParameter[] parameters)
    {
      lock (lockObj)
      {
        try
        {
          ChangeDatabase(database);
          return MySqlHlp.GetScalar(query, dbConnection, parameters);
        }
        catch (MySqlException ex)
        {
          if (IsCrashedTableException(ex.Number))
          {
            RepairCrashedTable(ex.Message);
            return MySqlHlp.GetScalar(query, dbConnection, parameters);
          }
          else
          {
            if (ex.Number != 0)
              Logger.WriteException(ex);
            CloseConnection();
            ChangeDatabase(database);
            return MySqlHlp.GetScalar(query, dbConnection, parameters);
          }
        }
        catch (Exception ex)
        {
          Logger.WriteException(ex);
          CloseConnection();
          ChangeDatabase(database);
          return MySqlHlp.GetScalar(query, dbConnection, parameters);
        }
      }
    }

    //public object GetScalar(string database, string query, params DbParameter[] parameters)
    //{
    //  lock (lockObj)
    //  {
    //    ChangeDatabase(database);
    //    return MySqlHlp.GetScalar(query, dbConnection, parameters);
    //  }
    //}

    public DataTable GetTable(string database, string query, params DbParameter[] parameters)
    {
      lock (lockObj)
      {
        try
        {
          ChangeDatabase(database);
          return MySqlHlp.GetTable(query, dbConnection, parameters);
        }
        catch (MySqlException ex)
        {
          if (IsCrashedTableException(ex.Number))
          {
            RepairCrashedTable(ex.Message);
            return MySqlHlp.GetTable(query, dbConnection, parameters);
          }
          else if (ex.Number != 1062)
          {
            if (ex.Number != 0)
              Logger.WriteException(ex);
            CloseConnection();
            ChangeDatabase(database);
            return MySqlHlp.GetTable(query, dbConnection, parameters);
          }
          throw;
        }
        catch (Exception ex)
        {
          Logger.WriteException(ex);
          CloseConnection();
          ChangeDatabase(database);
          return MySqlHlp.GetTable(query, dbConnection, parameters);
        }
      }
    }

    //public DataTable GetTable(string database, string query, params DbParameter[] parameters)
    //{
    //  lock (lockObj)
    //  {
    //    ChangeDatabase(database);
    //    return MySqlHlp.GetTable(query, dbConnection, parameters);
    //  }
    //}

    public void UpdateTable(string database, string query, DataTable table)
    {
      lock (lockObj)
      {
        try
        {
          ChangeDatabase(database);
          MySqlHlp.UpdateTable(query, dbConnection, table);
        }
        catch (MySqlException ex)
        {
          if (IsCrashedTableException(ex.Number))
          {
            RepairCrashedTable(ex.Message);
            MySqlHlp.UpdateTable(query, dbConnection, table);
          }
          else
          {
            if (ex.Number != 0)
              Logger.WriteException(ex);
            CloseConnection();
            ChangeDatabase(database);
            MySqlHlp.UpdateTable(query, dbConnection, table);
          }
        }
        catch (Exception ex)
        {
          Logger.WriteException(ex);
          CloseConnection();
          ChangeDatabase(database);
          MySqlHlp.UpdateTable(query, dbConnection, table);
        }
      }
    }

    //public void UpdateTable(string database, string query, DataTable table)
    //{
    //  lock (lockObj)
    //  {
    //    ChangeDatabase(database);
    //    MySqlHlp.UpdateTable(query, dbConnection, table);
    //  }
    //}

    public string DbParamPrefix
    {
      get { return "?"; }
    }

    public void Dispose()
    {
      CloseConnection();
    }

    void ChangeDatabase(string dataBase)
    {
      if (dbConnection == null)
      {
        dbConnection = MySqlHlp.OpenConnection(connectionStringWithoutDatabase);
        Logger.AddMessage("Создано подключение к базе данных: {0}, {1}", dataBase, dbConnection.ConnectionTimeout);
      }
      dbConnection.ChangeDatabase(dataBase);
    }

    void CloseConnection()
    {
      try
      {
        MySqlConnection closingConnection;
        if (dbConnection != null)
        {
          Logger.AddMessage("Закрываем подключение к базе данных");
          closingConnection = dbConnection;
          dbConnection = null;
          closingConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteException(new Exception("Ошибка при закрытии подключения к базе данных", ex));
      }
    }

    static string ParseFullTableName(string errorMessage)
    {
      int firstQuote = errorMessage.IndexOf("'");
      if (firstQuote == -1 || firstQuote == errorMessage.Length - 1)
        return null;
      int secondQuote = errorMessage.IndexOf("'", firstQuote + 1);
      if (secondQuote == -1)
        return null;
      string fullTableName = errorMessage.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
      if (fullTableName == "")
        return null;

      return fullTableName;
    }

    static bool IsCrashedTableException(int exceptionNumber)
    {
      return exceptionNumber == 144 || exceptionNumber == 145 || exceptionNumber == 126;
    }

    static string ParseTableName(string errorMessage)
    {
      string tablePath = ParseFullTableName(errorMessage);
      if (tablePath == null)
        return null;
      return Path.GetFileNameWithoutExtension(tablePath);
    }

    void RepairCrashedTable(string errorMessage)
    {
      try
      {
        Logger.AddMessage("Восстанавливаем таблицу после ошибки '{0}'", errorMessage);
        string tableName = ParseTableName(errorMessage);
        if (tableName == null)
        {
          Logger.AddMessage("Не удалось распарсить MySqlException = '{0}'", errorMessage);
          return;
        }
        DataTable table = MySqlHlp.GetTable(string.Format("Repair table {0}", tableName), dbConnection);
        if (table.Rows.Count != 0)
        {
          DataRow resultRow = table.Rows[table.Rows.Count - 1];
          string tbName = ASCIIEncoding.ASCII.GetString((byte[])resultRow["Table"]);
          string op = ASCIIEncoding.ASCII.GetString((byte[])resultRow["Op"]);
          string msgType = ASCIIEncoding.ASCII.GetString((byte[])resultRow["Msg_type"]);
          string repairMessage = ASCIIEncoding.ASCII.GetString((byte[])resultRow["Msg_text"]);
          Logger.AddMessage("Результат восстановления таблицы: '{0}', '{1}', '{2}', '{3}'",
            tbName, op, msgType, repairMessage);
        }
        else
          Logger.AddMessage("Восстановление таблицы завершилось с пустым результатом");
      }
      catch (Exception ex)
      {
        Logger.WriteException(ex);
      }
    }


  }
}
