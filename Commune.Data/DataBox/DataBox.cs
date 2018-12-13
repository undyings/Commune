using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Data;

namespace Commune.Data
{
  public class DataBox
  {
    public static string GetDatabase(IDataLayer dbConnection, out string tablePrefix)
    {
      if (dbConnection is IBoxConnection)
      {
        tablePrefix = ((IBoxConnection)dbConnection).TablePrefix;
        return ((IBoxConnection)dbConnection).Database;
      }

      tablePrefix = "light";
      return "";
    }

    public static ObjectBox LoadOrCreateObjects(IDataLayer dbConnection, int typeId,
      Getter<string, string> xmlIdsGetter, params string[] kinds)
    {
      ObjectBox box = new ObjectBox(dbConnection, DataCondition.ForTypes(typeId));

      bool isCreated = false;
      foreach (string kind in kinds)
      {
        string xmlIds = xmlIdsGetter(kind);
        if (box.ObjectByXmlIds.AnyRow(xmlIds) == null)
        {
          int createId = box.CreateObject(typeId, xmlIds, null);
          isCreated = true;
        }
      }

      if (isCreated)
        box.Update();

      return box;
    }

    public static LightObject LoadOrCreateObject(IDataLayer dbConnection, int typeId, 
      Getter<string, string> xmlIdsGetter, string kind)
    {
      ObjectBox box = LoadOrCreateObjects(dbConnection, typeId, xmlIdsGetter, kind);
      return new LightObject(box, box.AllObjectIds[0]);
    }

    public static LightObject LoadOrCreateObject(IDataLayer dbConnection, int typeId, string xmlIds)
    {
      ObjectBox box = new ObjectBox(dbConnection, string.Format("{0} and xml_ids = @xmlIds",
        DataCondition.ForTypes(typeId)), new DbParameter("xmlIds", xmlIds));

      int[] allObjectIds = box.AllObjectIds;
      if (allObjectIds.Length != 0)
        return new LightObject(box, allObjectIds[0]);

      int objectId = box.CreateObject(typeId, xmlIds, DateTime.UtcNow);
      return new LightObject(box, objectId);
    }


    public static LightObject LoadObject(IDataLayer dbConnection, int typeId, string xmlIds)
    {
      ObjectBox box = new ObjectBox(dbConnection, string.Format("{0} and xml_ids = @xmlIds",
        DataCondition.ForTypes(typeId)), new DbParameter("xmlIds", xmlIds));
      int[] allObjectIds = box.AllObjectIds;
      if (allObjectIds.Length == 0)
        return null;
      return new LightObject(box, allObjectIds[0]);
    }

    public static LightObject LoadObject(IDataLayer dbConnection, int typeId, int objectId)
    {
      ObjectBox box = new ObjectBox(dbConnection, DataCondition.ForTypeObjects(typeId, objectId));
      int[] allObjectIds = box.AllObjectIds;
      if (allObjectIds.Length == 0)
        return null;
      return new LightObject(box, allObjectIds[0]);
    }

    public static LightKin LoadKin(IDataLayer dbConnection, int typeId, int objectId)
    {
      KinBox box = new KinBox(dbConnection, DataCondition.ForTypeObjects(typeId, objectId));
      int[] allObjectIds = box.AllObjectIds;
      if (allObjectIds.Length == 0)
        return null;
      return new LightKin(box, allObjectIds[0]);
    }

    public static RowLink CreateAndFillLinkRow(TableLink linkTableLink, int parentId, int linkTypeId,
      int linkIndex, int childId)
    {
      RowLink newLinkRow = linkTableLink.NewRow();
      LinkType.ParentId.Set(newLinkRow, parentId);
      LinkType.TypeId.Set(newLinkRow, linkTypeId);
      LinkType.LinkIndex.Set(newLinkRow, linkIndex);
      LinkType.ChildId.Set(newLinkRow, childId);
      return newLinkRow;
    }

    public static int NewPropertyIndex<T>(IndexLink<T> propertyLink, T propertyKind,
      FieldBlank<int> indexBlank, int objectId)
    {
      RowLink[] rows = propertyLink.Rows(propertyKind, objectId);
      if (rows.Length != 0)
        return indexBlank.Get(_.Last(rows)) + 1;
      return 0;
    }

    public static void RemoveKinObject(KinBox kinBox, int objectId)
    {
      kinBox.ParentsByObjectId.RemoveRows(objectId);
      RemoveParentObject(kinBox, objectId);
    }

    public static void RemoveParentObject(ParentBox parentBox, int objectId)
    {
      parentBox.ChildsByObjectId.RemoveRows(objectId);
      RemoveVacantObject(parentBox, objectId);
    }

    public static void RemoveVacantObject(ObjectBox objectBox, int objectId)
    {
      objectBox.PropertiesByObjectId.RemoveRows(objectId);
      objectBox.ObjectById.RemoveRows(objectId);
    }

    [Obsolete("Используйте DeleteRecursiveParentObject")]
    public static void DeleteParentObject(IDataLayer dbConnection, int objectId)
    {
      DeleteRecursiveParentObject(dbConnection, objectId);
    }

