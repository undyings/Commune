using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace Commune.Diagnostics
{
  public class Log
  {
    const int MaxMessageCapacity = 100;
    const int MaxErrorCapacity = 30;
    const int MessageDelta = 40;
    const int ErrorDelta = 10;
    //	private Int64 GlobalMessageIndex = 0;
    //      private List<TraceMessage> messageHistory = new List<TraceMessage>();
    public readonly ListMessages MessageHistory = new ListMessages(MaxMessageCapacity, MessageDelta);
    public readonly ListMessages ErrorHistory = new ListMessages(MaxErrorCapacity, ErrorDelta);
    TraceMessage lastErrorMessage;

    readonly FileLog _FileLog;

    //	private bool _IsAutoShownOnError = false;
    private DateTime lastErrorTime = DateTime.Now;

    public Log(string logFilePath, long maxLogSize)
    {
      try
      {
        _FileLog = new FileLog(logFilePath, maxLogSize);

        _FileLog.WriteLine(string.Format(
          "<!--================================================================================-->\r\n" +
          "\r\n" +
          "<LogStarting time='{0}'/>\r\n", TimeToString(DateTime.Now)));
      }
      catch (Exception exc)
      {
        Console.WriteLine(new Exception("Не удалось создать лог файл", exc));
      }
    }

    //public Log()
    //  :
    //  this(Commune.Diagnostics.FileLog.DefaultLogFilePath, Commune.Diagnostics.FileLog.DefaultMaxLogSize)
    //{
    //}

    public IFileLog FileLog
    {
      get
      {
        return _FileLog;
      }
    }

    public DateTime LastErrorTime
    {
      get { return lastErrorTime; }
    }


    public void ProcessNewMessage(TraceMessage tMessage)
    {

      lock (ErrorHistory)
      {
        if (tMessage.Message is Exception)
        {
          lastErrorTime = DateTime.Now;
          lastErrorMessage = tMessage;
          //ErrorHistory.MessageList.Add(tMessage);
          //ErrorHistory.GlobalMessageIndex ++;

          //if (ErrorHistory.MessageList.Count > MaxErrorCapacity)
          //  ErrorHistory.ClearSomeMessages();
          ErrorHistory.AddNewMessage(tMessage);
        }

      }
      lock (MessageHistory)
      {
        //MessageHistory.MessageList.Add(tMessage);
        //MessageHistory.GlobalMessageIndex ++;
        MessageHistory.AddNewMessage(tMessage);
        _FileLog.WriteLine(ToXmlString(tMessage));

        //if (MessageHistory.MessageList.Count > MaxMessageCapacity)
        //  MessageHistory.ClearSomeMessages();
      }
    }

    static string ToXmlString(TraceMessage message)
    {
      StringBuilder builder = new StringBuilder();
      builder.AppendFormat("<Message time='{0}'>\r\n", TimeToString(message.MessageTime));
      if (message.IsWithThread)
      {
        if (message.ThreadName != null)
        {
          builder.AppendFormat("  <Thread name='{1}' id='{0}'/>\r\n", message.ThreadId,
            XmlEncode(message.ThreadName));
        }
        else
        {
          builder.AppendFormat("  <Thread id='{0}'/>\r\n", message.ThreadId);
        }
      }
      if (message.Method != null)
      {
        builder.AppendFormat("  <Method name='{0}.{1}' objectHash='{2}'/>\r\n",
          XmlEncode(message.Method.ReflectedType.FullName),
          XmlEncode(message.Method.Name),
          message.Hash);
      }
      if (message.Message != null)
      {
        builder.AppendFormat("  <Text>{0}</Text>\r\n", XmlEncode(Convert.ToString(message.Message)));
      }
      if (message.Args != null)
      {
        foreach (object arg in message.Args)
        {
          builder.AppendFormat("  <Arg>{0}</Arg>\r\n", XmlEncode(Convert.ToString(arg)));
        }
      }
      builder.Append("</Message>\r\n");
      return builder.ToString();
    }

    public static string XmlEncode(string text)
    {
      using (StringWriter textWriter = new StringWriter())
      {
        XmlTextWriter xmlWriter = new XmlTextWriter(textWriter);
        xmlWriter.WriteString(text);
        return textWriter.ToString();
      }
    }

    static string TimeToString(DateTime time)
    {
      return time.ToString("dd.MM.yyyy HH:mm:ss.fff");
    }

 
    [Serializable]
    public class Message
    {
      [XmlAttribute]
      public DateTime Time;
      public string MethodName;
      public string Text;
      [XmlElement("Arg")]
      public string[] Args;
    }

    public class ListMessages
    {
      public ListMessages(int maxMessageCapacity, int messageDelta)
      {
        this.maxMessageCapacity = maxMessageCapacity;
        this.messageDelta = messageDelta;
      }
      Int64 GlobalMessageIndex;
      readonly int maxMessageCapacity;
      readonly int messageDelta;
      public int Count { get { return (int)GlobalMessageIndex; } }

      List<TraceMessage> messageList = new List<TraceMessage>();
      internal List<TraceMessage> MessageList
      {
        get
        {
          lock (messageList)
          {
            return messageList;
          }
        }
      }
      /// <summary>
      /// Возвращает null при выходе за границы массива
      /// </summary>
      /// <param name="index"></param>
      /// <returns></returns>
      private TraceMessage this[Int64 index]
      {
        get
        {
          lock (messageList)
          {
            int realIndex = messageList.Count - (int)(GlobalMessageIndex - index);
            if (realIndex < messageList.Count && realIndex >= 0)
              return messageList[realIndex];
            else
              return null;
          }
        }
      }
      /// <summary>
      /// При первом вызове передавать -1, вернёт все хранимые сообщения. 
      /// При повторном вызове с той же переменной в качестве параметра
      /// возвращает ТОЛЬКО НОВЫЕ (относительно времени последнего вызова) сообщения.
      /// </summary>
      /// <param name="foreignIndex"></param>
      /// <returns></returns>
      public TraceMessage[] GetNewMessages(ref Int64 foreignIndex)
      {
        lock (messageList)
        {
          if (foreignIndex == -1)
          {
            foreignIndex = GlobalMessageIndex;
            return messageList.ToArray();
          }
          else
          {
            int index = Math.Max((int)(foreignIndex - GlobalMessageIndex + messageList.Count), 0);
            int count = Math.Min((int)(GlobalMessageIndex - foreignIndex), messageList.Count);
            List<TraceMessage> newMessages = messageList.GetRange(index, count);
            foreignIndex = GlobalMessageIndex;
            return newMessages.ToArray();
          }
        }
      }
      public void AddNewMessage(TraceMessage tMessage)
      {
        messageList.Add(tMessage);
        GlobalMessageIndex++;

        if (messageList.Count > maxMessageCapacity)
          ClearSomeMessages();
      }

      public void ClearSomeMessages()
      {
        lock (messageList)
        {
          messageList.RemoveRange(0, messageDelta);
        }
      }
    }
  }
}
