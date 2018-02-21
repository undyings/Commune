using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public class LightHead
  {
    public readonly ObjectHeadBox headBox;
    public readonly int Id;
    public LightHead(ObjectHeadBox objectBox, int objectId)
    {
      this.headBox = objectBox;
      this.Id = objectId;
    }

    public bool IsKind(int objectKind)
    {
      return headBox.IsKindObject(Id, objectKind);
    }

    //public TField Get<TField>(FieldBlank<TField> property)
    //{
    //  return property.Get(objectBox.ObjectById.AnyRow(Id));
    //}

    public void Set<TField>(FieldBlank<TField> property, TField value)
    {
      property.Set(headBox.ObjectById.AnyRow(Id), value);
    }

    //public TField Get<TField>(XmlUniqueProperty<TField> property)
    //{
    //  return property.Get(objectBox, Id);
    //}

    public void SetWithoutCheck<TField>(XmlUniqueProperty<TField> property, TField value)
    {
      property.SetWithoutCheck(headBox.ObjectById.AnyRow(Id), value);
    }
  }

  public class LightObject : LightHead
  {
    readonly ObjectBox objectBox;
    public LightObject(ObjectBox objectBox, int objectId) :
      this(objectBox, objectId, null)
    {
    }

    public ObjectBox Box
    {
      get { return objectBox; }
    }

    public readonly int? DefaultObjectId;
    public LightObject(ObjectBox objectBox, int objectId, int? defaultObjectId) :
      base(objectBox, objectId)
    {
      this.objectBox = objectBox;
      this.DefaultObjectId = defaultObjectId;
    }

    public int NewPropertyIndex(int propertyKind)
    {
      return DataBox.NewPropertyIndex(objectBox.PropertiesByObjectIdWithKind, propertyKind,
        PropertyType.PropertyIndex, Id);
    }

    RowLink FindOrCreateRow<TField>(IPropertyBlank<int, TField> property, int propertyIndex)
    {
      RowLink row = objectBox.PropertiesByObjectIdWithKind.Row(property, propertyIndex, Id);
      if (row != null)
        return row;

      {
        RowLink newRow = objectBox.PropertiesByObjectIdWithKind.TableLink.NewRow();
        PropertyType.ObjectId.Set(newRow, Id);
        PropertyType.TypeId.Set(newRow, property.Kind);
        PropertyType.PropertyIndex.Set(newRow, propertyIndex);

        TField defaultValue;
        if (DefaultObjectId != null)
        {
          defaultValue = objectBox.PropertiesByObjectIdWithKind.Any(property, DefaultObjectId.Value);
        }
        else
        {
          defaultValue = default(TField);
        }
        property.Field.Set(newRow, defaultValue);
        objectBox.PropertiesByObjectIdWithKind.InsertOver(newRow);
        return newRow;
      }
    }

    //public TField Get<TField>(IPropertyBlank<int, TField> property, int propertyIndex)
    //{
    //  RowLink row = objectBox.PropertiesByObjectIdWithKind.Row(property, propertyIndex, Id);
    //  if (row == null)
    //    return default(TField);
    //  return property.Field.Get(row);
    //}

    //public TField Get<TField>(IPropertyBlank<int, TField> property)
    //{
    //  return Get(property, 0);
    //}

    public void Set<TField>(IPropertyBlank<int, TField> property, int propertyIndex, TField propertyValue)
    {
      RowLink row = FindOrCreateRow(property, propertyIndex);
      property.Field.Set(row, propertyValue);
    }

    public void Set<TField>(IPropertyBlank<int, TField> property, TField propertyValue)
    {
      Set(property, 0, propertyValue);
    }

    public void Create<TField>(IPropertyBlank<int, TField> property)
    {
      FindOrCreateRow(property, 0);
    }

    public RowLink[] AllPropertyRows(IPropertyKindBlank<int> property)
    {
      return objectBox.PropertiesByObjectIdWithKind.Rows(property.Kind, Id);
    }

    public bool RemoveProperty(IPropertyKindBlank<int> property, int propertyIndex)
    {
      return objectBox.PropertiesByObjectIdWithKind.RemoveFromArray(property, propertyIndex, Id);
    }

    public void InsertProperty<TField>(IPropertyBlank<int, TField> property, int propertyIndex, TField propertyValue)
    {
      RowLink newRow = CreatePropertyRow(property, propertyValue);

      objectBox.PropertiesByObjectIdWithKind.InsertInArray(newRow, propertyIndex);
    }

    public void AddProperty<TField>(IPropertyBlank<int, TField> property, TField propertyValue)
    {
      RowLink newRow = CreatePropertyRow(property, propertyValue);

      objectBox.PropertiesByObjectIdWithKind.AddInArray(newRow);
    }

    RowLink CreatePropertyRow<TField>(IPropertyBlank<int, TField> property, TField propertyValue)
    {
      RowLink newRow = objectBox.PropertyTable.NewRow();
      PropertyType.ObjectId.Set(newRow, Id);
      PropertyType.TypeId.Set(newRow, property.Kind);
      property.Field.Set(newRow, propertyValue);

      return newRow;
    }

    public bool SetDefaultValue<TField>(IPropertyBlank<int, TField> property, TField defautValue)
    {
      return SetDefaultValue(property, 0, defautValue);
    }

    public bool SetDefaultValue<TField>(IPropertyBlank<int, TField> property, int propertyIndex, TField defaultValue)
    {
      RowLink row = objectBox.PropertiesByObjectIdWithKind.Row(property, propertyIndex, Id);
      //TraceHlp2.AddMessage("SetDefaultRow: {0}, {1}, {2}", property.Kind, propertyIndex, row != null);
      if (row != null)
        return false;

      Set(property, 0, defaultValue);
      return true;
    }
  }

  public class LightParent : LightObject
  {
    readonly ParentBox objectBox;
    public LightParent(ParentBox objectBox, int objectId) :
      base(objectBox, objectId)
    {
      this.objectBox = objectBox;
    }

    public RowLink[] AllChildRows(LinkKindBlank childLinkKind)
    {
      return objectBox.ChildsByObjectIdWithKind.Rows(childLinkKind.Kind, Id);
    }

    public int? GetChildId(LinkKindBlank childLinkKind, int linkIndex)
    {
      RowLink row = FindChildRow(childLinkKind, linkIndex);
      if (row == null)
        return null;
      return LinkType.ChildId.Get(row);
    }

    public int? GetChildId(LinkKindBlank childLinkKind)
    {
      return GetChildId(childLinkKind, 0);
    }

    RowLink FindChildRow(LinkKindBlank childLinkKind, int linkIndex)
    {
      RowLink[] rows = objectBox.ChildsByObjectIdWithKind.Rows(childLinkKind.Kind, Id);
      int position = _.BinarySearch(rows, linkIndex,
        delegate (RowLink row) { return LinkType.LinkIndex.Get(row); }, Comparer<int>.Default.Compare);
      if (position < 0)
        return null;
      return rows[position];
    }

    public void SetChildId(LinkKindBlank childLinkKind, int linkIndex, int? childId)
    {
      RowLink row = FindChildRow(childLinkKind, linkIndex);
      if (childId == null)
      {
        if (row != null)
          objectBox.ChildByLinkId.TableLink.RemoveRow(row);
        return;
      }
      if (row != null)
      {
        LinkType.ChildId.Set(row, childId.Value);
      }
      else if (row == null)
      {
        RowLink newRow = DataBox.CreateAndFillLinkRow(objectBox.ChildsByObjectIdWithKind.TableLink,
          Id, childLinkKind.Kind, linkIndex, childId.Value);
        objectBox.ChildsByObjectIdWithKind.InsertOver(newRow);
      }
    }

    public void SetChildId(LinkKindBlank childLinkKind, int? childId)
    {
      SetChildId(childLinkKind, 0, childId);
    }


    public int[] AllChildIds(LinkKindBlank childLinkKind)
    {
      List<int> childIds = new List<int>();
      foreach (RowLink child in AllChildRows(childLinkKind))
        childIds.Add(child.Get(LinkType.ChildId));
      return childIds.ToArray();
    }

    public bool RemoveChildLink(LinkKindBlank link, int linkIndex)
    {
      return objectBox.ChildsByObjectIdWithKind.RemoveFromArray(link, linkIndex, Id);
    }

    public int InsertChildLink(LinkKindBlank link, int linkIndex, int childId)
    {
      return InsertChildLink(link, linkIndex, childId, null, null);
    }

    public int InsertChildLink(LinkKindBlank link, int linkIndex, int childId,
      DateTime? actFrom, DateTime? actTill)
    {
      RowLink newRow = CreateLinkRow(link, childId);
      newRow.Set(LinkType.ActFrom, actFrom);
      newRow.Set(LinkType.ActTill, actTill);

      objectBox.ChildsByObjectIdWithKind.InsertInArray(newRow, linkIndex);
      return newRow.Get(LinkType.LinkId);
    }

    public int AddChildLink(LinkKindBlank link, int childId)
    {
      return AddChildLink(link, childId, null, null);
    }

    public int AddChildLink(LinkKindBlank link, int childId, DateTime? actFrom, DateTime? actTill)
    {
      RowLink newRow = CreateLinkRow(link, childId);
      newRow.Set(LinkType.ActFrom, actFrom);
      newRow.Set(LinkType.ActTill, actTill);

      objectBox.ChildsByObjectIdWithKind.AddInArray(newRow);

      return newRow.Get(LinkType.LinkId);
    }

    RowLink CreateLinkRow(LinkKindBlank link, int childId)
    {
      RowLink newRow = objectBox.ChildTable.NewRow();
      newRow.Set(LinkType.ParentId, Id);
      newRow.Set(LinkType.TypeId, link.Kind);
      newRow.Set(LinkType.ChildId, childId);

      return newRow;
    }
  }

  public class LightKin : LightParent
  {
    readonly KinBox objectBox;
    public LightKin(KinBox objectBox, int objectId) :
      base(objectBox, objectId)
    {
      this.objectBox = objectBox;
    }

    public int? GetParentId(LinkKindBlank parentLinkKind, int linkIndex)
    {
      RowLink row = FindParentRow(parentLinkKind, linkIndex);
      if (row == null)
        return null;
      return row.Get(LinkType.ParentId);
    }

    public int? GetParentId(LinkKindBlank parentLinkKind)
    {
      return GetParentId(parentLinkKind, 0);
    }

    public void AddParentId(LinkKindBlank parentLinkKind, int parentId)
    {
      RowLink[] rows = objectBox.ParentsByObjectIdWithKind.Rows(parentLinkKind.Kind, Id);
      int linkIndex = 0;
      if (rows.Length != 0)
        linkIndex = _.Last(rows).Get(LinkType.LinkIndex) + 1;

      SetParentId(parentLinkKind, linkIndex, parentId);
    }

    public void SetParentId(LinkKindBlank parentLinkKind, int linkIndex, int? parentId)
    {
      RowLink row = FindParentRow(parentLinkKind, linkIndex);
      if (parentId == null)
      {
        if (row != null)
          objectBox.ParentTable.RemoveRow(row);
        return;
      }

      if (row != null)
      {
        row.Set(LinkType.ParentId, parentId.Value);
      }
      else if (row == null)
      {
        RowLink newRow = DataBox.CreateAndFillLinkRow(objectBox.ParentsByObjectIdWithKind.TableLink,
          parentId.Value, parentLinkKind.Kind, linkIndex, Id);
        objectBox.ParentsByObjectIdWithKind.InsertOver(newRow);
      }
    }

    public void SetParentId(LinkKindBlank parentLinkKind, int parentId)
    {
      SetParentId(parentLinkKind, 0, parentId);
    }

    public void InsertParentId(LinkKindBlank parentLinkKind, int linkIndex, int parentId)
    {
      RowLink row = DataBox.CreateAndFillLinkRow(objectBox.ParentsByObjectIdWithKind.TableLink,
        parentId, parentLinkKind.Kind, linkIndex, Id);
      objectBox.ParentsByObjectIdWithKind.InsertOver(row);
    }

    public RowLink[] AllParentRows(LinkKindBlank parentLinkKind)
    {
      return objectBox.ParentsByObjectIdWithKind.Rows(parentLinkKind.Kind, Id);
    }

    RowLink FindParentRow(LinkKindBlank parentLinkKind, int linkIndex)
    {
      RowLink[] rows = objectBox.ParentsByObjectIdWithKind.Rows(parentLinkKind.Kind, Id);
      int position = _.BinarySearch(rows, linkIndex,
        delegate (RowLink row) { return LinkType.LinkIndex.Get(row); }, Comparer<int>.Default.Compare);
      if (position < 0)
        return null;
      return rows[position];
    }
  }

}
