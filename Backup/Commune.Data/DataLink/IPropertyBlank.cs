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


  public class RowPropertyBlank<TField> : RowPropertyBlank<string, TField>
  {
    public RowPropertyBlank(string propertyKind, FieldBlank<TField> field)
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

  public class StringPropertyKindBlank : IPropertyKindBlank<string>
  {
    private readonly string kind;

    public StringPropertyKindBlank(string propertyKind)
    {
      this.kind = propertyKind;
    }

    public IPropertyBlank<string, TField> For<TField>(FieldBlank<TField> field)
    {
      return new RowPropertyBlank<TField>(this.kind, field);
    }

    public string Kind
    {
      get
      {
        return this.kind;
      }
    }
  }

}
