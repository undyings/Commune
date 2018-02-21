using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Commune.Data
{
  public interface IDataLayer
  {
    DataTable GetTable(string database, string query, params DbParameter[] parameters);
    object GetScalar(string database, string query, params DbParameter[] parameters);
    void UpdateTable(string database, string query, DataTable table);
    string DbParamPrefix { get; }
  }

  public interface IBoxConnection
  {
    string Database
    {
      get;
    }

    string TablePrefix
    {
      get;
    }
  }
}
