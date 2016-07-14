using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Globalization;

namespace Commune.Data
{
  public static class DatabaseHlp
  {
    public static void SafeRenameTable(IDataLayer dbConnection, string database, string oldTableName,
      string newTableName)
    {
      SafeDropTable(dbConnection, database, newTableName);
      dbConnection.GetScalar(database, string.Format("Alter table {0} Rename {1}", oldTableName, newTableName));
    }

    public static void SafeDropTable(IDataLayer dbConnection, string database, string tableName)
    {
      dbConnection.GetScalar(database, string.Format("Drop Table if exists {0}", tableName));
    }

    public static int GetTableCount(IDataLayer dbConnection, string database, string tableName)
    {
      return DatabaseHlp.ConvertToInt(dbConnection.GetScalar(database,
        string.Format("Select count(*) From {0}", tableName))) ?? 0;
    }

    public static string CutOffString(string value, int limitCount)
    {
      if (value != null && value.Length > limitCount)
        return value.Substring(0, limitCount);
      return value;
    }

    public static string GetStringEnumCondition(string conditionColumn, System.Collections.IEnumerable conditionIds)
    {
      if (!_.CountMore(conditionIds, 0))
        return "1=0";
      return string.Format("{0} in ({1})", conditionColumn, StringHlp.Join(",", "'{0}'", conditionIds));
    }

    public static string GetNumericalEnumCondition(string conditionColumn, System.Collections.IEnumerable conditionIds)
    {
      if (!_.CountMore(conditionIds, 0))
        return "1=0";
      return string.Format("{0} in ({1})", conditionColumn, StringHlp.Join(",", "{0}", conditionIds));
    }

    public static string DecimalToString(decimal? value)
    {
      if (value == null)
        return null;
      return value.Value.ToString(CultureInfo.InvariantCulture);
    }

    public static int GetNewId(IDataLayer dbConnection, string tableName, string idColumnName)
    {
      object maxObj = dbConnection.GetScalar("", string.Format("Select Max({0}) From {1}", idColumnName, tableName));
      if (maxObj == DBNull.Value)
        return 0;
      return Convert.ToInt32(maxObj) + 1;
    }

    public static int RowCount(IDataLayer dbConnection, string database, string tableName, 
      string conditionWithoutWhere, params DbParameter[] parameters)
    {
      object rawCount = dbConnection.GetScalar(database, string.Format("Select count(*) From {0} Where {1}",
        tableName, conditionWithoutWhere), parameters);
      return Convert.ToInt32(rawCount);
    }

    public static string DatabaseValueAsString(object databaseValue)
    {
      if (databaseValue == null)
        return null;
      string valueAsStr = databaseValue as string;
      if (valueAsStr == null)
      {
        Logger.AddMessage("Значение базы данных '{0}' не является string", databaseValue);
        return null;
      }
      return valueAsStr;
    }

    public static object DoubleToMoney(object valueObj)
    {
      double? value = valueObj as double?;
      if (value != null)
        return Math.Round(value.Value, 2);
      return 0;
    }

    public static object GuidTo32String(object guidObj)
    {
      return ((Guid)guidObj).ToString("N");
      //return Convert.ToBase64String(((Guid)guidObj).ToByteArray());
    }
    public static object GuidFrom32String(object guidObj)
    {
      return ConvertHlp.ToGuid(guidObj);
      //return new Guid(Convert.FromBase64String((string)guidObj));
    }

    public static object FromBoolToEnglishString(object boolObj)
    {
      bool isEnabled = (bool)boolObj;
      if (isEnabled)
        return "yes";
      return "no";
    }

    public static object ToBoolFromEnglishString(object englishStringBool)
    {
      string englishString = (string)englishStringBool;
      if (englishString == "yes")
        return true;
      return false;
    }

    public static object ToString(object valueObj)
    {
      return Convert.ToString(valueObj);
    }

    public static object ToDouble(object doubleObj)
    {
      return ConvertToDouble(doubleObj);
    }

    public static object ToInt(object intObj)
    {
      return ConvertToInt(intObj);
    }

    public static double? ConvertToDouble(object doubleObj)
    {
      if (doubleObj == DBNull.Value)
        return null;
      if (doubleObj == null)
        return null;
      return Convert.ToDouble(doubleObj);
    }

    public static int? ConvertToInt(object intObj)
    {
      if (intObj == DBNull.Value)
        return null;
      if (intObj == null)
        return null;
      return Convert.ToInt32(intObj);
    }
    public static string ConvertToString(object stringObj)
    {
      if (stringObj == DBNull.Value)
        return null;
      if (stringObj == null)
        return null;
      return Convert.ToString(stringObj);
    }
    public static bool? ConvertToBool(object boolObj)
    {
      if (boolObj == DBNull.Value)
        return null;
      if (boolObj == null)
        return null;
      return Convert.ToBoolean(boolObj);
    }
    public static DateTime? ConvertToDateTime(object timeObj)
    {
      if (timeObj == DBNull.Value)
        return null;
      if (timeObj == null)
        return null;
      return (DateTime)timeObj;
    }
  }
}
