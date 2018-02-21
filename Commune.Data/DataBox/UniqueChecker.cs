using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public class UniqueChecker
  {
    readonly IDataLayer dbConnection;
    readonly ObjectHeadBox objectBox;
    readonly int[] allLoadObjectIds;
    public UniqueChecker(IDataLayer dbConnection, ObjectHeadBox objectBox)
    {
      this.dbConnection = dbConnection;
      this.objectBox = objectBox;
      this.allLoadObjectIds = objectBox.AllObjectIds;
    }

    //public bool IsUniqueObject(TrafficObject obj)
    //{
    //  return IsUniqueKey(obj.Get(TrafficCard.ObjectId), obj.Get(TrafficCard.TypeId),
    //    obj.Get(TrafficCard.XmlObjectIds), obj.Get(TrafficCard.ActFrom));
    //}

    public bool IsUniqueRow(RowLink row)
    {
      return IsUniqueKey(row.Get(ObjectType.ObjectId), row.Get(ObjectType.TypeId),
        row.Get(ObjectType.XmlObjectIds), row.Get(ObjectType.ActFrom));
    }

    public bool IsUniqueKey(int? objectId, int typeId, string xmlIds, DateTime? actFrom)
    {
      bool isTypeWithUniqueActFrom = actFrom != null;
      foreach (RowLink row in objectBox.ObjectByXmlIds.Rows(xmlIds))
      {
        if (row.Get(ObjectType.ObjectId) == objectId)
          continue;
        if (row.Get(ObjectType.TypeId) != typeId)
          continue;
        if (isTypeWithUniqueActFrom && row.Get(ObjectType.ActFrom) != actFrom)
          continue;
        return false;
      }

      List<DbParameter> dbParams = new List<DbParameter>();
      dbParams.Add(new DbParameter("typeId", typeId));
      dbParams.Add(new DbParameter("xmlIds", xmlIds));
      string actFromCondition = "1=1";
      if (isTypeWithUniqueActFrom)
      {
        object rawActFrom = actFrom.Value;
        if (ObjectType.ActFrom.ToDataBaseValue != null)
          rawActFrom = ObjectType.ActFrom.ToDataBaseValue(actFrom.Value);
        actFromCondition = string.Format("act_from = {0}actFrom", dbConnection.DbParamPrefix);
        dbParams.Add(new DbParameter("actFrom", rawActFrom));
      }

      //string primaryCondition = string.Format("obj_id not in ({0})",
      //  StringHlp2.Join(",", "{0}", TrafficCard.ObjectId.All(objectBox.ObjectById.TableLink.AllRows)));

      string primaryCondition = "";
      if (allLoadObjectIds.Length != 0)
      {
        primaryCondition = string.Format(" and obj_id not in ({0})",
          StringHlp.Join(",", "{0}", allLoadObjectIds));
      }

      object rawCount = dbConnection.GetScalar(objectBox.database, string.Format(
        "Select count(*) From {0} Where type_id = {3}typeId and xml_ids = {3}xmlIds and {1}{2}",
        objectBox.objectTableName, actFromCondition, primaryCondition, dbConnection.DbParamPrefix),
        dbParams.ToArray()
      );
      if (rawCount == DBNull.Value || Convert.ToInt32(rawCount) == 0)
        return true;
      return false;
    }
  }
}
