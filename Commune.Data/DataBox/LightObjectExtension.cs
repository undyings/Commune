using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Commune.Data
{
  public static class LightObjectExtension
  {
    public static TField Get<TField>(this LightHead obj, FieldBlank<TField> property)
    {
      if (obj == null)
        return default(TField);

      return property.Get(obj.headBox.ObjectById.AnyRow(obj.Id));
    }

    public static TField Get<TField>(this LightHead obj, XmlUniqueProperty<TField> property)
    {
      if (obj == null)
        return default(TField);

      return property.Get(obj.headBox, obj.Id);
    }

    public static TField Get<TField>(this LightObject obj,
      IPropertyBlank<int, TField> property, int propertyIndex)
    {
      if (obj == null)
        return default(TField);

      RowLink row = obj.Box.PropertiesByObjectIdWithKind.Row(property, propertyIndex, obj.Id);
      if (row == null)
        return default(TField);
      return property.Field.Get(row);
    }

    public static TField Get<TField>(this LightObject obj, IPropertyBlank<int, TField> property)
    {
      if (obj == null)
        return default(TField);

      return Get(obj, property, 0);
    }
  }
}
