using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public class ObjectBox : ObjectHeadBox
  {
    public readonly OrderedIndexLink<int> PropertiesByObjectIdWithKind;
    public readonly SimpleIndexLink PropertiesByObjectId;

    public readonly string propertyTableName;

    public ObjectBox(IDataLayer dbConnection,
      string conditionWithoutWhere, params DbParameter[] conditionParameters) :
        this(dbConnection, conditionWithoutWhere, false, conditionParameters)
    {
    }

    public ObjectBox(IDataLayer dbConnection,
      string conditionWithoutWhere,
      bool isStubPrimaryKeyCreator,
      params DbParameter[] conditionParameters) :
      base(dbConnection, conditionWithoutWhere, isStubPrimaryKeyCreator, conditionParameters)
    {
      this.propertyTableName = string.Format("{0}_property", tablePrefix);

      string select = "Select prop_id, obj_id, type_id, prop_index, prop_value From " + propertyTableName;

      DataTable propertiesTable = dbConnection.GetTable(database,
        string.Format("{0} Where {1} order by obj_id, type_id, prop_index asc",
          select, DataCondition.ForEnum("obj_id", ObjectById.Keys))
      );

      propertiesTable.PrimaryKey = new DataColumn[] { propertiesTable.Columns[0] };

      IPrimaryKeyCreator primaryKeyCreator = new StandartPrimaryKeyCreator(dbConnection, database,
        primaryKeyTableName, 10, propertyTableName, PropertyType.PropertyId, isStubPrimaryKeyCreator);

      TableLink propertiesLink = new TableLink(
        new DatabaseTableProvider(dbConnection, database, propertiesTable, select),
        primaryKeyCreator,
        new FieldBlank[] { PropertyType.PropertyId, PropertyType.ObjectId, PropertyType.TypeId,
              PropertyType.PropertyIndex, PropertyType.PropertyValue },
        new IndexBlank[] { PropertyType.PropertyById, PropertyType.PropertiesByObjectIdAndTypeId,
            PropertyType.PropertiesByObjectId });

      this.PropertiesByObjectIdWithKind = new OrderedIndexLink<int>(propertiesLink,
        PropertyType.PropertiesByObjectIdAndTypeId,
        delegate (int propertyKind, object[] basicKeyParts)
        {
          return CreateKeyParts(propertyKind, basicKeyParts);
        }, PropertyType.PropertyIndex);

      this.PropertiesByObjectId = new SimpleIndexLink(propertiesLink, PropertyType.PropertiesByObjectId);
    }

    protected object[] CreateKeyParts(int propertyKind, object[] basicKeyParts)
    {
      object[] keys = new object[basicKeyParts.Length + 1];
      for (int i = 0; i < basicKeyParts.Length; ++i)
        keys[i] = basicKeyParts[i];
      keys[keys.Length - 1] = propertyKind;
      return keys;
    }

    public TableLink PropertyTable
    {
      get { return PropertiesByObjectId.TableLink; }
    }

    public override void Update()
    {
      base.Update();
      PropertiesByObjectId.TableLink.UpdateTable();
    }

    public override long DataChangeTick
    {
      get
      {
        return base.DataChangeTick + PropertyTable.DataChangeTick + PropertyTable.RowListChangeTick;
      }
    }
  }
}
