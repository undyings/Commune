using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class Tuple<T1, T2>
  {
    public Tuple(T1 first, T2 second)
    {
      this.First = first;
      this.Second = second;
    }
    public readonly T1 First;
    public readonly T2 Second;

    public override bool Equals(object obj)
    {
      Tuple<T1, T2> other = obj as Tuple<T1, T2>;
      if (other != null)
      {
        return object.Equals(this.First, other.First) &&
         object.Equals(this.Second, other.Second);
      }
      return false;
    }
    public override int GetHashCode()
    {
      return _.GetHashCode(First) ^ _.GetHashCode(Second);
    }
  }

  public class Tuple<T1, T2, T3>
  {
    public Tuple(T1 first, T2 second, T3 third)
    {
      this.First = first;
      this.Second = second;
      this.Third = third;
    }
    public readonly T1 First;
    public readonly T2 Second;
    public readonly T3 Third;

    public override bool Equals(object obj)
    {
      Tuple<T1, T2, T3> other = obj as Tuple<T1, T2, T3>;
      if (other != null)
      {
        return object.Equals(this.First, other.First) &&
         object.Equals(this.Second, other.Second) &&
         object.Equals(this.Third, other.Third);
      }
      return false;
    }
    public override int GetHashCode()
    {
      return _.GetHashCode(First) ^ _.GetHashCode(Second) ^ _.GetHashCode(Third);
    }
  }

  public class Tuple<T1, T2, T3, T4>
  {
    public Tuple(T1 first, T2 second, T3 third, T4 fourth)
    {
      this.First = first;
      this.Second = second;
      this.Third = third;
      this.Fourth = fourth;
    }
    public readonly T1 First;
    public readonly T2 Second;
    public readonly T3 Third;
    public readonly T4 Fourth;

    public override bool Equals(object obj)
    {
      Tuple<T1, T2, T3, T4> other = obj as Tuple<T1, T2, T3, T4>;
      if (other != null)
      {
        return object.Equals(this.First, other.First) &&
         object.Equals(this.Second, other.Second) &&
         object.Equals(this.Third, other.Third) &&
         object.Equals(this.Fourth, other.Fourth);
      }
      return false;
    }
    public override int GetHashCode()
    {
      return _.GetHashCode(First) ^ _.GetHashCode(Second) ^ _.GetHashCode(Third) ^ _.GetHashCode(Fourth);
    }
  }

  public class Tuple<T1, T2, T3, T4, T5>
  {
    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
    {
      this.First = first;
      this.Second = second;
      this.Third = third;
      this.Fourth = fourth;
      this.Fifth = fifth;
    }
    public readonly T1 First;
    public readonly T2 Second;
    public readonly T3 Third;
    public readonly T4 Fourth;
    public readonly T5 Fifth;

    public override bool Equals(object obj)
    {
      Tuple<T1, T2, T3, T4, T5> other = obj as Tuple<T1, T2, T3, T4, T5>;
      if (other != null)
      {
        return object.Equals(this.First, other.First) &&
         object.Equals(this.Second, other.Second) &&
         object.Equals(this.Third, other.Third) &&
         object.Equals(this.Fourth, other.Fourth) &&
         object.Equals(this.Fifth, other.Fifth);
      }
      return false;
    }
    public override int GetHashCode()
    {
      return _.GetHashCode(First) ^ _.GetHashCode(Second) ^ _.GetHashCode(Third) ^ _.GetHashCode(Fourth) ^
        _.GetHashCode(Fifth);
    }
  }

  public class Tuple<T1, T2, T3, T4, T5, T6>
  {
    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
    {
      this.First = first;
      this.Second = second;
      this.Third = third;
      this.Fourth = fourth;
      this.Fifth = fifth;
      this.Sixth = sixth;
    }
    public readonly T1 First;
    public readonly T2 Second;
    public readonly T3 Third;
    public readonly T4 Fourth;
    public readonly T5 Fifth;
    public readonly T6 Sixth;

    public override bool Equals(object obj)
    {
      Tuple<T1, T2, T3, T4, T5, T6> other = obj as Tuple<T1, T2, T3, T4, T5, T6>;
      if (other != null)
      {
        return object.Equals(this.First, other.First) &&
         object.Equals(this.Second, other.Second) &&
         object.Equals(this.Third, other.Third) &&
         object.Equals(this.Fourth, other.Fourth) &&
         object.Equals(this.Fifth, other.Fifth) &&
         object.Equals(this.Sixth, other.Sixth);
      }
      return false;
    }
    public override int GetHashCode()
    {
      return _.GetHashCode(First) ^ _.GetHashCode(Second) ^ _.GetHashCode(Third) ^ _.GetHashCode(Fourth) ^
        _.GetHashCode(Fifth) ^ _.GetHashCode(Sixth);
    }
  }
}
