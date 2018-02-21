using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace Commune.Data
{
  public class MySqlHlp
  {
    public const string MySql = "mysql";
    public const string InformationSchema = "information_schema";

    public static bool TableExist(IDataLayer dbConnection, string database, string tableName)
    {
      object val = dbConnection.GetScalar(MySqlHlp.InformationSchema,
        "Select count(*) From Columns Where table_schema = ?database and table_name = ?tableName",
        new DbParameter("database", database), new DbParameter("tableName", tableName));
      return Convert.ToInt32(val) > 0;
    }

    public static bool DatabaseExist(IDataLayer dbConnection, string database)
    {
      DataTable table = dbConnection.GetTable("mysql",
        string.Format("SHOW DATABASES LIKE '{0}';", database));
      return table.Rows.Count != 0;
    }

    public static void CreateDatabase(IDataLayer dbConnection, string database)
    {
      dbConnection.GetScalar("mysql", string.Format("CREATE DATABASE IF NOT EXISTS {0};", database));
    }

    public static MySqlCommand CreateCommand(MySqlConnection connection, MySqlTransaction transaction)
    {
      MySqlCommand command = connection.CreateCommand();
      command.Transaction = transaction;
      return command;
    }

    public static MySqlCommand CreateCommand(MySqlTransaction transaction)
    {
      MySqlCommand command = new MySqlCommand();
      command.Transaction = transaction;
      return command;
    }

    public static string GetDbConnectionString(string connectionStringWithoutDatabase, string database)
    {
      return string.Format("{0};DATABASE={1}", connectionStringWithoutDatabase, database);
    }

    public static MySqlConnection OpenConnection(string connectionString)
    {
      MySqlConnection con = new MySqlConnection(connectionString);
      con.Open();
      return con;
    }
    public static DataTable GetTableFromAdapter(MySqlDataAdapter data)
    {
      DataTable table = new DataTable();
      data.Fill(table);
      return table;
    }
    public static DataTable GetTableWithSchemaFromAdapter(MySqlDataAdapter data)
    {
      DataTable table = new DataTable();
      data.FillSchema(table, SchemaType.Source);
      data.Fill(table);
      return table;
    }
    public static DataTable GetTable(string query, string connection, params DbParameter[] parameters)
    {
      using (MySqlDataAdapter data = new MySqlDataAdapter(query, connection))
      {
        foreach (DbParameter parameter in parameters)
          data.SelectCommand.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        return MySqlHlp.GetTableFromAdapter(data);
      }
    }

    public static DataTable GetTable(string query, MySqlConnection dbConnection, params DbParameter[] parameters)
    {
      MySqlDataAdapter data = new MySqlDataAdapter(query, dbConnection);
      {
        foreach (DbParameter parameter in parameters)
          data.SelectCommand.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        return MySqlHlp.GetTableFromAdapter(data);
      }
    }

    public static DataTable GetTableWithScheme(string query, string connection, params DbParameter[] parameters)
    {
      using (MySqlDataAdapter data = new MySqlDataAdapter(query, connection))
      {
        foreach (DbParameter parameter in parameters)
          data.SelectCommand.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        return MySqlHlp.GetTableWithSchemaFromAdapter(data);
      }
    }

    public static void UpdateTable(string query, string connectString, DataTable table)
    {
      using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connectString))
      {
        using (new MySqlCommandBuilder(adapter))
        {
          adapter.Update(table);
        }
      }
    }

    public static void UpdateTable(string query, MySqlConnection dbConnection, DataTable table)
    {
      MySqlDataAdapter adapter = new MySqlDataAdapter(query, dbConnection);
      {
        using (new MySqlCommandBuilder(adapter))
        {
          adapter.Update(table);
        }
      }
    }

    public static object GetScalar(string query, string connection, params DbParameter[] parameters)
    {
      using (MySqlConnection con = OpenConnection(connection))
      using (MySqlCommand cmd = new MySqlCommand(query, con))
      {
        foreach (DbParameter parameter in parameters)
        {
          cmd.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        }
        return cmd.ExecuteScalar();
      }
    }

    public static object GetScalar(string query, MySqlConnection dbConnection, params DbParameter[] parameters)
    {
      MySqlCommand cmd = new MySqlCommand(query, dbConnection);
      {
        foreach (DbParameter parameter in parameters)
        {
          cmd.Parameters.Add(new MySqlParameter(parameter.Name, parameter.Value));
        }
        return cmd.ExecuteScalar();
      }
    }

    public static void ExecuteCommand(MySqlConnection connection, MySqlCommand command)
    {
      command.Connection = connection;
      command.ExecuteNonQuery();
    }
  }
}
