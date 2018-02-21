using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Commune.Basis;

namespace Commune.Forms
{
  public class InternalGridCache
  {
    public InternalGridCache()
    {
    }

    object[,] cacheValues;
    int cacheFirstDisplayRowIndex = 0;

    public object GetValue(int columnIndex, int rowIndex)
    {
      int displayRowIndex = rowIndex - cacheFirstDisplayRowIndex;
      if (cacheValues == null || cacheValues.GetLength(0) <= columnIndex ||
        cacheValues.GetLength(1) <= displayRowIndex || displayRowIndex < 0)
        return null;
      return cacheValues[columnIndex, displayRowIndex];
    }

    public void Refresh(IList<IGridColumn> columns, IList rows, int firstDisplayRowIndex, int displayRowsCount)
    {
      //TraceHlp2.AddMessage("InternalGridCache: {0}, {1}, {2}, {3}", columns.Count, rows.Count,
      //  firstDisplayRowIndex, displayRowsCount);

      if (firstDisplayRowIndex < 0)
        return;

      this.cacheFirstDisplayRowIndex = firstDisplayRowIndex;
      cacheValues = new object[columns.Count, displayRowsCount];

      for (int curCol = 0; curCol < columns.Count; curCol++)
        for (int curRow = 0; curRow < displayRowsCount; curRow++)
        {
          if (curRow >= rows.Count)
            break;
          try
          {
            cacheValues[curCol, curRow] = columns[curCol].GetValue(rows[firstDisplayRowIndex + curRow]);
          }
          catch (Exception exc)
          {
            Logger.WriteException(exc);
          }
        }
    }

    public bool IsValid(IList<IGridColumn> columns, IList rows, int firstDisplayRowIndex, int displayRowsCount)
    {
      if (firstDisplayRowIndex < 0)
        return true;

      if (firstDisplayRowIndex + displayRowsCount > rows.Count)
        return false;

      if (cacheValues == null)
        return false;

      if (this.cacheFirstDisplayRowIndex != firstDisplayRowIndex)
        return false;

      if (cacheValues.GetLength(0) != columns.Count)
        return false;

      if (cacheValues.GetLength(1) != displayRowsCount)
        return false;

      for (int curCol = 0; curCol < columns.Count; curCol++)
        for (int curRow = 0; curRow < displayRowsCount; curRow++)
          try
          {
            if (!object.Equals(cacheValues[curCol, curRow], columns[curCol].GetValue(rows[firstDisplayRowIndex + curRow])))
              return false;
          }
          catch (Exception exc)
          {
            Logger.WriteException(exc);
            return true;
          }
      return true;
    }
  }
}
