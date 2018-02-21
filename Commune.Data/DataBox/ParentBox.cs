using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Commune.Basis;

namespace Commune.Data
{
  public class ParentBox : ObjectBox
  {
    public readonly OrderedIndexLink<int> ChildsByObjectIdWithKind;
    public readonly SimpleIndexLink ChildByLinkId;
    public readonly SimpleIndexLink ChildsByObjectId;
    public readonly OrderedIndexLink<int> ChildsByChildIdWithKind;

    public readonly string linkTableName;

    public ParentBox(IDataLayer dbConnection,
      string conditionWithoutWhere, params DbParameter[] conditionParameters) :
      base(dbConnection, conditionWithoutWhere, conditionParameters)
    {
      this.linkTableName = string.Format("{0}_link", tablePrefix);

      string select = "Select link_id, parent_id, type_id, link_index, child_id, act_from, act_till From " + linkTableName;

      DataTable childsTable = dbConnection.GetTable(database, 
        string.Format("{0} Where {1} order by parent_id, type_id, link_index asc",
          select, DataCondition.ForEnum("parent_id", ObjectById.Keys))
      );

      childsTable.PrimaryKey = new DataColumn[] { childsTable.Columns[0] };

      TableLink childLinksLink = new TableLink(
        new DatabaseTableProvider(dbConnection, database, childsTable, select),
        new StandartPrimaryKeyCreator(dbConnection, database, primaryKeyTableName, 5, linkTableName, LinkType.LinkId, false),
        new FieldBlank[] { LinkType.LinkId, LinkType.ParentId, LinkType.TypeId,
          LinkType.LinkIndex, LinkType.ChildId, LinkType.ActFrom, LinkType.ActTill },
        new IndexBlank[] { LinkType.LinkById, LinkType.LinksByParentIdAndTypeId,
          LinkType.LinksByParentId, LinkType.LinksByChildId, LinkType.LinksByChildIdAndTypeId }
      );

      this.ChildByLinkId = new SimpleIndexLink(childLinksLink, LinkType.LinkById);
      this.ChildsByObjectId = new SimpleIndexLink(childLinksLink, LinkType.LinksByParentId);

      this.ChildsByObjectIdWithKind = new OrderedIndexLink<int>(childLinksLink,
        LinkType.LinksByParentIdAndTypeId,
        delegate (int linkKind, object[] basicKeyParts)
        {
          return ArrayHlp.Merge(basicKeyParts, new object[] { linkKind });
        }, LinkType.LinkIndex);

      this.ChildsByChildIdWithKind = new OrderedIndexLink<int>(childLinksLink,
        LinkType.LinksByChildIdAndTypeId,
        delegate (int linkKind, object[] basicKeyParts)
        {
          return CreateKeyParts(linkKind, basicKeyParts);
        }, LinkType.LinkIndex);
    }

    public TableLink ChildTable
    {
      get { return ChildByLinkId.TableLink; }
    }

    public override void Update()
    {
      base.Update();
      ChildByLinkId.TableLink.UpdateTable();
    }

    public override long DataChangeTick
    {
      get
      {
        return base.DataChangeTick + ChildTable.DataChangeTick + ChildTable.RowListChangeTick;
      }
    }
  }
}
