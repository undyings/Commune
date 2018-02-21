using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public class ObjectHeadBox
  {
    public readonly SimpleIndexLink ObjectById;
    public readonly SimpleIndexLink ObjectByXmlIds;

    public readonly UniqueChecker ObjectUniqueChecker;

    public readonly IDataLayer dbConnection;
    public readonly string database;
    public readonly string tablePrefix;
    public readonly string primaryKeyTableName;
    public readonly string objectTableName;

    public ObjectHeadBox(IDataLayer dbConnection,
      string conditionWithoutWhere, params DbParameter[] conditionParameters) :
        this(dbConnection, conditionWithoutWhere, false, conditionParameters)
    {

    }

    public ObjectHeadBox(IDataLayer dbConnection,
      string conditionWithoutWhere,
      bool isStubPrimaryKeyCreator,
      params DbParameter[] conditionParameters)
    {
      this.dbConnection = dbConnection;

      this.database = DataBox.GetDatabase(dbConnection, out tablePrefix);

      this.primaryKeyTableName = string.Format("{0}_primary_key", tablePrefix);
      this.objectTableName = string.Format("{0}_object", tablePrefix);

      string select = "Select obj_id, type_id, xml_ids, act_from, act_till From " + objectTableName;

      DataTable objectsTable = dbConnection.GetTable(database, 
        string.Format("{0} Where {1}", select, conditionWithoutWhere), conditionParameters);
      objectsTable.PrimaryKey = new DataColumn[] { objectsTable.Columns[0] };


      IPrimaryKeyCreator primaryKeyCreator = new StandartPrimaryKeyCreator(dbConnection, database,
          primaryKeyTableName, 1, objectTableName, ObjectType.ObjectId, isStubPrimaryKeyCreator);

      TableLink objectsLink = new TableLink(
        new DatabaseTableProvider(dbConnection, database, objectsTable, select),
        primaryKeyCreator,
        new FieldBlank[] { ObjectType.ObjectId, ObjectType.TypeId, ObjectType.XmlObjectIds,
          ObjectType.ActFrom, ObjectType.ActTill },
        new IndexBlank[] { ObjectType.ObjectById, ObjectType.ObjectByXmlIds }
      );

      this.ObjectById = new SimpleIndexLink(objectsLink, ObjectType.ObjectById);
      this.ObjectByXmlIds = new SimpleIndexLink(objectsLink, ObjectType.ObjectByXmlIds);

      this.ObjectUniqueChecker = new UniqueChecker(dbConnection, this);
    }

    public RowLink CreateObjectRow(int typeKind, string xmlObjectIds, DateTime? actFrom)
    {
      RowLink newRow = ObjectById.TableLink.NewRow();
      newRow.Set(ObjectType.TypeId, typeKind);
      newRow.Set(ObjectType.XmlObjectIds, xmlObjectIds);
      newRow.Set(ObjectType.ActFrom, actFrom);

      return newRow;
    }

    public int CreateObject(int typeKind, string xmlObjectIds, DateTime? actFrom)
    {
      RowLink newRow = CreateObjectRow(typeKind, xmlObjectIds, actFrom);
      ObjectById.TableLink.AddRow(newRow);

      return newRow.Get(ObjectType.ObjectId);
    }

    public int? CreateUniqueObject(int typeKind, string xmlObjectIds, DateTime? actFrom)
    {
      if (!ObjectUniqueChecker.IsUniqueKey(null, typeKind, xmlObjectIds, actFrom))
        return null;

      return CreateObject(typeKind, xmlObjectIds, actFrom);
    }

    //Extension
    public TableLink ObjectTable
    {
      get { return ObjectById.TableLink; }
    }

    //Extension
    public int[] AllObjectIds
    {
      get { return ObjectType.ObjectId.All(ObjectTable.AllRows); }
    }

    public bool IsKindObject(int objectId, params int[] requiredKinds)
    {
      int typeId = ObjectById.Any(ObjectType.TypeId, objectId);
      return _.Contains(requiredKinds, typeId);
    }

    public virtual void Update()
    {
      ObjectById.TableLink.UpdateTable();
    }

    public virtual long DataChangeTick
    {
      get
      {
        return ObjectById.TableLink.DataChangeTick;
      }
    }

    public long RowListChangeTick
    {
      get { return ObjectById.TableLink.RowListChangeTick; }
    }

  }
}
