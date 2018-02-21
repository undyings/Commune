using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace Commune.Forms
{
  public class GraphicsAlignment
  {
    public readonly StringAlignment Vertical;
    public readonly StringAlignment Horizontal;

    public GraphicsAlignment(StringAlignment vertical, StringAlignment horizontal)
    {
      this.Vertical = vertical;
      this.Horizontal = horizontal;
    }
  }

  //Extension class
  public class SynchronizerHlp
  {
    public static void ShowToolTip(ToolTip toolTip, string toolTipCaption, Control control, Point screenLocation)
    {
      Form form = control.FindForm();
      if (form == null)
        return;
      Point clientLocation = form.PointToClient(screenLocation);
      toolTip.Active = false;
      toolTip.Show(toolTipCaption, form, clientLocation);
      toolTip.Active = true;
      toolTip.Show(toolTipCaption, form, clientLocation);
    }

    public static void InnerPaintCellImage(Graphics graphics, Rectangle cellRectangle,
      IGridColumn column, object row, object value, Image image, bool? isRightImage,
      out Rectangle textRect)
    {
      int border = 1;

      textRect = cellRectangle;

      if (image == null)//Т.е не редактируемая ячейка 
        image = SynchronizerHlp.GetImageForCellOrValue(column, row, value);

      if (image != null)
      {
        //Определяем требуемую высоту 
        int imageHeight = cellRectangle.Height - border * 2;
        //но если исходная картинка меньше, то оставляем её высоту
        if (image.Height < imageHeight)
          imageHeight = image.Height;
        //Получаем коэффициент изменения
        double factor = (double)image.Height / imageHeight;
        //Расчитываем ширину
        int imageWidth = Convert.ToInt32(Math.Round(image.Width / factor, 0));

        int textWidth = cellRectangle.Width - (imageWidth + border);
        RectangleF imageRect = new RectangleF();

        switch (isRightImage)
        {
          case true:
            textRect = new Rectangle(textRect.X, textRect.Y, textWidth, textRect.Height);
            imageRect = new Rectangle(textRect.Right,
              cellRectangle.Y + ((cellRectangle.Height - imageHeight) / 2),
              imageWidth, imageHeight);//cellRectangle.Height - border * 2);
            break;
          case false:
            float bord = (cellRectangle.Height - imageHeight) / 2;

            imageRect = new RectangleF(cellRectangle.X + border, cellRectangle.Y + bord,
              imageWidth, imageHeight);


            textRect = new Rectangle((int)imageRect.Right + border, textRect.Y,
                  textWidth, textRect.Height);
            break;
          case null://текст НЕ выводится, а картинка выводится по центру
            textRect = new Rectangle(textRect.X, textRect.Y, 0, 0);
            imageRect = new Rectangle(
              cellRectangle.X + ((cellRectangle.Width - imageWidth) / 2),
              cellRectangle.Y + ((cellRectangle.Height - imageHeight) / 2),
              imageWidth, imageHeight);
            break;
          default:
            break;
        }

        graphics.DrawImage(image, imageRect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
      }
    }

    public static void PaintImageWithTextCell(Graphics graphics, Rectangle cellRectangle,
      IGridColumn column, object row, object value, Image image, bool? isRightImage)
    {
      Rectangle textRect;
      InnerPaintCellImage(graphics, cellRectangle, column, row, value, image, isRightImage, out textRect);
      SynchronizerHlp.PaintTextCell(graphics, textRect, column, row, value);
    }

    [Obsolete("Используйте перегрузку принимающую флажок isRightImage")]
    public static void PaintImageWithTextCell(Graphics graphics, Rectangle cellRectangle,
      IGridColumn column, object row, object value)
    {
      //Color? backColor = SynchronizerHlp.GetBackColor(column, row);
      //if (backColor != null)
      //  graphics.FillRectangle(new SolidBrush(backColor.Value), cellRectangle);

      //cellRectangle = new Rectangle(cellRectangle.X + 3, cellRectangle.Y + 3,
      //  cellRectangle.Width - 6, cellRectangle.Height - 6);

      Rectangle rawImageRectangle = new Rectangle(cellRectangle.X, cellRectangle.Y,
        (int)(cellRectangle.Height * 1.5f), cellRectangle.Height);
      Rectangle textRectangle = new Rectangle(rawImageRectangle.Right, cellRectangle.Y,
        Math.Max(0, cellRectangle.Width - rawImageRectangle.Width), cellRectangle.Height);

      Image image = SynchronizerHlp.GetImageForCellOrValue(column, row, value);
      if (image != null)
      {
        Size iconSize = new Size(image.Width * rawImageRectangle.Height / image.Height, rawImageRectangle.Height);
        if (image.Width / rawImageRectangle.Width > image.Height / rawImageRectangle.Height)
          iconSize = new Size(rawImageRectangle.Width, image.Height * rawImageRectangle.Width / image.Width);

        Rectangle imageRect = new Rectangle(rawImageRectangle.X + (rawImageRectangle.Width - iconSize.Width) / 2,
          rawImageRectangle.Y + (rawImageRectangle.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
        graphics.DrawImage(image, imageRect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
      }
      SynchronizerHlp.PaintTextCell(graphics, textRectangle, column, row, value);
    }

    public static void PaintTextCell(Graphics graphics, Rectangle cellRectangle,
      IGridColumn column, object row, object value)
    {
      PaintTextCell(graphics, cellRectangle, column, row, value, SynchronizerHlp.GetBackColor(column, row));
    }

    public static void PaintTextCell(Graphics graphics, Rectangle cellRectangle,
      IGridColumn column, object row, object value, Color? backColor)
    {
      string text = SynchronizerHlp.ValueToString(column, row, value);
      Font font = SynchronizerHlp.GetFont(column, row);
      StringFormat stringFormat = new StringFormat();
      GraphicsAlignment alignment = SynchronizerHlp.ConvertToGraphicsAlignment(SynchronizerHlp.GetAlignment(column, row));
      stringFormat.FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
      stringFormat.Alignment = alignment.Horizontal;
      stringFormat.LineAlignment = alignment.Vertical;

      if (backColor != null)
        graphics.FillRectangle(new SolidBrush(backColor.Value), cellRectangle);

      graphics.DrawString(text, font, new SolidBrush(Color.Black),
        new Rectangle(cellRectangle.X + 1, cellRectangle.Y + 1, cellRectangle.Width - 2, cellRectangle.Height - 2),
        stringFormat);
    }

    public static IGridColumn CreateNumberColumnForFilters<TRow>(IGridFilters<TRow> gridFilters, params ColumnExtensionAttribute[] columnAttributes)
    {
      IGridColumn filter = GridExt.Column<object>("ResetFilters", (Getter<object, object>)delegate
      {
        return (object)"X";
      }, (Executter<object, object>)delegate
      {
        ((GridFilters<TRow, TRow>)gridFilters).Storage.ResetFilters();
        ((GridFilters<TRow, TRow>)gridFilters).GridSynch.ForceSynchronize();
      }, GridExt.Bold(true), GridExt.CellDescription((Getter<string, object>)delegate
      {
        return "Сбросить фильтр";
      }), GridExt.CellFiller((IGridCellFiller)ButtonGridCellFiller.Default));
      string name = "№";
      ColumnExtensionAttribute[][] extensionAttributeArray1 = new ColumnExtensionAttribute[2][]
      {
        columnAttributes,
        null
      };
      extensionAttributeArray1[1] = new ColumnExtensionAttribute[3]
      {
        GridExt.CharWidth(4),
        GridExt.MaxCharWidth(4),
        GridExt.Filter(filter)
      };
      ColumnExtensionAttribute[] extensionAttributeArray2 = 
        ArrayHlp.Merge<ColumnExtensionAttribute>(extensionAttributeArray1);
      return (IGridColumn)new SimpleNumberColumn(name, extensionAttributeArray2);
    }


    //public static string ToString(object value)
    //{
    //  if (value == null)
    //    return null;
    //  return value.ToString();
    //}

    static object GetInnerExtended(IGridColumn column, string extensionName, object row)
    {
      if (column == null)
        return null;

      if (column is IMultiColumn)
        return ((IMultiColumn)column).GetExtended(extensionName, row);
      return column.GetExtended(extensionName);
    }

    public static object GetExtended(IGridColumn column, string extensionName, object row)
    {
      if (column == null)
        return null;

      Getter<Cell, object> cellGetter = GetInnerExtended(column, "CellProperties", row) as Getter<Cell, object>;
      if (cellGetter != null)
      {
        Cell cell = cellGetter(row);
        if (cell != null)
        {
          object extension = cell.Get(extensionName);
          if (extension != null)
            return extension;
        }
      }

      return GetInnerExtended(column, extensionName, row);
    }

    public static string ValueToString(IGridColumn column, object row, object value)
    {
      Getter<string, object> toStringContverter = GetExtended(column, "ValueToString", row) as Getter<string, object>;
      if (toStringContverter != null)
        return toStringContverter(value);
      if (value != null)
        return value.ToString();
      return "";
    }

    public static Brush GetRowBackBrush(IGridColumn column, object row, int rowIndex,
      VirtualGridDrawSettings drawSettings)
    {
      Getter<Brush, object> brushGetter = GetExtended(column, "RowBackBrush", row) as Getter<Brush, object>;
      if (brushGetter != null)
        return brushGetter(row);

      if (rowIndex % 2 != 0)
        return drawSettings.EvenRowBackBrush;

      return drawSettings.OddRowBackBrush;
    }

    public static IGridColumn GetCaptionColumn(IGridColumn column, IGridColumn defaultCaptionColumn)
    {
      IGridColumn captionColumn = column.GetExtended("CaptionColumn") as IGridColumn;
      if (captionColumn != null)
        return captionColumn;
      return defaultCaptionColumn;
    }

    public static HorizontalAlignment ConvertToHorizontalAlignment(DataGridViewContentAlignment alignment)
    {
      switch (alignment)
      {
        case DataGridViewContentAlignment.BottomLeft:
        case DataGridViewContentAlignment.MiddleLeft:
        case DataGridViewContentAlignment.TopLeft:
          return HorizontalAlignment.Left;
        case DataGridViewContentAlignment.BottomCenter:
        case DataGridViewContentAlignment.MiddleCenter:
        case DataGridViewContentAlignment.TopCenter:
          return HorizontalAlignment.Center;
        case DataGridViewContentAlignment.BottomRight:
        case DataGridViewContentAlignment.MiddleRight:
        case DataGridViewContentAlignment.TopRight:
          return HorizontalAlignment.Right;
      }
      return HorizontalAlignment.Left;
    }

    public static GraphicsAlignment ConvertToGraphicsAlignment(DataGridViewContentAlignment alignment)
    {
      switch (alignment)
      {
        case DataGridViewContentAlignment.BottomLeft:
          return new GraphicsAlignment(StringAlignment.Far, StringAlignment.Near);
        case DataGridViewContentAlignment.MiddleLeft:
          return new GraphicsAlignment(StringAlignment.Center, StringAlignment.Near);
        case DataGridViewContentAlignment.TopLeft:
          return new GraphicsAlignment(StringAlignment.Near, StringAlignment.Near);
        case DataGridViewContentAlignment.BottomCenter:
          return new GraphicsAlignment(StringAlignment.Far, StringAlignment.Center);
        case DataGridViewContentAlignment.MiddleCenter:
          return new GraphicsAlignment(StringAlignment.Center, StringAlignment.Center);
        case DataGridViewContentAlignment.TopCenter:
          return new GraphicsAlignment(StringAlignment.Near, StringAlignment.Center);
        case DataGridViewContentAlignment.BottomRight:
          return new GraphicsAlignment(StringAlignment.Far, StringAlignment.Far);
        case DataGridViewContentAlignment.MiddleRight:
          return new GraphicsAlignment(StringAlignment.Center, StringAlignment.Far);
        case DataGridViewContentAlignment.TopRight:
          return new GraphicsAlignment(StringAlignment.Near, StringAlignment.Far);
      }
      return new GraphicsAlignment(StringAlignment.Center, StringAlignment.Near);
    }

    public static double BaseUnitsInPixel = 3.125;
    public static double GetWidth(Graphics graphics, object widthPattern, Font font)
    {
      double width = 0;
      if (widthPattern != null && widthPattern is string)
      {
        string attrString = (string)widthPattern;
        int index = attrString.IndexOf(':');
        string attrValue = attrString.Substring(index + 1);
        if (attrString.Contains("pat"))
        {
          width = graphics.MeasureString(attrValue, font).Width;
        }
        else if (attrString.StartsWith("pixel"))
        {
          width = double.Parse(attrValue);
        }
        else if (attrString.Contains("pix"))
        {
          width = double.Parse(attrValue) * BaseUnitsInPixel;
        }
        else if (attrString.Contains("char"))
        {
          int charNum = int.Parse(attrValue);
          width = graphics.MeasureString("W" + new String('9', charNum - 1), font).Width;
        }
      }
      return width;
    }

    public static bool IsRightFakeCell(IGridColumn column, object row)
    {
      if (column == null)
        return false;

      Getter<bool, object> getter = GetExtended(column, "IsRightFakeCell", row) as Getter<bool, object>;
      if (getter == null)
        return false;
      return getter(row) && !IsCompositeCell(column, row);
    }

    public static bool IsLowerFakeCell(IGridColumn column, object row)
    {
      if (column == null)
        return false;

      Getter<bool, object> getter = GetExtended(column, "IsLowerFakeCell", row) as Getter<bool, object>;
      if (getter == null)
        return false;
      return getter(row) && !IsCompositeCell(column, row);
    }

    public static bool IsCompositeCell(IGridColumn column, object row)
    {
      if (column == null)
        return false;

      Getter<bool, object> getter = GetExtended(column, "IsCompositeCell", row) as Getter<bool, object>;
      if (getter == null)
        return false;
      return getter(row);
    }

    //public static string GetLeftPrefix(IGridColumn column, object row)
    //{
    //  if (column == null)
    //    return "";

    //  Getter<string, object> getter = GetExtended(column, "CellLeftPrefix", row) as Getter<string, object>;
    //  if (getter == null)
    //    return "";
    //  return getter(row);
    //}

    //public static string GetRightPrefix(IGridColumn column, object row)
    //{
    //  if (column == null)
    //    return "";

    //  Getter<string, object> getter = GetExtended(column, "CellRightPrefix", row) as Getter<string, object>;
    //  if (getter == null)
    //    return "";
    //  return getter(row);
    //}

    public static bool IsCompositeHeader(IGridColumn column)
    {
      return column.GetExtended("IsCompositeHeader") as bool? ?? false;
    }

    public static bool IsFakeHeader(IGridColumn column)
    {
      return column.GetExtended("IsFakeHeader") as bool? ?? false;
    }

    public static IGridColumn GetFilter(IGridColumn column)
    {
      if (column == null)
        return null;

      return column.GetExtended("Filter") as IGridColumn;
    }

    public static Image GetImageForCellOrValue(IGridColumn column, object row, object cellValue)
    {
      //Getter<Image, object, object> imageGetter = column.GetExtended("ImageForCellOrValue") as
      //  Getter<Image, object, object>;
      Getter<Image, object, object> imageGetter = GetExtended(column, "ImageForCellOrValue", row)
        as Getter<Image, object, object>;
      if (imageGetter == null)
        return null;
      return imageGetter(row, cellValue);
    }

    public static Color? GetBackColorForCellOrValue(IGridColumn column, object row, object cellValue)
    {
      Getter<Color?, object, object> backColorGetter = GetExtended(column, "BackColorForCellOrValue", row)
        as Getter<Color?, object, object>;
      if (backColorGetter == null)
        return null;
      return backColorGetter(row, cellValue);
    }

    public static object GetValue(IGridColumn column, object row)
    {
      if (column == null)
        return null;

      object value = column.GetValue(row);

      string format = SynchronizerHlp.GetFormat(column);
      if (format != null && value != null && !(value is Image))
      {
        return String.Format(format, value);
      }
      return value;
    }

    public static bool IsAnotherRow(IGridColumn column, object row)
    {
      if (column == null)
        return true;

      Getter<bool, object> checker = column.GetExtended("IsAnotherRow") as Getter<bool, object>;
      if (checker == null)
        return false;

      return checker(row);
    }

    public static string GetFormat(IGridColumn column)
    {
      try
      {
        object formPattern = column.GetExtended("Format");
        if (formPattern != null)
          return "{0, 0:" + (string)formPattern + "}";
      }
      catch (Exception exc)
      {
        Logger.WriteException(exc);
      }
      return null;
    }

    public static bool IsSubtitle(IGridColumn column)
    {
      object isSubtitle = column.GetExtended("IsSubtitle");
      if (isSubtitle is bool)
        return (bool)isSubtitle;
      return false;
    }

    public static string GetDisplayName(IGridColumn column)
    {
      Getter<string> nameGetter = column.GetExtended("DisplayNameGetter") as Getter<string>;
      if (nameGetter != null)
        return nameGetter();

      object name = column.GetExtended("DisplayName");
      if (name != null)
        return name.ToString();

      return column.Name;
    }

    public static Image GetHeaderImage(IGridColumn column)
    {
      Getter<Image> imageGetter = column.GetExtended("HeaderImage") as Getter<Image>;
      if (imageGetter == null)
        return null;
      return imageGetter();
    }

    public static string GetHeaderDescription(IGridColumn column)
    {
      Getter<string> descriptionGetter = column.GetExtended("HeaderDescription") as Getter<string>;
      if (descriptionGetter == null)
        return null;
      return descriptionGetter();
    }

    public static string GetCellDescription(IGridColumn column, object row)
    {
      Getter<string, object> descriptionGetter = GetExtended(column, "CellDescription", row) as Getter<string, object>;
      if (descriptionGetter == null)
        return null;
      return descriptionGetter(row);
    }

    public static bool IsOrCondition(IGridColumn filterColumn, object filterValue)
    {
      Getter<bool, object> isOrConditionChecker = filterColumn.GetExtended("IsOrCondition") as Getter<bool, object>;
      if (isOrConditionChecker == null)
        return false;
      return isOrConditionChecker(filterValue);
    }

    public static bool IsHideRow(IGridColumn gridColumn, IGridColumn filterColumn, object rowId, object filterValue)
    {
      Getter<bool, object, object, object> isHideRowChecker =
        filterColumn.GetExtended("IsHideRowChecker") as Getter<bool, object, object, object>;
      if (isHideRowChecker == null)
        return false;
      return isHideRowChecker(rowId, gridColumn.GetValue(rowId), filterValue);
    }

    public static bool IsMultilineCell(IGridColumn gridColumn, object row)
    {
      Getter<bool, object> isMultilineCellGetter = GetExtended(gridColumn, "IsMultilineCell", row) as Getter<bool, object>;
      if (isMultilineCellGetter == null)
        return false;
      return isMultilineCellGetter(row);
    }

    public static bool IsReadonly(IGridColumn column)
    {
      object readOnly = column.GetExtended("IsReadOnly");
      if (readOnly is bool)
        return (bool)readOnly;
      return true;
    }

    public static bool IsReadOnlyCell(IGridColumn column, object row)
    {
      bool isReadOnly = IsReadonly(column);
      if (isReadOnly)
        return true;

      Getter<bool, object> readOnlyGetter = GetExtended(column, "IsReadOnlyCell", row) as Getter<bool, object>;
      if (readOnlyGetter == null)
        return false;
      return readOnlyGetter(row);
    }

    public static bool IsVisible(IGridColumn column)
    {
      Getter<bool> visibleGetter = column.GetExtended("IsVisible") as Getter<bool>;
      if (visibleGetter != null)
        return visibleGetter();
      return true;
    }

    public static DataGridViewContentAlignment GetAlignment(IGridColumn column, object row)
    {
      if (column != null)
      {
        try
        {
          //DataGridViewContentAlignment? alignment = column.GetExtended("Alignment") as DataGridViewContentAlignment?;
          DataGridViewContentAlignment? alignment = GetExtended(column, "Alignment", row) as DataGridViewContentAlignment?;
          if (alignment != null && alignment.Value != DataGridViewContentAlignment.NotSet)
            return alignment.Value;
          else
          {
            Getter<bool?, object> isTopGetter = GetExtended(column, "CellVAlignment", row) as Getter<bool?, object>;
            Getter<bool?, object> isLeftGetter = GetExtended(column, "CellHAlignment", row) as Getter<bool?, object>;
            return GridExt.ConvertVHAlignmentToDataGrid(
              isTopGetter != null ? isTopGetter(row) : null, isLeftGetter != null ? isLeftGetter(row) : true);
          }
        }
        catch (Exception exc)
        {
          Logger.WriteException(exc);
        }
      }
      return DataGridViewContentAlignment.MiddleLeft;
    }

    public static double GetBorder(IGridColumn column)
    {
      if (column != null)
      {
        object borderPattern = column.GetExtended("Border");
        if (borderPattern != null)
          return Convert.ToDouble(borderPattern);
      }
      return 1;
    }

    public static double GetContentWidth(Graphics graphics, IGridColumn column, IEnumerable rows)
    {
      double maxContentWidth = 0;
      foreach (object row in rows)
      {
        object value = SynchronizerHlp.GetValue(column, row);
        double valueWidth = 0;
        if (value != null && value != DBNull.Value)
        {
          if (value is Image)
            valueWidth = ((Image)value).Width;
          else
            valueWidth = graphics.MeasureString(value.ToString(), SynchronizerHlp.GetFont(column, null)).Width;

          if (maxContentWidth < valueWidth)
            maxContentWidth = valueWidth;
        }
      }
      return maxContentWidth;
    }

    public static bool MakeParseError(IGridColumn column, object row, object parseValue)
    {
      Executter<object, object> executter = GetExtended(column, "ParseError", row) as Executter<object, object>;
      if (executter == null)
        return false;
      executter(row, parseValue);
      return true;
    }

    public static bool MakeButtonClick(IGridColumn column, object row)
    {
      Executter<object> executter = GetExtended(column, "CellButtonClick", row) as Executter<object>;
      if (executter == null)
        return false;
      executter(row);
      return true;
    }

    public static bool MakeCellClick(GridCell cell, Point clickCoord)
    {
      Executter<GridCell, Point> executter = SynchronizerHlp.GetExtended(cell.Property, "CellClickInformer", cell.DataItem) as Executter<GridCell, Point>;
      if (executter == null)
        return false;
      executter(cell, clickCoord);
      return true;
    }

    //public static bool MakeCellClick(IGridColumn column, object row)
    //{
    //  Executter<object> executter = GetExtended(column, "CellClickInformer", row) as Executter<object>;
    //  if (executter == null)
    //    return false;
    //  executter(row);
    //  return true;
    //}

    public static bool MakeCellDoubleClick(IGridColumn column, object row)
    {
      Executter<object> executter = GetExtended(column, "CellDoubleClickInformer", row) as Executter<object>;
      if (executter == null)
        return false;
      executter(row);
      return true;
    }

    public static bool MakeCellEnter(IGridColumn column, object row)
    {
      Executter<object> executter = GetExtended(column, "CellEnterInformer", row) as Executter<object>;
      if (executter == null)
        return false;
      executter(row);
      return true;
    }

    public static Font GetFont(IGridColumn column, object row)
    {
      bool isBold = false;
      Getter<bool?, object> isBoldGetter = GetExtended(column, "CellsBold", row) as Getter<bool?, object>;
      if (isBoldGetter != null)
        isBold = isBoldGetter(row) ?? false;

      bool isItalic = false;
      Getter<bool?, object> isItalicGetter = GetExtended(column, "CellsItalic", row) as Getter<bool?, object>;
      if (isItalicGetter != null)
        isItalic = isItalicGetter(row) ?? false;

      object fontSize = GetExtended(column, "FontSize", row);
      float emSize = 8;
      if (fontSize is float)
        emSize = (float)fontSize;
      else if (fontSize is int)
        emSize = (float)((int)fontSize);

      object fontName = GetExtended(column, "FontName", row);
      string familyName = "Microsoft Sans Serif";
      if (fontName is string)
        familyName = (string)fontName;

      FontStyle fontStyle = FontStyle.Regular;
      if (isBold && isItalic)
        fontStyle = FontStyle.Bold | FontStyle.Italic;
      else if (isBold)
        fontStyle = FontStyle.Bold;
      else if (isItalic)
        fontStyle = FontStyle.Italic;
      return new Font(familyName, emSize, fontStyle);
    }

    public static Color? GetBackColor(IGridColumn column, object row)
    {
      Getter<Color?, object> backColorGetter = GetExtended(column, "CellsBackColor", row) as Getter<Color?, object>;
      if (backColorGetter != null)
        return backColorGetter(row);
      return null;
    }

    public static int? GetRelativeWidth(IGridColumn column)
    {
      return column.GetExtended("RelativeWidth") as int?;
    }

    //public static int? GetFixedRelativeWidth(IGridColumn column)
    //{
    //  return column.GetExtended("FixedRelativeWidth") as int?;
    //}

    //public static bool IsFixedWidth(IGridColumn column)
    //{
    //  return column.GetExtended("IsFixedWidth") as bool? == true;
    //}

    public static bool IsNoDelayApply(IGridColumn column)
    {
      bool? noDelayApply = column.GetExtended("NoDelayApply") as bool?;
      if (noDelayApply != null)
        return noDelayApply.Value;
      return false;
    }

    public static bool IsBlinkingColumnHeader(IGridColumn column)
    {
      bool? isBlinking = column.GetExtended("IsBlinkingColumnHeader") as bool?;
      return isBlinking == true;
    }

    public static double GetMinWidth(Graphics graphics, IGridColumn column)
    {
      Font font = SynchronizerHlp.GetFont(column, null);
      double width = SynchronizerHlp.GetWidth(graphics, column.GetExtended("MinWidth"), font);
      double leastWidth = graphics.MeasureString("xxx", font).Width;
      return Math.Max(width, leastWidth);
    }

    public static double GetMaxWidth(Graphics graphics, IGridColumn column)
    {
      double maxWidth = SynchronizerHlp.GetWidth(graphics, column.GetExtended("MaxWidth"),
        SynchronizerHlp.GetFont(column, null));
      if (maxWidth == 0)
        return double.PositiveInfinity;
      return maxWidth;
    }

    public static bool IsCellSelectionDisabled(IGridColumn column, object row)
    {
      Getter<bool, object> getter = SynchronizerHlp.GetExtended(column, "CellSelectionDisabled", row) as Getter<bool, object>;
      if (getter == null)
        return false;
      else
        return getter(row);
    }

    public static Pen GetHMarkingPen(IGridColumn column, object row)
    {
      Getter<Pen, object> getter = SynchronizerHlp.GetExtended(column, "CellHMarkingPen", row) as Getter<Pen, object>;
      if (getter != null)
        return getter(row);
      else
        return (Pen)null;
    }

    public static Pen GetVMarkingPen(IGridColumn column, object row)
    {
      Getter<Pen, object> getter = SynchronizerHlp.GetExtended(column, "CellVMarkingPen", row) as Getter<Pen, object>;
      if (getter != null)
        return getter(row);
      else
        return (Pen)null;
    }

    public static bool GetCellEnabled(IGridColumn column, object row)
    {
      Getter<bool, object> getter = SynchronizerHlp.GetExtended(column, "CellEnabled", row) as Getter<bool, object>;
      if (getter != null)
        return getter(row);
      else
        return true;
    }
  }
}
