using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Data
{
  public class Nothing
  {
    public readonly static Nothing Value = new Nothing();
    Nothing() { }
  }

  public interface IPropertyKindBlank<TKind>
  {
    TKind Kind { get; }
  }

  public interface IPropertyBlank<TKind, TField> : IPropertyKindBlank<TKind>
  {
    FieldBlank<TField> Field { get; }
  }


  public class RowPropertyBlank<TField> : RowPropertyBlank<int, TField>
  {
    public RowPropertyBlank(int propertyKind, FieldBlank<TField> field)
      : base(propertyKind, field)
    {
    }
  }

  public class RowPropertyBlank<TKind, TField> : IPropertyBlank<TKind, TField>, IPropertyKindBlank<TKind>
  {
    private readonly FieldBlank<TField> field;
    private readonly TKind kind;

    public RowPropertyBlank(TKind propertyKind, FieldBlank<TField> field)
    {
      this.kind = propertyKind;
      this.field = field;
    }

    public FieldBlank<TField> Field
    {
      get { return this.field; }
    }

    public TKind Kind
    {
      get { return this.kind; }
    }
  }

  public class LinkKindBlank : IPropertyKindBlank<int>
  {
    private readonly int kind;

    public LinkKindBlank(int linkKind)
    {
      this.kind = linkKind;
    }

    public IPropertyBlank<int, TField> For<TField>(FieldBlank<TField> field)
    {
      return new RowPropertyBlank<TField>(this.kind, field);
    }

    public int Kind
    {
      get
      {
        return this.kind;
      }
    }
  }

}
