using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Commune.Diagnostics
{
  public class TraceMessage
  {
    public TraceMessage(object message)
    {
      this.message = message;
    }
    public TraceMessage()
    {
      message = String.Empty;
    }

    public TraceMessage WithTime(DateTime time)
    {
      this.messageTime = time;
      return this;
    }
    public TraceMessage WithMethod(MethodBase method)
    {
      this._method = method;
      return this;
    }
    public TraceMessage WithArgs(params object[] args)
    {
      this._args = args;
      return this;
    }
    public TraceMessage WithHash(int hash)
    {
      this._hash = hash;
      return this;
    }
    public TraceMessage WithExc(Exception exc)
    {
      this.message = exc;
      return this;
    }
    public TraceMessage WithText(string text)
    {
      this.message = text;
      return this;
    }

    public TraceMessage WithThread()
    {
      this.ThreadName = System.Threading.Thread.CurrentThread.Name;
      this.ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
      this.IsWithThread = true;
      return this;
    }

    //public string ThreadName = System.Threading.Thread.CurrentThread.Name;
    //public int ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

    public bool IsWithThread = false;
    public string ThreadName = null;
    public int ThreadId = -1;

    private DateTime messageTime = DateTime.Now;
    public DateTime MessageTime
    {
      get
      {
        return messageTime;
      }
      set
      {
        messageTime = value;
      }
    }
    private object message;
    public object Message
    {
      get
      {
        return message;
      }
      set
      {
        message = value;
      }
    }
    private MethodBase _method = null;
    public MethodBase Method
    {
      get
      {
        return _method;
      }
    }
    private object[] _args = new object[] { };
    public object[] Args
    {
      get
      {
        return _args;
      }
    }
    private int _hash = 0;
    public int Hash
    {
      get
      {
        return _hash;
      }
    }

    string OutputHash
    {
      get
      {
        return (_hash == 0) ? "" : "(" + _hash + ")";
      }
    }
    string NotNull(string message)
    {
      if (message != null)
        return message;
      return "";
    }
    string ToString(object obj)
    {
      if (obj == null)
        return null;
      return obj.ToString();
    }

    public override string ToString()
    {
      System.Text.StringBuilder builder = new System.Text.StringBuilder();
      if (_method != null && message is Exception)
      {
        builder.AppendFormat("{0}{2}.{1} \r\n",
          _method.ReflectedType.FullName, _method.Name, OutputHash);
      }
      if (_args != null && _args.Length != 0)
      {
        builder.AppendFormat("Аргументы: ");
        foreach (object arg in _args)
        {
          builder.Append((arg == null ? "<null>" : arg.ToString()));
          builder.Append(", ");
        }
        builder.Append("\r\n");
      }
      if (ThreadName != null)
        builder.AppendFormat("{0}: ", ThreadName);
      if (message != null)
        builder.Append(message);
      return builder.ToString();
    }
  }
}
