using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Commune.Data
{
  public interface IRowLink
  {
    object GetValue(string name);
    void SetValue(string name, object value);
    bool IsContainsValue(string name);
  }
}
