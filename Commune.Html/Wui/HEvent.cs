using Commune.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroBolt.Wui
{
  public class HEventElement : HElement
  {
    public readonly hevent handler = null;

    public HEventElement(HName name, params object[] content) :
      base(name, content)
    {
      foreach (object node in content)
      {
        if (node is hevent)
        {
          handler = (hevent)node;
          break;
        }
      }
    }
  }

  public class hevent : hdata
  {
    readonly Func<object[], JsonData, object> eventHandler;

    public object Execute(JsonData jsonData)
    {
      object[] ids = ArrayHlp.Convert(this.ToArray(),
        delegate(HObject attr)
        { 
          return ((HAttribute)attr).Value; 
        }
      );
      return eventHandler(ids, jsonData);

      //object[] ids =  attributes.ConvertAll(delegate(HObject attr)
      //{ return ((HAttribute)attr).Value; }).ToArray();
      //return eventHandler(ids, jsonData);
    }

    public hevent(Func<object[], JsonData, object> eventHandler)
    {
      this.eventHandler = eventHandler;
    }
  }
}
