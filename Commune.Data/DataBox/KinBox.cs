using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public class KinBox : ParentBox
  {
    public readonly SimpleIndexLink ParentsByObjectId;
    public readonly OrderedIndexLink<int> ParentsByObjectIdWithKind;
    public readonly OrderedIndexLink<int> ObjectsByParentIdWithKind;

    public TableLink ParentTable
    {
      get { return ParentsByObjectId.TableLink; }
    }

    public KinBox(IDataLayer dbConnection,
      string conditionWithoutWhere, params DbParameter[] conditionParameters) :
      base(dbConnection, conditionWithoutWhere, conditionParameters)
    {
      string select = "Select link_id, parent_id, type_id, link_index, child_id From " + linkTableName;

      DataTable parentsTable = dbConnection.GetTable(database,
        string.Format("{0} Where {1} order by parent_id, type_id, link_index asc",
          select, DataCondition.ForEnum("child_id", ObjectById.Keys))
      );

      parentsTable.PrimaryKey = new DataColumn[] { parentsTable.Columns[0] };

      TableLink parentLinksLink = new TableLink(
        new DatabaseTableProvider(dbConnection, database, parentsTable, select),
        new StandartPrimaryKeyCreator(dbConnection, database, primaryKeyTableName, 
          5, linkTableName, LinkType.LinkId, false),
        new FieldBlank[] { LinkType.LinkId, LinkType.ParentId, LinkType.TypeId, LinkType.LinkIndex, LinkType.ChildId },
        new IndexBlank[] { LinkType.LinkById, LinkType.LinksByParentIdAndTypeId,
            LinkType.LinksByChildId, LinkType.LinksByChildIdAndTypeId });

      this.ParentsByObjectId = new SimpleIndexLink(parentLinksLink, LinkType.LinksByChildId);
      this.ParentsByObjectIdWithKind = new OrderedIndexLink<int>(parentLinksLink,
        LinkType.LinksByChildIdAndTypeId,
        delegate (int linkKind, object[] basicKeyParts)
        {
          return CreateKeyParts(linkKind, basicKeyParts);
        }, LinkType.LinkIndex);
      this.ObjectsByParentIdWithKind = new OrderedIndexLink<int>(parentLinksLink,
        LinkType.LinksByParentIdAndTypeId,
        delegate (int linkKind, object[] basicKeyParts)
        {
          return CreateKeyParts(linkKind, basicKeyParts);
        }, LinkType.LinkIndex);
    }

    public override void Update()
    {
      base.Update();
      ParentsByObjectId.TableLink.UpdateTable();
    }

    public override long DataChangeTick
    {
      get
      {
        return base.DataChangeTick + ParentTable.DataChangeTick + ParentTable.RowListChangeTick;
      }
    }
  }
}
