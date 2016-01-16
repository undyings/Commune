using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  /// <summary>
  /// Инвертирует результат сравнения для value
  /// </summary>
  public class CompareReverser : IComparable
  {
    public readonly object Value;
    public CompareReverser(object value)
    {
      this.Value = value;
    }
    public int CompareTo(object obj)
    {
      CompareReverser reverser = obj as CompareReverser;
      if (reverser != null)
        obj = reverser.Value;
      return -_.ValueComparison(Value, obj);
    }
  }
}
