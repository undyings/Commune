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
  public class MySqlDataBoxHlp
  {
    public static void CheckAndCreateDataBoxTables(IDataLayer dbConnection, string database)
    {
      string tablePrefix = "light";

      string objectTableName = tablePrefix + "_object";
      if (!MySqlHlp.TableExist(dbConnection, database, objectTableName))
      {
        CreateTableForObjects(dbConnection, database, objectTableName);
        Logger.AddMessage("Создана таблица объектов {0}", objectTableName);
      }

      string propertyTableName = tablePrefix + "_property";
      if (!MySqlHlp.TableExist(dbConnection, database, propertyTableName))
      {
        CreateTableForProperties(dbConnection, database, propertyTableName);
        Logger.AddMessage("Создана таблица для свойств объектов {0}", propertyTableName);
      }

      string linkTableName = tablePrefix + "_link";
      if (!MySqlHlp.TableExist(dbConnection, database, linkTableName))
      {
        CreateTableForLinks(dbConnection, database, linkTableName);
        Logger.AddMessage("Создана таблица ссылок на объекты {0}", linkTableName);
      }

      string primaryKeyTableName = tablePrefix + "_primary_key";
      if (!MySqlHlp.TableExist(dbConnection, database, primaryKeyTableName))
      {
        CreateTableForPrimaryKey(dbConnection, database, primaryKeyTableName);
        Logger.AddMessage("Создана таблица табличных первичных ключей {0}", primaryKeyTableName);
      }

      Tuple<string, string>[] allTables = new Tuple<string, string>[] {
        _.Tuple(objectTableName, "obj_id"), _.Tuple(propertyTableName, "prop_id"),
        _.Tuple(linkTableName, "link_id")
      };

      foreach (Tuple<string, string> table in allTables)
      {
        if (dbConnection.GetTable(database, string.Format(
          @"Select * From {0} Where table_name = '{1}'", primaryKeyTableName, table.Item1)).Rows.Count == 0)
        {
          int? maxId = DatabaseHlp.ConvertToInt(dbConnection.GetScalar(database,
            string.Format("Select max({0}) From {1}", table.Item2, table.Item1)));
          dbConnection.GetScalar(database, string.Format("Insert Into {0} values ('{1}', {2})",
            primaryKeyTableName, table.Item1, maxId ?? 0));
          Logger.AddMessage("Добавлен max_primary_key для таблицы {0}", table.Item1);
        }
      }
    }

    public static void CreateTableForObjects(IDataLayer dbConnection, string database, string tableName)
    {
      dbConnection.GetScalar(database,
        string.Format(@"
          CREATE TABLE `{0}` (
	          `obj_id` INT NOT NULL,
	          `type_id` INT NOT NULL,
	          `xml_ids` CHAR(50) NOT NULL,
	          `act_from` DATETIME NULL,
	          `act_till` DATETIME NULL,
	          `archive` INT NULL,
            PRIMARY KEY(`obj_id`),
            INDEX `{0}_by_type_act_from` (`type_id`, `act_from`),
            INDEX `{0}_by_type_act_till` (`type_id`, `act_till`),
	          INDEX `{0}_by_type_xml_attrs` (`type_id`, `xml_ids`)
          )
          COLLATE = 'cp1251_bin'
          ENGINE = MyISAM
          ROW_FORMAT = FIXED
          ;
        ", tableName)
      );
    }

    public static void CreateTableForProperties(IDataLayer dbConnection, string database, string tableName)
    {
      dbConnection.GetScalar(database,
        string.Format(@"
          CREATE TABLE `{0}` (
            `prop_id` INT NOT NULL,
	          `obj_id` INT NOT NULL,
	          `type_id` INT NOT NULL,
            `prop_index` INT NOT NULL DEFAULT '0',
            `prop_value` TEXT NULL,
            PRIMARY KEY(`prop_id`),
            INDEX `{0}_by_obj_type_index` (`obj_id`, `type_id`, `prop_index`),
            INDEX `{0}_by_type_id` (`type_id`)
          )
          COLLATE = 'cp1251_bin'
          ENGINE = MyISAM
          ROW_FORMAT = DYNAMIC
          ;
        ", tableName)
      );
    }

    public static void CreateTableForLinks(IDataLayer dbConnection, string database, string tableName)
    {
      dbConnection.GetScalar(database,
        string.Format(@"
          CREATE TABLE `{0}` (
            `link_id` INT NOT NULL,
	          `parent_id` INT NOT NULL,
	          `type_id` INT NOT NULL,
            `link_index` INT NOT NULL,
            `child_id` INT NOT NULL,
	          `act_from` DATETIME NULL,
	          `act_till` DATETIME NULL,
            PRIMARY KEY(`link_id`),
            INDEX `{0}_by_child_type_id` (`child_id`, `type_id`),
            INDEX `{0}_by_parent_type_id` (`parent_id`, `type_id`, `link_index`)
          )
          COLLATE = 'cp1251_bin'
          ENGINE = MyISAM
          ROW_FORMAT = FIXED
          ;
        ", tableName)
      );
    }

    public static void CreateTableForPrimaryKey(IDataLayer dbConnection, string database, string tableName)
    {
      dbConnection.GetScalar(database,
        string.Format(@"
          CREATE TABLE `{0}` (
            `table_name` CHAR(20) NOT NULL,
	          `max_primary_key` INT NOT NULL,
            PRIMARY KEY(`table_name`)
          )
          COLLATE = 'cp1251_bin'
          ENGINE = MyISAM
          ROW_FORMAT = FIXED
          ;
        ", tableName)
      );

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
