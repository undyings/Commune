using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Data
{
  public class ObjectType
  {
    public readonly static FieldBlank<int> ObjectId = new FieldBlank<int>("ObjectId", IntLongConverter.Default);
    public readonly static FieldBlank<int> TypeId = new FieldBlank<int>("TypeId", IntLongConverter.Default);
    public readonly static FieldBlank<string> XmlObjectIds = new FieldBlank<string>("XmlObjectIds");
    public readonly static FieldBlank<DateTime?> ActFrom = new FieldBlank<DateTime?>("ActFrom");
    public readonly static FieldBlank<DateTime?> ActTill = new FieldBlank<DateTime?>("ActTill");
    //public readonly static FieldBlank<DateTime?> ActFrom = 
    //  new FieldBlank<DateTime?>("ActFrom", DateTimeLongConverter.Default);
    //public readonly static FieldBlank<DateTime?> ActTill = 
    //  new FieldBlank<DateTime?>("ActTill", DateTimeLongConverter.Default);

    public readonly static SingleIndexBlank ObjectById = new SingleIndexBlank("ObjectById", ObjectId);
    public readonly static MultiIndexBlank ObjectsByTypeId = new MultiIndexBlank("ObjectsByTypeId", TypeId);
    public readonly static MultiIndexBlank ObjectByXmlIds = new MultiIndexBlank("ObjectByXmlIds", XmlObjectIds);
  }

  public class PropertyType
  {
    public readonly static FieldBlank<int> PropertyId = new FieldBlank<int>("PropertyId", IntLongConverter.Default);
    public readonly static FieldBlank<int> ObjectId = new FieldBlank<int>("ObjectId", IntLongConverter.Default);
    public readonly static FieldBlank<int> TypeId = new FieldBlank<int>("TypeId", IntLongConverter.Default);
    public readonly static FieldBlank<int> PropertyIndex = new FieldBlank<int>("PropertyIndex", IntLongConverter.Default);
    public readonly static FieldBlank<string> PropertyValue = new FieldBlank<string>("PropertyValue");

    public readonly static SingleIndexBlank PropertyById = new SingleIndexBlank("PropertyById", PropertyId);
    public readonly static SingleIndexBlank PropertyByUnique = new SingleIndexBlank("PropertyByUnique",
      ObjectId, TypeId, PropertyIndex);
    public readonly static MultiIndexBlank PropertiesByObjectIdAndTypeId = new MultiIndexBlank("PropertiesByObjectIdAndTypeId",
      ObjectId, TypeId);
    public readonly static MultiIndexBlank PropertiesByTypeId = new MultiIndexBlank("PropertiesByTypeId", TypeId);
    public readonly static MultiIndexBlank PropertiesByObjectId = new MultiIndexBlank("PropertiesByObjectId", ObjectId);
  }

  public class LinkType
  {
    public readonly static FieldBlank<int> LinkId = new FieldBlank<int>("LinkId", IntLongConverter.Default);
    public readonly static FieldBlank<int> ParentId = new FieldBlank<int>("ParentId", IntLongConverter.Default);
    public readonly static FieldBlank<int> TypeId = new FieldBlank<int>("TypeId", IntLongConverter.Default);
    public readonly static FieldBlank<int> LinkIndex = new FieldBlank<int>("LinkIndex", IntLongConverter.Default);
    public readonly static FieldBlank<int> ChildId = new FieldBlank<int>("ChildId", IntLongConverter.Default);
    public readonly static FieldBlank<DateTime?> ActFrom = new FieldBlank<DateTime?>("ActFrom");
    public readonly static FieldBlank<DateTime?> ActTill = new FieldBlank<DateTime?>("ActTill");

    public readonly static SingleIndexBlank LinkById = new SingleIndexBlank("LinkById", LinkId);
    public readonly static MultiIndexBlank LinksByParentIdAndTypeId = new MultiIndexBlank("LinksByParentIdAndTypeId",
      ParentId, TypeId);
    public readonly static MultiIndexBlank LinksByParentId = new MultiIndexBlank("LinksByParentId", ParentId);
    public readonly static MultiIndexBlank LinksByChildId = new MultiIndexBlank("LinksByChildId", ChildId);
    public readonly static MultiIndexBlank LinksByChildIdAndTypeId = new MultiIndexBlank("LinksByChildIdAndTypeId",
      ChildId, TypeId);
  }
}
