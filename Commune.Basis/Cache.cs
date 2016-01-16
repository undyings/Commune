using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{

  public interface ICache<TResult>
  {
    TResult Result { get; }
    long ChangeTick { get; }  
  }

  public class RawCache<TResult> : ICache<TResult>
  {
    public RawCache(Getter<TResult, object[]> resulter, params Getter<object>[] sourcers)
    {
      this.resulter = resulter;
      this.sourcers = sourcers;
      args_Old = new object[sourcers.Length];
    }
    Getter<TResult, object[]> resulter;
    Getter<object>[] sourcers;

    bool isInited = false;
    object[] args_Old;
    TResult result_cache;
    public long ChangeTick 
    { 
      get 
      {
        CheckAndUpdateData();
        return _ChangeTick; 
      } 
    }
    long _ChangeTick = 1;
    public TResult Result
    {
      get
      {
        CheckAndUpdateData();
        return result_cache;
      }
    }

    public bool IsInited
    {
      get { return isInited; }
    }

    public TResult ResultWithoutCheckAndUpdate
    {
      get { return result_cache; }
    }

    public long ChangeTickWithoutCheckAndUpdate
    {
      get { return _ChangeTick; }
    }

    private void CheckAndUpdateData()
    {
      object[] args = new object[sourcers.Length];
      for (int i = 0; i < sourcers.Length; ++i)
      {
        args[i] = sourcers[i]();
      }
      bool isChanged = false;
      for (int i = 0; i < sourcers.Length; ++i)
      {
        if (!object.Equals(args[i], args_Old[i]))
        {
          isChanged = true;
          break;
        }
      }

      if (!isInited || isChanged)
      {
        TResult result = resulter(args);

        result_cache = result;
        args_Old = args;
        isInited = true;
        _ChangeTick++;
      }
    }
  }

  public class Cache<TResult, TArg1> :
    RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1> resulter, Getter<TArg1> sourcer1)
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0]); },
      delegate { return sourcer1(); })
    {
    }
  }
  public class Cache<TResult, TArg1, TArg2> :
    RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2> resulter, Getter<TArg1> sourcer1, Getter<TArg2> sourcer2)
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); })
    {
    }
  }

  public class Cache<TResult, TArg1, TArg2, TArg3> :
    RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2, TArg3> resulter,
      Getter<TArg1> sourcer1,
      Getter<TArg2> sourcer2,
      Getter<TArg3> sourcer3
      )
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); },
      delegate { return sourcer3(); })
    {
    }
  }
  public class Cache<TResult, TArg1, TArg2, TArg3, TArg4> :
    RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2, TArg3, TArg4> resulter,
      Getter<TArg1> sourcer1,
      Getter<TArg2> sourcer2,
      Getter<TArg3> sourcer3,
      Getter<TArg4> sourcer4
      )
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); },
      delegate { return sourcer3(); },
      delegate { return sourcer4(); })
    {
    }
  }
  public class Cache<TResult, TArg1, TArg2, TArg3, TArg4, TArg5> :
    RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2, TArg3, TArg4, TArg5> resulter,
      Getter<TArg1> sourcer1,
      Getter<TArg2> sourcer2,
      Getter<TArg3> sourcer3,
      Getter<TArg4> sourcer4,
      Getter<TArg5> sourcer5
      )
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3], (TArg5)args[4]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); },
      delegate { return sourcer3(); },
      delegate { return sourcer4(); },
      delegate { return sourcer5(); })
    {
    }
  }
  public class Cache<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> :
  RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> resulter,
      Getter<TArg1> sourcer1,
      Getter<TArg2> sourcer2,
      Getter<TArg3> sourcer3,
      Getter<TArg4> sourcer4,
      Getter<TArg5> sourcer5,
      Getter<TArg6> sourcer6
      )
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3], (TArg5)args[4], (TArg6)args[5]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); },
      delegate { return sourcer3(); },
      delegate { return sourcer4(); },
      delegate { return sourcer5(); },
      delegate { return sourcer6(); })
    {
    }
  }
  public class Cache<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> :
  RawCache<TResult>
  {
    public Cache(Getter<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> resulter,
      Getter<TArg1> sourcer1,
      Getter<TArg2> sourcer2,
      Getter<TArg3> sourcer3,
      Getter<TArg4> sourcer4,
      Getter<TArg5> sourcer5,
      Getter<TArg6> sourcer6,
      Getter<TArg6> sourcer7
      )
      :
      base(delegate(object[] args) { return resulter((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3], (TArg5)args[4], (TArg6)args[5], (TArg7)args[6]); },
      delegate { return sourcer1(); },
      delegate { return sourcer2(); },
      delegate { return sourcer3(); },
      delegate { return sourcer4(); },
      delegate { return sourcer5(); },
      delegate { return sourcer6(); },
      delegate { return sourcer7(); })
    {
    }
  }
}
