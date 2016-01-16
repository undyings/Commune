using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Data
{
  [Serializable]
  public class DbParameter
  {
    public DbParameter()
    {
    }
    public DbParameter(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }
    public string Name;
    public object Value;
  }
}