    public static void DeleteRecursiveParentObject(IDataLayer dbConnection, int objectId)
    {
      string tablePrefix;
      string database = DataBox.GetDatabase(dbConnection, out tablePrefix);

      DeleteRecursiveChildObject(dbConnection, database, tablePrefix, objectId);

      dbConnection.GetScalar(database,
        string.Format("Delete From {0}_link Where child_id = {1}objectId",
          tablePrefix, dbConnection.DbParamPrefix),
        new DbParameter("objectId", objectId)
      );
    }

    static void DeleteRecursiveChildObject(IDataLayer dbConnection, string database, string tablePrefix, int objectId)
    {
      DataTable table = dbConnection.GetTable(database, string.Format(
        "Select child_id From {0}_link Where parent_id = {1}objectId", tablePrefix, dbConnection.DbParamPrefix),
        new DbParameter("objectId", objectId)
      );

      foreach (DataRow row in table.Rows)
      {
        int? childId = ConvertHlp.ToInt(row[0]);
        if (childId != null)
          DeleteRecursiveChildObject(dbConnection, database, tablePrefix, childId.Value);
      }

      if (table.Rows.Count > 0)
      {
        dbConnection.GetScalar(database,
          string.Format("Delete From {0}_link Where parent_id = {1}objectId",
            tablePrefix, dbConnection.DbParamPrefix),
          new DbParameter("objectId", objectId)
        );
      }

      dbConnection.GetScalar(database,
        string.Format("Delete From {0}_property Where obj_id = {1}objectId",
          tablePrefix, dbConnection.DbParamPrefix),
        new DbParameter("objectId", objectId)
      );

      dbConnection.GetScalar(database,
        string.Format("Delete From {0}_object Where obj_id = {1}objectId",
          tablePrefix, dbConnection.DbParamPrefix),
        new DbParameter("objectId", objectId)
      );
    }

    public static RowPropertyBlank<TField> Create<TField>(int propertyKind, FieldBlank<TField> field)
    {
      return new RowPropertyBlank<TField>(propertyKind, field);
    }

    static string propertyFieldName
    {
      get
      {
        return PropertyType.PropertyValue.FieldName;
      }
    }

    public readonly static FieldBlank<string> StringValue = new FieldBlank<string>(propertyFieldName);
    public readonly static FieldBlank<int> IntValue = new FieldBlank<int>(propertyFieldName,
      IntStringConverter.Default, 0);
    public readonly static FieldBlank<long> LongValue = new FieldBlank<long>(propertyFieldName,
      LongStringConverter.Default, 0);
    public readonly static FieldBlank<long?> LongNullableValue = new FieldBlank<long?>(propertyFieldName,
      LongStringConverter.Default, null);
    public readonly static FieldBlank<uint> UIntValue = new FieldBlank<uint>(
      propertyFieldName, UIntStringConverter.Default, 0);
    public readonly static FieldBlank<int?> IntNullableValue = new FieldBlank<int?>(
      propertyFieldName, IntStringConverter.Default, null);
    public readonly static FieldBlank<float> FloatValue = new FieldBlank<float>(propertyFieldName,
      FloatStringConverter.Default, 0);
    public readonly static FieldBlank<float?> FloatNullableValue = new FieldBlank<float?>(propertyFieldName,
      FloatStringConverter.Default, null);
    public readonly static FieldBlank<double> DoubleValue = new FieldBlank<double>(propertyFieldName,
      DoubleStringConverter.Default, 0);
    public readonly static FieldBlank<decimal?> DecimalNullableValue = new FieldBlank<decimal?>(
      propertyFieldName, DecimalStringConverter.Default, null);
    public readonly static FieldBlank<Guid> GuidValue = new FieldBlank<Guid>(propertyFieldName,
      GuidStringConverter.Default, Guid.Empty);
    public readonly static FieldBlank<TimeSpan?> TimeSpanValue = new FieldBlank<TimeSpan?>(propertyFieldName,
      TimeSpanStringConverter.Default, TimeSpan.Zero);
    public readonly static FieldBlank<bool> BoolValue = new FieldBlank<bool>(propertyFieldName,
      BoolStringConverter.Default, false);
    public readonly static FieldBlank<DateTime?> DateTimeNullableValue = new FieldBlank<DateTime?>(
      propertyFieldName, DateTimeStringConverter.Default, null);
    public readonly static FieldBlank<DateTime> DateTimeValue = new FieldBlank<DateTime>(
      propertyFieldName, DateTimeStringConverter.NotNullable, new DateTime(1, 1, 1));
    public readonly static FieldBlank<TimeSpan?> SecondNullableValue = new FieldBlank<TimeSpan?>(
      propertyFieldName, TimeSpanSecondConverter.Nullable, null);
    public readonly static FieldBlank<TimeSpan> SecondValue = new FieldBlank<TimeSpan>(
      propertyFieldName, TimeSpanSecondConverter.NotNullable, TimeSpan.Zero);

  }
}
