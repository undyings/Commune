using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public class SQLiteDatabaseHlp
  {
    public static void DeleteParentObject(IDataLayer dbConnection, int objectId)
    {
      string tablePrefix;
      string database = DataBox.GetDatabase(dbConnection, out tablePrefix);

      DeleteChildObject(dbConnection, database, tablePrefix, objectId);

      dbConnection.GetScalar(database,
        string.Format("Delete From {0}_link Where child_id = @objectId",
          tablePrefix),
        new DbParameter("objectId", objectId)
      );
    }

    static void DeleteChildObject(IDataLayer dbConnection, string database, string tablePrefix, int objectId)
    {
      DataTable table = dbConnection.GetTable(database, string.Format(
        "Select child_id From {0}_link Where parent_id = @objectId", tablePrefix),
        new DbParameter("objectId", objectId)
      );

      foreach (DataRow row in table.Rows)
      {
        int? childId = ConvertHlp.ToInt(row[0]);
        if (childId != null)
          DeleteChildObject(dbConnection, database, tablePrefix, childId.Value);
      }

      if (table.Rows.Count > 0)
      {
        dbConnection.GetScalar(database,
          string.Format("Delete From {0}_link Where parent_id = @objectId",
            tablePrefix),
          new DbParameter("objectId", objectId)
        );
      }

      dbConnection.GetScalar("",
        string.Format("Delete From {0}_property Where obj_id = @objectId",
          tablePrefix),
        new DbParameter("objectId", objectId)
      );

      dbConnection.GetScalar("",
        string.Format("Delete From {0}_object Where obj_id = @objectId",
          tablePrefix),
        new DbParameter("objectId", objectId)
      );
    }

    //public static void DeleteObject(IDataLayer dbConnection, int objectId)
    //{
    //  string tablePrefix;
    //  string database = DataBox.GetDatabase(dbConnection, out tablePrefix);

    //  dbConnection.GetScalar(database,
    //    string.Format("Delete From {0}_link Where parent_id = @objectId or child_id = @objectId",
    //      tablePrefix),
    //    new DbParameter("objectId", objectId)
    //  );

    //  dbConnection.GetScalar("",
    //    string.Format("Delete From {0}_property Where obj_id = @objectId",
    //      tablePrefix),
    //    new DbParameter("objectId", objectId)
    //  );

    //  dbConnection.GetScalar("",
    //    string.Format("Delete From {0}_object Where obj_id = @objectId",
    //      tablePrefix),
    //    new DbParameter("objectId", objectId)
    //  );
    //}

    public static bool TableExist(IDataLayer dbConnection, string tableName)
    {
      object rawCount = dbConnection.GetScalar("",
        "Select count(*) From sqlite_master Where name = @tableName",
        new DbParameter("tableName", tableName));

      return Convert.ToInt32(rawCount) != 0;
    }

    public static void CheckAndCreateDataBoxTables(IDataLayer dbConnection)
    {
      string tablePrefix;
      string database = DataBox.GetDatabase(dbConnection, out tablePrefix);

      string objectTableName = tablePrefix + "_object";
      if (!TableExist(dbConnection, objectTableName))
      {
        CreateTableForObjects(dbConnection, objectTableName);
        Logger.AddMessage("Создана таблица объектов {0}", objectTableName);
      }

      string propertyTableName = tablePrefix + "_property";
      if (!TableExist(dbConnection, propertyTableName))
      {
        CreateTableForProperties(dbConnection, propertyTableName);
        Logger.AddMessage("Создана таблица для свойств объектов {0}", propertyTableName);
      }

      string linkTableName = tablePrefix + "_link";
      if (!TableExist(dbConnection, linkTableName))
      {
        CreateTableForLinks(dbConnection, linkTableName);
        Logger.AddMessage("Создана таблица ссылок на объекты {0}", linkTableName);
      }

      string primaryKeyTableName = tablePrefix + "_primary_key";
      if (!TableExist(dbConnection, primaryKeyTableName))
      {
        CreateTableForPrimaryKey(dbConnection, primaryKeyTableName);
        Logger.AddMessage("Создана таблица табличных первичных ключей {0}", primaryKeyTableName);
      }

      Tuple<string, string>[] allTables = new Tuple<string, string>[] {
        _.Tuple(objectTableName, "obj_id"), _.Tuple(propertyTableName, "prop_id"),
        _.Tuple(linkTableName, "link_id")
      };

      foreach (Tuple<string, string> table in allTables)
      {
        if (dbConnection.GetTable("", string.Format(
          @"Select * From {0} Where table_name = '{1}'", primaryKeyTableName, table.Item1)).Rows.Count == 0)
        {
          int? maxId = DatabaseHlp.ConvertToInt(dbConnection.GetScalar("",
            string.Format("Select max({0}) From {1}", table.Item2, table.Item1)));
          dbConnection.GetScalar("", string.Format("Insert Into {0} values ('{1}', {2})",
            primaryKeyTableName, table.Item1, maxId ?? 0));
          Logger.AddMessage("Добавлен max_primary_key для таблицы {0}", table.Item1);
        }
      }
    }

    public static void CreateTableForObjects(IDataLayer dbConnection, string tableName)
    {
      dbConnection.GetScalar("",
        string.Format(
          @"CREATE TABLE {0} (
              obj_id    integer PRIMARY KEY NOT NULL,
              type_id   integer NOT NULL,
              xml_ids   text,
              act_from  datetime,
              act_till  datetime,
              archive   integer
            );

            CREATE INDEX {0}_by_type_act_from
              ON {0}
              (type_id, act_from);

            CREATE INDEX {0}_by_type_act_till
              ON {0}
              (type_id, act_till);

            CREATE INDEX {0}_by_type_xml_attrs
              ON {0}
              (type_id, xml_ids);",
            tableName
          )
        );
    }

    public static void CreateTableForProperties(IDataLayer dbConnection, string tableName)
    {
      dbConnection.GetScalar("",
        string.Format(
          @"CREATE TABLE {0} (
              prop_id     integer PRIMARY KEY NOT NULL,
              obj_id      integer NOT NULL,
              type_id     integer NOT NULL,
              prop_index  integer NOT NULL DEFAULT 0,
              prop_value  text
            );

            CREATE INDEX {0}_by_obj_type_index
              ON {0}
              (obj_id, type_id, prop_index);

            CREATE INDEX {0}_by_type_id
              ON {0}
              (type_id);",
            tableName
          )
        );
    }

    public static void CreateTableForLinks(IDataLayer dbConnection, string tableName)
    {
      dbConnection.GetScalar("",
        string.Format(
          @"CREATE TABLE {0} (
              link_id     integer PRIMARY KEY NOT NULL,
              parent_id   integer NOT NULL,
              type_id     integer NOT NULL,
              link_index  integer NOT NULL,
              child_id    integer NOT NULL,
              act_from    datetime,
              act_till    datetime
            );

            CREATE INDEX {0}_by_child_type_id
              ON {0}
              (child_id, type_id);

            CREATE INDEX {0}_by_parent_type_id
              ON {0}
              (parent_id, type_id, link_index);",
            tableName
          )
        );
    }

    public static void CreateTableForPrimaryKey(IDataLayer dbConnection, string tableName)
    {
      dbConnection.GetScalar("",
        string.Format(
          @"CREATE TABLE {0} (
              table_name  text PRIMARY KEY NOT NULL,
              max_primary_key integer
          );", tableName
        )
      );
    }
  }
}
