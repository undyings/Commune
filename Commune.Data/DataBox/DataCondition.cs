using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public class DataCondition
  {
    public static string ForEnum(string columnName, IEnumerable<UniversalKey> keys)
    {
      List<string> keysAsStr = new List<string>();
      foreach (UniversalKey key in keys)
        keysAsStr.Add(key.KeyParts[0].ToString());
      if (keysAsStr.Count == 0)
        return "1=0";
      return string.Format("{0} in ({1})", columnName, string.Join(",", keysAsStr.ToArray()));
    }

    public static string ForEnum(string conditionColumn, params int[] conditionIds)
    {
      if (conditionIds.Length == 0)
        return "1=0";

      return string.Format("{0} in ({1})", conditionColumn, StringHlp.Join(",", "{0}", conditionIds));
    }

    public static string ForTypeObjects(int typeId, params int[] objectIds)
    {
      return string.Format("{0} and {1}", ForObjects(objectIds), ForTypes(typeId));
    }

    public static string ForObjects(params int[] objectIds)
    {
      return ForEnum("obj_id", objectIds);
    }

    public static string ForEnumString(string conditionColumn, params string[] conditionIds)
    {
      return string.Format("{0} in ({1})", conditionColumn, StringHlp.Join(",", "'{0}'", conditionIds));
    }

    public static string ForChilds(ParentBox parentBox, params LinkKindBlank[] linkKinds)
    {
      List<int> childIds = new List<int>();

      Dictionary<int, bool> linkKindByTypeId = new Dictionary<int, bool>();
      foreach (LinkKindBlank linkKind in linkKinds)
        linkKindByTypeId[linkKind.Kind] = true;

      foreach (RowLink childRow in parentBox.ChildByLinkId.TableLink.AllRows)
      {
        if (linkKindByTypeId.ContainsKey(childRow.Get(LinkType.TypeId)))
          childIds.Add(childRow.Get(LinkType.ChildId));
      }
      return ForEnum("obj_id", childIds.ToArray());
    }

    public static string ForTypes(params int[] typeIds)
    {
      if (typeIds.Length == 0)
        return "1=1";
      return ForEnum("type_id", typeIds);
    }
  }
}
