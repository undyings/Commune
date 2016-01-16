using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class UserException : Exception
  {
    public readonly string Title;

    public UserException(string title, string messageFormat, params object[] args)
      : base(string.Format(messageFormat, args))
    {
      this.Title = title;
    }
  }
}

