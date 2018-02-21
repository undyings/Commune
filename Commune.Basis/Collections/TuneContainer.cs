using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Basis
{
  public class TuneContainer<T>
  {
    readonly Dictionary<string, T> tuneByName = new Dictionary<string, T>();

    public TuneContainer()
    {
    }

    public T GetTune(string tuneName)
    {
      T tune;
      if (tuneByName.TryGetValue(tuneName, out tune))
        return tune;
      return default(T);
    }

    public void WithTune(string tuneName, T value)
    {
      if (tuneName == null)
        return;
      tuneByName[tuneName] = value;
    }
  }
}
