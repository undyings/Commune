using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public class SimpleIndexLink : IndexLink<Nothing>
  {
    static object[] SimpleKeyCreator(Nothing kind, object[] basicKeyParts)
    {
      return basicKeyParts;
    }

    public SimpleIndexLink(TableLink tableLink, IndexBlank indexBlank) :
      base(tableLink, indexBlank, SimpleKeyCreator)
    {
    }

    public RowLink AnyRow(params object[] basicKeyParts)
    {
      foreach (RowLink row in base.Rows(null, basicKeyParts))
        return row;
      return null;
    }

    public RowLink[] Rows(params object[] basicKeyParts)
    {
      return base.Rows(null, basicKeyParts);
    }

    public bool Exist(params object[] basicKeyParts)
    {
      return base.Exist(null, basicKeyParts);
    }

    public int RemoveRows(params object[] basicKeyParts)
    {
      return base.RemoveRows(null, basicKeyParts);
    }
  }

  public class OrderedIndexLink<TKind> : IndexLink<TKind>
  {
    readonly FieldBlank<int> orderField;
    public OrderedIndexLink(TableLink tableLink, IndexBlank indexBlank,
      Getter<object[], TKind, object[]> keyForPropertyKindCreator, FieldBlank<int> orderField) :
      base(tableLink, indexBlank, keyForPropertyKindCreator)
    {
      this.orderField = orderField;
    }

    public TField Get<TField>(IPropertyBlank<TKind, TField> property, int orderIndex, params object[] basicKeyParts)
    {
      RowLink row = Row(property, orderIndex, basicKeyParts);
      if (row == null)
        return default(TField);
      return property.Field.Get(row);
    }

    public bool Set<TField>(IPropertyBlank<TKind, TField> property, int orderIndex, TField value, params object[] basicKeyParts)
    {
      RowLink row = Row(property, orderIndex, basicKeyParts);
      if (row == null)
        return false;
      property.Field.Set(row, value);
      return true;
    }

    public RowLink Row(IPropertyKindBlank<TKind> propertyKind, int orderIndex, params object[] basicKeyParts)
    {
      RowLink[] rows = Rows(propertyKind.Kind, basicKeyParts);
      if (rows.Length == 0)
        return null;
      if (rows.Length == 1)
      {
        if (orderField.Get(rows[0]) == orderIndex)
          return rows[0];
        return null;
      }
      int position = _.BinarySearch(rows, orderIndex, 
        delegate(RowLink row) { return orderField.Get(row); }, Comparer<int>.Default.Compare);
      if (position < 0)
        return null;
      return rows[position];
    }

    public override RowLink[] Rows(TKind propertyKind, params object[] basicKeyParts)
    {
      RowLink[] rows = base.Rows(propertyKind, basicKeyParts);
      ArrayHlp.Sort(rows, delegate(RowLink row) { return orderField.Get(row); });
      return rows;
    }

    public void InsertInArray(RowLink insertItem, int insertIndex)
    {
      RowLink[] rows = TableLink.FindRows(IndexBlank, IndexBlank.CreateKey(TableLink, insertItem).KeyParts);
      ArrayHlp.Sort(rows, delegate(RowLink row) { return orderField.Get(row); });
      InsertInArray(rows, insertItem, insertIndex);
    }

    void InsertInArray(RowLink[] rows, RowLink insertArrayItem, int insertIndex)
    {
      if (insertIndex > rows.Length)
        insertIndex = rows.Length;

      insertArrayItem.Set(orderField, insertIndex);

      bool isLeftConflict = false;
      if (insertIndex > 0)
        isLeftConflict = (rows[insertIndex - 1].Get(orderField) != insertIndex - 1);
      bool isRightConflict = insertIndex < rows.Length;

      if (isLeftConflict)
      {
        for (int i = 0; i < insertIndex; ++i)
          TableLink.RemoveIndexForRow(rows[i]);
      }
      if (isRightConflict)
      {
        for (int i = insertIndex; i < rows.Length; ++i)
          TableLink.RemoveIndexForRow(rows[i]);
      }

      if (isLeftConflict)
      {
        for (int i = 0; i < insertIndex; ++i)
        {
          rows[i].Set(orderField, i);
          TableLink.CreateIndexForRow(rows[i]);
        }
      }

      TableLink.AddRow(insertArrayItem);

      if (isRightConflict)
      {
        for (int i = insertIndex; i < rows.Length; ++i)
        {
          rows[i].Set(orderField, i + 1);
          TableLink.CreateIndexForRow(rows[i]);
        }
      }
    }

    //public void InsertOver(RowLink insertRowLink)
    //{
    //  RowLink[] rows = TableLink.FindRows(IndexBlank, IndexBlank.CreateKey(TableLink, insertRowLink).KeyParts);
    //  int linkIndex = insertRowLink.Get(orderField);
    //  int insertPosition = _.BinarySearch(rows, linkIndex, delegate(RowLink row)
    //    { return row.Get(orderField); }, Comparer<int>.Default.Compare);
    //  if (insertPosition < 0)
    //    insertPosition = ~insertPosition;

    //  insertRowLink.Set(orderField, insertPosition);
    //  InsertInArray(insertRowLink);
    //}

    [Obsolete("Использовать только для последовательностей, в которых linkIndex может не совпадать индексом последовательности")]
    public void InsertOver(RowLink insertRowLink)
    {
      RowLink[] rows = TableLink.FindRows(IndexBlank, IndexBlank.CreateKey(TableLink, insertRowLink).KeyParts);

      int insertPosition = _.BinarySearch(rows, insertRowLink, delegate(RowLink row1, RowLink row2)
      {
        return Comparer<int>.Default.Compare(orderField.Get(row1), orderField.Get(row2));
      });

      bool notConflict = false;
      if (insertPosition < 0)
      {
        notConflict = true;
        insertPosition = ~insertPosition;
      }

      for (int i = insertPosition; i < rows.Length; ++i)
      {
        TableLink.RemoveIndexForRow(rows[i]);
      }

      int orderIndex = orderField.Get(insertRowLink);
      TableLink.AddRow(insertRowLink);
      for (int i = insertPosition; i < rows.Length; ++i)
      {
        orderIndex++;
        if (!notConflict)
          orderField.Set(rows[i], orderIndex);
        TableLink.CreateIndexForRow(rows[i]);
      }
    }

    //public void InsertInArray(RowLink insertRowLink)
    //{
    //  InsertOver(insertRowLink);
    //}

    public void AddInArray(RowLink addArrayItem)
    {
      RowLink[] rows = TableLink.FindRows(IndexBlank, IndexBlank.CreateKey(TableLink, addArrayItem).KeyParts);
      InsertInArray(rows, addArrayItem, rows.Length);
    }

    //public void AddInArray(RowLink addRowLink)
    //{
    //  RowLink[] rows = TableLink.FindRows(IndexBlank, IndexBlank.CreateKey(TableLink, addRowLink).KeyParts);
    //  int orderIndex = rows.Length != 0 ? (rows[rows.Length - 1].Get(orderField) + 1) : 0;
    //  addRowLink.Set(orderField, orderIndex);
    //  InsertInArray(addRowLink);
    //}

    public bool RemoveFromArray(IPropertyKindBlank<TKind> propertyKind, int removeArrayIndex,
      params object[] basicKeyParts)
    {
      RowLink[] rows = Rows(propertyKind.Kind, basicKeyParts);

      if (removeArrayIndex >= rows.Length)
        return false;

      TableLink.RemoveRow(rows[removeArrayIndex]);

      for (int i = removeArrayIndex + 1; i < rows.Length; ++i)
        TableLink.RemoveIndexForRow(rows[i]);

      for (int i = removeArrayIndex + 1; i < rows.Length; ++i)
      {
        rows[i].Set(orderField, i - 1);
        TableLink.CreateIndexForRow(rows[i]);
      }
      return true;
    }

    //public bool RemoveFromArray(IPropertyKindBlank<TKind> propertyKind, int orderIndex, params object[] basicKeyParts)
    //{
    //  RowLink[] rows = Rows(propertyKind.Kind, basicKeyParts);

    //  int position = _.BinarySearch(rows, orderIndex,
    //    delegate(RowLink row) { return orderField.Get(row); }, Comparer<int>.Default.Compare);

    //  if (position < 0)
    //    return false;

    //  TableLink.RemoveRow(rows[position]);

    //  for (int i = position + 1; i < rows.Length; ++i)
    //    TableLink.RemoveIndexForRow(rows[i]);

    //  for (int i = position + 1; i < rows.Length; ++i)
    //  {
    //    orderField.Set(rows[i], orderIndex);
    //    TableLink.CreateIndexForRow(rows[i]);
    //    orderIndex++;
    //  }
    //  return true;
    //}
  }

  public class IndexLink<TKind>
  {
    public readonly TableLink TableLink;
    public readonly IndexBlank IndexBlank;
    readonly Getter<object[], TKind, object[]> keyForPropertyKindCreator;

    public IndexLink(TableLink tableLink, IndexBlank indexBlank, 
      Getter<object[], TKind, object[]> keyForPropertyKindCreator)
    {
      this.TableLink = tableLink;
      this.IndexBlank = indexBlank;
      this.keyForPropertyKindCreator = keyForPropertyKindCreator;
    }

    public virtual RowLink[] Rows(TKind propertyKind, params object[] basicKeyParts)
    {
      return TableLink.FindRows(IndexBlank, keyForPropertyKindCreator(propertyKind, basicKeyParts));
    }

    public ICollection<UniversalKey> Keys
    {
      get { return TableLink.KeysForIndex(IndexBlank); }
    }

    public bool Exist(TKind propertyKind, params object[] basicKeyParts)
    {
      return Rows(propertyKind, basicKeyParts).Length != 0;
    }

    public TField Any<TField>(IPropertyBlank<TKind, TField> property, params object[] basicKeyParts)
    {
      return property.Field.Any(Rows(property.Kind, basicKeyParts));
    }

    public TField[] All<TField>(IPropertyBlank<TKind, TField> property, params object[] basicKeyParts)
    {
      return property.Field.All(Rows(property.Kind, basicKeyParts));
    }

    //public void Set<TField>(IPropertyBlank<TKind, TField> property, TField value, params object[] basicKeyParts)
    //{
    //  foreach (IRowLink row in Rows(property.Kind, basicKeyParts))
    //    property.Field.Set(row, value);
    //}

    public int RemoveRows(TKind propertyKind, params object[] basicKeyParts)
    {
      RowLink[] removeRows = Rows(propertyKind, basicKeyParts);
      foreach (RowLink row in removeRows)
        TableLink.RemoveRow(row);
      return removeRows.Length;
    }

    //public void InsertOver(IPropertyBlank<TKind, int> propertyIndex, RowLink insertRowLink, params object[] basicKeyParts)
    //{
    //  RowLink[] rows = Rows(propertyIndex.Kind, basicKeyParts);

    //  int insertPosition = _.BinarySearch(rows, insertRowLink, delegate(RowLink row1, RowLink row2)
    //  {
    //    return _.ValueComparison(propertyIndex.Field.Get(row1), propertyIndex.Field.Get(row2));
    //  });

    //  if (insertPosition < 0)
    //    insertPosition = ~insertPosition;

    //  for (int i = insertPosition; i < rows.Length; ++i)
    //  {
    //    TableLink.RemoveIndexForRow(rows[i]);
    //  }

    //  int linkIndex = propertyIndex.Field.Get(insertRowLink);
    //  TableLink.AddRow(insertRowLink);
    //  for (int i = insertPosition; i < rows.Length; ++i)
    //  {
    //    linkIndex++;
    //    propertyIndex.Field.Set(rows[i], linkIndex);
    //    TableLink.CreateIndexForRow(rows[i]);
    //  }
    //}
  }
}
