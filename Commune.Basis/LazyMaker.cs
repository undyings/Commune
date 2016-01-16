using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public abstract class LazyMaker
  {
    private bool isInited;
    private readonly Executter<object[]> maker;
    private readonly Getter<object>[] sources;
    private object[] cacheArgs;
    private long changeTick;

    public long ChangeTick
    {
      get
      {
        this.Make();
        return this.changeTick;
      }
    }

    protected LazyMaker(Executter<object[]> maker, params Getter<object>[] sources)
    {
      this.maker = maker;
      this.sources = sources;
      this.cacheArgs = new object[sources.Length];
    }

    public bool Make()
    {
      return this.CheckAndUpdateSources();
    }

    private bool CheckAndUpdateSources()
    {
      object[] objArray = new object[this.sources.Length];
      for (int index = 0; index < this.sources.Length; ++index)
        objArray[index] = this.sources[index]();
      bool flag = false;
      if (this.isInited)
      {
        for (int index = 0; index < this.sources.Length; ++index)
        {
          if (!object.Equals(objArray[index], this.cacheArgs[index]))
          {
            flag = true;
            break;
          }
        }
      }
      if (this.isInited && !flag)
        return false;
      this.cacheArgs = objArray;
      ++this.changeTick;
      this.isInited = true;
      this.maker(objArray);
      return true;
    }
  }

  public class LazyMaker<TSource1> : LazyMaker
  {
    public LazyMaker(Executter<TSource1> maker, Getter<TSource1> source1)
      : base((Executter<object[]>)(args => maker((TSource1)args[0])), new Getter<object>[1]
      {
        (Getter<object>) (() => (object) source1())
      })
    {
    }
  }

  public class LazyMaker<TSource1, TSource2> : LazyMaker
  {
    public LazyMaker(Executter<TSource1, TSource2> maker, Getter<TSource1> source1, Getter<TSource2> source2)
      : base((Executter<object[]>)(args => maker((TSource1)args[0], (TSource2)args[1])), (Getter<object>)(() => (object)source1()), (Getter<object>)(() => (object)source2()))
    {
    }
  }

  public class LazyMaker<TSource1, TSource2, TSource3> : LazyMaker
  {
    public LazyMaker(Executter<TSource1, TSource2, TSource3> maker, Getter<TSource1> source1, Getter<TSource2> source2, Getter<TSource3> source3)
      : base((Executter<object[]>)(args => maker((TSource1)args[0], (TSource2)args[1], (TSource3)args[2])), (Getter<object>)(() => (object)source1()), (Getter<object>)(() => (object)source2()), (Getter<object>)(() => (object)source3()))
    {
    }
  }

  public class LazyMaker<TSource1, TSource2, TSource3, TSource4> : LazyMaker
  {
    public LazyMaker(Executter<TSource1, TSource2, TSource3, TSource4> maker, Getter<TSource1> source1, Getter<TSource2> source2, Getter<TSource3> source3, Getter<TSource4> source4)
      : base((Executter<object[]>)(args => maker((TSource1)args[0], (TSource2)args[1], (TSource3)args[2], (TSource4)args[3])), (Getter<object>)(() => (object)source1()), (Getter<object>)(() => (object)source2()), (Getter<object>)(() => (object)source3()), (Getter<object>)(() => (object)source4()))
    {
    }
  }

  public class LazyMaker<TSource1, TSource2, TSource3, TSource4, TSource5> : LazyMaker
  {
    public LazyMaker(Executter<TSource1, TSource2, TSource3, TSource4, TSource5> maker, Getter<TSource1> source1, Getter<TSource2> source2, Getter<TSource3> source3, Getter<TSource4> source4, Getter<TSource5> source5)
      : base((Executter<object[]>)(args => maker((TSource1)args[0], (TSource2)args[1], (TSource3)args[2], (TSource4)args[3], (TSource5)args[4])), (Getter<object>)(() => (object)source1()), (Getter<object>)(() => (object)source2()), (Getter<object>)(() => (object)source3()), (Getter<object>)(() => (object)source4()), (Getter<object>)(() => (object)source5()))
    {
    }
  }
}