using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Commune.Basis;
using System.Collections;

namespace Commune.Forms
{
  public static class GridExt
  {
    public static Getter<IList<IGridColumn>> ColumnsGetter(params IGridColumn[] columns)
    {
      return delegate { return columns; };
    }

    public static IGridColumn Column(string name, params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<object>(name, null, null, extensions);
    }

    public static GridColumn<T> InnerColumn<T>(params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<T>("", null, null, extensions);
    }

    public static GridColumn<T> InnerColumn<T>(Getter<object, T> getter, params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<T>("", getter, null, extensions);
    }

    public static GridColumn<T> InnerColumn<T>(Getter<object, T> getter, Executter<T, object> setter,
      params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<T>("", getter, setter, extensions);
    }

    public static IGridColumn Column<T>(string name, Getter<object, T> getter,
      params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<T>(name, getter, null, extensions);
    }

    public static IGridColumn Column<T>(string name, Getter<object, T> getter, Executter<T, object> setter,
      params ColumnExtensionAttribute[] extensions)
    {
      return new GridColumn<T>(name, getter, setter, extensions);
    }

    public static IGridColumn Column<T>(string name, string displayName,
      Getter<object, T> getter, params ColumnExtensionAttribute[] extensions)
    {
      GridColumn<T> column = new GridColumn<T>(name, getter, null, extensions);
      column.WithExtension(GridExt.DisplayName(displayName));
      return column;
    }

    public static IGridColumn Column<TRow>(string name, Getter<Cell, TRow> getter,
      Executter<TRow, object> setter, params ColumnExtensionAttribute[] extensions)
    {
      GridColumn<TRow> column = new GridColumn<TRow>(name, delegate (TRow row)
      {
        return getter(row).Value;
      },
        setter, extensions
      );
      column.WithExtension(GridExt.CellProperties(getter));
      return column;
    }

    public static IGridColumn Column<TRow>(string name, Getter<Cell, TRow> getter,
      params ColumnExtensionAttribute[] extensions)
    {
      return Column(name, getter, null, extensions);
    }

    public static IGridColumn Column<T>(string name, string displayName,
      Getter<object, T> getter, Executter<T, object> setter, params ColumnExtensionAttribute[] extensions)
    {
      GridColumn<T> column = new GridColumn<T>(name, getter, setter, extensions);
      column.WithExtension(GridExt.DisplayName(displayName));
      return column;
    }

    public static IGridColumn MultiColumn<T1, T2>(string name, GridColumn<T1> column1, GridColumn<T2> column2,
      params ColumnExtensionAttribute[] extensions)
    {
      return new MultiColumn<T1, T2>(name, column1, column2, extensions);
    }

    public static IGridColumn MultiColumn<T1, T2, T3>(string name, GridColumn<T1> column1, GridColumn<T2> column2,
      GridColumn<T3> column3, params ColumnExtensionAttribute[] extensions)
    {
      return new MultiColumn<T1, T2, T3>(name, column1, column2, column3, extensions);
    }

    public static IGridColumn MultiColumn<T1, T2, T3, T4>(string name, GridColumn<T1> column1,
      GridColumn<T2> column2, GridColumn<T3> column3, GridColumn<T4> column4,
      params ColumnExtensionAttribute[] extensions)
    {
      return new MultiColumn<T1, T2, T3, T4>(name, column1, column2, column3, column4, extensions);
    }

    public static IGridColumn MultiColumn<T1, T2, T3, T4, T5>(string name, GridColumn<T1> column1,
      GridColumn<T2> column2, GridColumn<T3> column3, GridColumn<T4> column4, GridColumn<T5> column5,
      params ColumnExtensionAttribute[] extensions)
    {
      return new MultiColumn<T1, T2, T3, T4, T5>(name, column1, column2, column3, column4, column5, extensions);
    }


    //public static IGridColumn Column<T1, T2>(string name, Getter<object, T1> getter1, Getter<object, T2> getter2,
    //  params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2>(name, getter1, getter2, null, null, true, extensions);
    //}

    //public static IGridColumn Column<T1, T2>(string name, Getter<object, T1> getter1, Getter<object, T2> getter2,
    //  Executter<T1, object> setter1, Executter<T2, object> setter2, params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2>(name, getter1, getter2, setter1, setter2, false, extensions);
    //}

    //public static IGridColumn Column<T1, T2, T3>(string name, 
    //  Getter<object, T1> getter1, Getter<object, T2> getter2, Getter<object, T3> getter3,
    //  Executter<T1, object> setter1, Executter<T2, object> setter2, Executter<T3, object> setter3,
    //  params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2, T3>(name, getter1, getter2, getter3, 
    //    setter1, setter2, setter3, false, extensions);
    //}

    //public static IGridColumn Column<T1, T2, T3>(string name,
    //  Getter<object, T1> getter1, Getter<object, T2> getter2, Getter<object, T3> getter3,
    //  params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2, T3>(name, getter1, getter2, getter3, null, null, null, true, extensions);
    //}

    //public static IGridColumn Column<T1, T2, T3, T4>(string name, Getter<object, T1> getter1,
    //  Getter<object, T2> getter2, Getter<object, T3> getter3, Getter<object, T4> getter4,
    //  Executter<T1, object> setter1, Executter<T2, object> setter2, Executter<T3, object> setter3,
    //  Executter<T4, object> setter4, params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2, T3, T4>(name, getter1, getter2, getter3, getter4,
    //    setter1, setter2, setter3, setter4, false, extensions);
    //}

    //public static IGridColumn Column<T1, T2, T3, T4>(string name, Getter<object, T1> getter1,
    //  Getter<object, T2> getter2, Getter<object, T3> getter3, Getter<object, T4> getter4,
    //  params ColumnExtensionAttribute[] extensions)
    //{
    //  return new MultiColumn<T1, T2, T3, T4>(name, getter1, getter2, getter3, getter4, 
    //    null, null, null, null, true, extensions);
    //}

    public static void WithDefaultExtensions(IGridColumn[] columns, params ColumnExtensionAttribute[] extensions)
    {
      foreach (IGridColumn column in columns)
      {
        ISupportDefaultExtensionColumn columnWithDefault = column as ISupportDefaultExtensionColumn;
        if (columnWithDefault == null)
          continue;

        foreach (ColumnExtensionAttribute extension in extensions)
        {
          if (column.GetExtended(extension.Name) == null || (extension.Name == "Alignment" &&
            (DataGridViewContentAlignment)column.GetExtended("Alignment") == DataGridViewContentAlignment.NotSet))
            columnWithDefault.WithExtension(extension);
        }
      }
    }

    public static Getter<T> AsGetter<T>(T value)
    {
      return delegate { return value; };
    }

    public static ColumnExtensionAttribute CaptionColumn(params ColumnExtensionAttribute[] extensions)
    {
      return new ColumnExtensionAttribute("CaptionColumn", new GridColumn<IGridColumn>("",
        delegate (IGridColumn column) { return SynchronizerHlp.GetDisplayName(column); }, null,
        extensions));
    }

    public static DataGridViewContentAlignment ConvertVHAlignmentToDataGrid(bool? isTop, bool? isLeft)
    {
      if (isTop == true && isLeft == true)
        return DataGridViewContentAlignment.TopLeft;
      if (isTop == true && isLeft == null)
        return DataGridViewContentAlignment.TopCenter;
      if (isTop == true && isLeft == false)
        return DataGridViewContentAlignment.TopRight;
      if (isTop == null && isLeft == true)
        return DataGridViewContentAlignment.MiddleLeft;
      if (isTop == null && isLeft == null)
        return DataGridViewContentAlignment.MiddleCenter;
      if (isTop == null && isLeft == false)
        return DataGridViewContentAlignment.MiddleRight;
      if (isTop == false && isLeft == true)
        return DataGridViewContentAlignment.BottomLeft;
      if (isTop == false && isLeft == null)
        return DataGridViewContentAlignment.BottomCenter;
      if (isTop == false && isLeft == false)
        return DataGridViewContentAlignment.BottomRight;
      return DataGridViewContentAlignment.NotSet;
    }

    public static ColumnExtensionAttribute VHAlignment(bool? isTop, bool? isLeft)
    {
      DataGridViewContentAlignment alignment = ConvertVHAlignmentToDataGrid(isTop, isLeft);
      return Alignment(alignment);
    }

    static ColumnExtensionAttribute CreateExecutter<T>(string name, Executter<T> informer)
    {
      return new ColumnExtensionAttribute(name, new Executter<object>(delegate (object row)
      {
        informer((T)row);
      }));
    }

    static ColumnExtensionAttribute CreateGetter<T1, T2>(string name, Getter<T1, T2> getter)
    {
      return new ColumnExtensionAttribute(name, new Getter<T1, object>(delegate (object row)
      {
        return getter((T2)row);
      }));
    }

    public static ColumnExtensionAttribute ParseError<T>(Executter<T, object> errorInformer)
    {
      return new ColumnExtensionAttribute("ParseError", new Executter<object, object>(
        delegate (object row, object parseValue)
        {
          errorInformer((T)row, parseValue);
        }));
    }

    public static ColumnExtensionAttribute DefaultRowSettings(Getter<IGridColumn> defaultRowSettingsGetter)
    {
      return new ColumnExtensionAttribute("DefaultRowSettings", defaultRowSettingsGetter);
    }

    public static ColumnExtensionAttribute CellProperties<TRow>(Getter<Cell, TRow> cellGetter)
    {
      return CreateGetter("CellProperties", cellGetter);
    }

    //public static ColumnExtensionAttribute LeftPrefix(string leftPrefix)
    //{
    //  return new ColumnExtensionAttribute("CellLeftPrefix", new Getter<string, object>(
    //    delegate(object row)
    //    {
    //      return leftPrefix;
    //    }));
    //}

    //public static ColumnExtensionAttribute RightPrefix(string rightPrefix)
    //{
    //  return new ColumnExtensionAttribute("CellRightPrefix", new Getter<string, object>(
    //    delegate(object row)
    //    {
    //      return rightPrefix;
    //    }));
    //}

    public static ColumnExtensionAttribute RowBackBrush<TRow>(Getter<Brush, TRow> backBrushGetter)
    {
      return CreateGetter("RowBackBrush", backBrushGetter);
    }

    public static ColumnExtensionAttribute IsRightFakeCell<TRow>(Getter<bool, TRow> isRightFakeCellGetter)
    {
      return CreateGetter("IsRightFakeCell", isRightFakeCellGetter);
    }

    public static ColumnExtensionAttribute IsRightFakeCell<TRow>()
    {
      return IsRightFakeCell(delegate (TRow row) { return true; });
    }

    public static ColumnExtensionAttribute IsRightFakeCell(bool isRightFakeCell)
    {
      return new ColumnExtensionAttribute("IsRightFakeCell",
        new Getter<bool, object>(delegate (object row) { return isRightFakeCell; }));
    }

    public static ColumnExtensionAttribute IsLowerFakeCell<TRow>(Getter<bool, TRow> isLowerFakeCellGetter)
    {
      return CreateGetter("IsLowerFakeCell", isLowerFakeCellGetter);
    }

    public static ColumnExtensionAttribute IsLowerFakeCell<TRow>()
    {
      return IsLowerFakeCell(delegate (TRow row) { return true; });
    }

    public static ColumnExtensionAttribute IsLowerFakeCell(bool isLowerFakeCell)
    {
      return new ColumnExtensionAttribute("IsLowerFakeCell",
        new Getter<bool, object>(delegate (object row) { return isLowerFakeCell; }));
    }

    public static ColumnExtensionAttribute IsCompositeCell<TRow>()
    {
      return IsCompositeCell(delegate (TRow row) { return true; });
    }

    public static ColumnExtensionAttribute IsCompositeCell<TRow>(Getter<bool, TRow> isCompositeCellGetter)
    {
      return CreateGetter("IsCompositeCell", isCompositeCellGetter);
    }

    public static ColumnExtensionAttribute IsCompositeCell(bool isCompositeCell)
    {
      return new ColumnExtensionAttribute("IsCompositeCell",
        new Getter<bool, object>(delegate (object row) { return isCompositeCell; }));
    }

    public static ColumnExtensionAttribute IsCompositeHeader(bool isCompositeHeader)
    {
      return new ColumnExtensionAttribute("IsCompositeHeader", isCompositeHeader);
    }

    public static ColumnExtensionAttribute IsFakeHeader(bool isFakeHeader)
    {
      return new ColumnExtensionAttribute("IsFakeHeader", isFakeHeader);
    }

    public static ColumnExtensionAttribute IsAnotherRow<TRow>(Getter<bool, TRow> isAnotherRowChecker)
    {
      return CreateGetter("IsAnotherRow", isAnotherRowChecker);
    }

    public static ColumnExtensionAttribute CellEnabled<T>(Getter<bool, T> cellEnabledGetter)
    {
      return GridExt.CreateGetter<bool, T>("CellEnabled", cellEnabledGetter);
    }

    public static ColumnExtensionAttribute CellButtonClick<T>(Executter<T> informer)
    {
      return CreateExecutter("CellButtonClick", informer);
    }

    public static ColumnExtensionAttribute CellEnterInformer<T>(Executter<T> informer)
    {
      return CreateExecutter("CellEnterInformer", informer);
    }

    public static ColumnExtensionAttribute CellEnterInformer(Executter<object> informer)
    {
      return new ColumnExtensionAttribute("CellEnterInformer", informer);
    }

    public static ColumnExtensionAttribute CellDoubleClickInformer<T>(Executter<T> informer)
    {
      return CreateExecutter("CellDoubleClickInformer", informer);
    }

    public static ColumnExtensionAttribute CellDoubleClickInformer(Executter<object> informer)
    {
      return new ColumnExtensionAttribute("CellDoubleClickInformer", informer);
    }

    public static ColumnExtensionAttribute CellClickAdvancedInformer(Executter<GridCell, Point> informer)
    {
      return new ColumnExtensionAttribute("CellClickInformer", (object)informer);
    }

    public static ColumnExtensionAttribute CellClickInformer<T>(Executter<T> informer)
    {
      return new ColumnExtensionAttribute("CellClickInformer", (object)(Executter<GridCell, Point>)((cell, clickCoord) => informer((T)cell.DataItem)));
    }

    public static ColumnExtensionAttribute CellClickInformer(Executter<object> informer)
    {
      return new ColumnExtensionAttribute("CellClickInformer", (object)(Executter<GridCell, Point>)((cell, clickCoord) => informer(cell.DataItem)));
    }


    public static ColumnExtensionAttribute HAlignment(bool? isLeft)
    {
      return VHAlignment(null, isLeft);
    }

    public static ColumnExtensionAttribute CellHAlignment<T>(Getter<bool?, T> isLeftAlignmentGetter)
    {
      return CreateGetter("CellHAlignment", isLeftAlignmentGetter);
      //return new ColumnExtensionAttribute("CellHAlignment", isLeftAlignmentGetter);
    }

    public static ColumnExtensionAttribute CellVAlignment<T>(Getter<bool?, T> isTopAlignmentGetter)
    {
      return CreateGetter("CellVAlignment", isTopAlignmentGetter);
      //return new ColumnExtensionAttribute("CellVAlignment", isTopAlignmentGetter);
    }

    public static ColumnExtensionAttribute Alignment(DataGridViewContentAlignment alignment)
    {
      return new ColumnExtensionAttribute("Alignment", alignment);
    }

    public static ColumnExtensionAttribute CellBackColor<T>(Getter<Color?, T> cellBackColorGetter)
    {
      return CreateGetter("CellsBackColor", cellBackColorGetter);
    }

    public static ColumnExtensionAttribute CellBackColor(Getter<Color?, object> cellBackColorGetter)
    {
      return new ColumnExtensionAttribute("CellsBackColor", cellBackColorGetter);
    }

    public static ColumnExtensionAttribute CellBackColor(Color color)
    {
      return CellBackColor(delegate { return color; });
    }

    public static ColumnExtensionAttribute BackColorForCellValue<TRow, TValue>(
      Getter<Color?, TRow, TValue> colorGetter)
    {
      return new ColumnExtensionAttribute("BackColorForCellOrValue", new Getter<Color?, object, object>(
        delegate (object row, object value)
        {
          return colorGetter((TRow)row, (TValue)value);
        }));
    }

    public static ColumnExtensionAttribute CellHMarkingPen(Pen hMarkingPen)
    {
      return new ColumnExtensionAttribute("CellHMarkingPen", (object)(Getter<Pen, object>)delegate
      {
        return hMarkingPen;
      });
    }

    public static ColumnExtensionAttribute CellHMarkingPen<T>(Getter<Pen, T> hMarkingPenGetter)
    {
      return GridExt.CreateGetter<Pen, T>("CellHMarkingPen", hMarkingPenGetter);
    }

    public static ColumnExtensionAttribute CellVMarkingPen(Pen vMarkingPen)
    {
      return new ColumnExtensionAttribute("CellVMarkingPen", (object)(Getter<Pen, object>)delegate
      {
        return vMarkingPen;
      });
    }

    public static ColumnExtensionAttribute CellVMarkingPen<T>(Getter<Pen, T> vMarkingPenGetter)
    {
      return GridExt.CreateGetter<Pen, T>("CellVMarkingPen", vMarkingPenGetter);
    }

    public static ColumnExtensionAttribute CellSelectionDisabled(bool selectionDisabled)
    {
      return GridExt.CellSelectionDisabled<object>((Getter<bool, object>)(row => selectionDisabled));
    }

    public static ColumnExtensionAttribute CellSelectionDisabled<T>(Getter<bool, T> selectionDisabledGetter)
    {
      return GridExt.CreateGetter<bool, T>("CellSelectionDisabled", selectionDisabledGetter);
    }

    public static ColumnExtensionAttribute IsMultilineCell
    {
      get
      {
        return new ColumnExtensionAttribute("IsMultilineCell",
    new Getter<bool, object>(delegate (object row) { return true; }));
      }
    }

    public static ColumnExtensionAttribute IsReadOnlyCell<T>(Getter<bool, T> readOnlyCellGetter)
    {
      return CreateGetter("IsReadOnlyCell", readOnlyCellGetter);
    }

    public static ColumnExtensionAttribute IsReadOnlyCell(Getter<bool, object> readOnlyCellGetter)
    {
      return new ColumnExtensionAttribute("IsReadOnlyCell", readOnlyCellGetter);
    }

    //public static ColumnExtensionAttribute PixWidth(int pixWidth)
    //{
    //  return new ColumnExtensionAttribute("MinWidth", string.Format("pix:{0}", pixWidth));
    //}

    public static ColumnExtensionAttribute PixelWidth(int pixelWidth)
    {
      return new ColumnExtensionAttribute("MinWidth", string.Format("pixel:{0}", pixelWidth));
    }

    public static ColumnExtensionAttribute MaxPixelWidth(int pixelWidth)
    {
      return new ColumnExtensionAttribute("MaxWidth", string.Format("pixel:{0}", pixelWidth));
    }

    public static ColumnExtensionAttribute CharWidth(int charWidth)
    {
      return new ColumnExtensionAttribute("MinWidth", string.Format("char:{0}", charWidth));
    }

    public static ColumnExtensionAttribute PatternWidth(string pattern)
    {
      return new ColumnExtensionAttribute("MinWidth", string.Format("pat:{0}", pattern ?? ""));
    }

    public static ColumnExtensionAttribute MinCharWidth(int charWidth)
    {
      return CharWidth(charWidth);
    }

    public static ColumnExtensionAttribute MinPatternWidth(string pattern)
    {
      return PatternWidth(pattern);
    }

    public static ColumnExtensionAttribute MaxCharWidth(int charWidth)
    {
      return new ColumnExtensionAttribute("MaxWidth", string.Format("char:{0}", charWidth));
    }

    public static ColumnExtensionAttribute MaxPatternWidth(string pattern)
    {
      return new ColumnExtensionAttribute("MaxWidth", string.Format("pat:{0}", pattern ?? ""));
    }

    public static ColumnExtensionAttribute RelativeWidth(int widthInPercent)
    {
      return new ColumnExtensionAttribute("RelativeWidth", widthInPercent);
    }

    //public static ColumnExtensionAttribute IsFixedWidth
    //{
    //  get { return new ColumnExtensionAttribute("IsFixedWidth", true); }
    //}

    //public static ColumnExtensionAttribute FixedRelativeWidth(int widthInPercent)
    //{
    //  return new ColumnExtensionAttribute("FixedRelativeWidth", widthInPercent);
    //}

    public static ColumnExtensionAttribute NoDelayApply
    {
      get { return new ColumnExtensionAttribute("NoDelayApply", true); }
    }

    public static ColumnExtensionAttribute IsBlinkingColumnHeader
    {
      get { return new ColumnExtensionAttribute("IsBlinkingColumnHeader", true); }
    }

    public static ColumnExtensionAttribute Format(string format)
    {
      return new ColumnExtensionAttribute("Format", format);
    }

    public static ColumnExtensionAttribute Border(int borderWidth)
    {
      return new ColumnExtensionAttribute("Border", borderWidth);
    }

    public static ColumnExtensionAttribute Filter(IGridColumn filter)
    {
      return new ColumnExtensionAttribute("Filter", filter);
    }

    public static ColumnExtensionAttribute Filter<TRow>(IGridFilters<TRow> gridFilters,
      Getter<bool, TRow, object, object> isHideRowChecker, params ColumnExtensionAttribute[] extensions)
    {
      IGridColumn filterColumn = null;
      filterColumn = GridExt.Column("",
        delegate { return gridFilters.Storage.GetFilterValue(filterColumn); },
        delegate (object row, object value)
        {
          gridFilters.Storage.AssignFilterValue(filterColumn, value);
          gridFilters.GridSynch.ForceSynchronize();
        }, ArrayHlp.Merge(extensions,
        new ColumnExtensionAttribute[] { GridExt.NoDelayApply, GridExt.IsHideRowChecker(isHideRowChecker) })
      );
      return GridExt.Filter(filterColumn);
    }

    public static ColumnExtensionAttribute FilterForLikelyString<TRow>(IGridFilters<TRow> gridFilters,
      params ColumnExtensionAttribute[] extensions)
    {
      return Filter(gridFilters, delegate (TRow row, object columnValue, object filterValue)
      {
        return !(columnValue as string ?? "").ToLower().Contains((filterValue as string ?? "").ToLower());
      }, extensions);
    }

    public static ColumnExtensionAttribute FilterForCheckColumn<TRow>(IGridFilters<TRow> gridFilters,
      params ColumnExtensionAttribute[] extensions)
    {
      return Filter(gridFilters, delegate (TRow row, object columnValue, object filterValue)
      {
        bool? check = filterValue as bool?;
        if (check == null)
          return false;
        bool? value = columnValue as bool?;
        return value != check;
      }, ArrayHlp.Merge(new ColumnExtensionAttribute[] {
          GridExt.CellFiller(new CheckGridCellFiller(true, true)) }, extensions)
      );
    }

    public static ColumnExtensionAttribute HeaderImage(Getter<Image> headerImageGetter)
    {
      return new ColumnExtensionAttribute("HeaderImage", headerImageGetter);
    }

    public static ColumnExtensionAttribute HeaderImage(Image headerImage)
    {
      return HeaderImage(delegate { return headerImage; });
    }

    public static ColumnExtensionAttribute HeaderDescription(Getter<string> headerDescriptionGetter)
    {
      return new ColumnExtensionAttribute("HeaderDescription", headerDescriptionGetter);
    }

    public static ColumnExtensionAttribute HeaderDescription(string headerDescription)
    {
      return HeaderDescription(delegate { return headerDescription; });
    }

    public static ColumnExtensionAttribute CellDescription<T>(Getter<string, T> cellDescriptionGetter)
    {
      return CreateGetter("CellDescription", cellDescriptionGetter);
    }

    public static ColumnExtensionAttribute CellDescription(Getter<string, object> cellDescriptionGetter)
    {
      return new ColumnExtensionAttribute("CellDescription", cellDescriptionGetter);
    }

    public static ColumnExtensionAttribute CellDescription(string description)
    {
      return CellDescription(delegate (object row) { return description; });
    }

    public static ColumnExtensionAttribute IsVisible(Getter<bool> isVisibleGetter)
    {
      return new ColumnExtensionAttribute("IsVisible", isVisibleGetter);
    }

    public static ColumnExtensionAttribute CellFiller(IGridCellFiller gridCellFiller)
    {
      return new ColumnExtensionAttribute("CellFiller", gridCellFiller);
    }

    public static ColumnExtensionAttribute DisplayName(string displayName)
    {
      return DisplayName(AsGetter(displayName));
    }

    public static ColumnExtensionAttribute DisplayName(Getter<string> displayNameGetter)
    {
      return new ColumnExtensionAttribute("DisplayNameGetter", displayNameGetter);
    }

    public static ColumnExtensionAttribute ComboItems<T>(Getter<IEnumerable, T> comboItemsGetter)
    {
      return CreateGetter("ComboItems", comboItemsGetter);
    }

    public static ColumnExtensionAttribute ComboItems(Getter<IEnumerable, object> comboItemsGetter)
    {
      return new ColumnExtensionAttribute("ComboItems", comboItemsGetter);
    }

    public static ColumnExtensionAttribute ComboItems(IEnumerable comboItems)
    {
      return ComboItems(delegate
      {
        return comboItems;
      });
    }

    public static ColumnExtensionAttribute IsSubtitle
    {
      get
      {
        return new ColumnExtensionAttribute("IsSubtitle", true);
      }
    }

    public static ColumnExtensionAttribute IsOrCondition(Getter<bool, object> isOrConditionChecker)
    {
      return new ColumnExtensionAttribute("IsOrCondition", isOrConditionChecker);
    }

    public static ColumnExtensionAttribute IsHideRowChecker(Getter<bool, object, object, object> isHideRowChecker)
    {
      return new ColumnExtensionAttribute("IsHideRowChecker", isHideRowChecker);
    }

    public static ColumnExtensionAttribute IsHideRowChecker<TRow, TColumnValue, TFilterValue>(
      Getter<bool, TRow, TColumnValue, TFilterValue> isHideRowChecker)
    {
      return new ColumnExtensionAttribute("IsHideRowChecker", new Getter<bool, object, object, object>(
        delegate (object row, object rowValue, object filterValue)
        {
          return isHideRowChecker((TRow)row, (TColumnValue)rowValue, (TFilterValue)filterValue);
        }));
    }

    //public static ColumnExtensionAttribute ImageForCellOrValue(Image imageForColumn)
    //{
    //  return ImageForCellOrValue(delegate(object row, object value)
    //  {
    //    return imageForColumn;
    //  });
    //}

    //public static ColumnExtensionAttribute ImageForCellOrValue(Getter<Image, object, object> imageForValueGetter)
    //{
    //  return new ColumnExtensionAttribute("ImageForCellOrValue", imageForValueGetter);
    //}

    public static ColumnExtensionAttribute ImageForCell(Image image)
    {
      return new ColumnExtensionAttribute("ImageForCellOrValue", new Getter<Image, object, object>(
        delegate (object row, object value)
        {
          return image;
        }
      ));
    }

    public static ColumnExtensionAttribute ImageForCell<TRow>(Getter<Image, TRow> imageForCellGetter)
    {
      return new ColumnExtensionAttribute("ImageForCellOrValue", new Getter<Image, object, object>(
        delegate (object row, object value)
        {
          return imageForCellGetter((TRow)row);
        }));
    }

    public static ColumnExtensionAttribute ImageForCellValue<TValue>(Getter<Image, TValue> imageForValueGetter)
    {
      return new ColumnExtensionAttribute("ImageForCellOrValue", new Getter<Image, object, object>(
        delegate (object row, object value)
        {
          return imageForValueGetter((TValue)value);
        }));
    }

    public static ColumnExtensionAttribute Bold(bool isBoldFont)
    {
      return Bold(delegate (object row)
      {
        return isBoldFont;
      });
    }

    public static ColumnExtensionAttribute Bold<T>(Getter<bool?, T> isBoldFontGetter)
    {
      return CreateGetter("CellsBold", isBoldFontGetter);
    }

    //public static ColumnExtensionAttribute Bold(Getter<bool?, object> isBoldFontGetter)
    //{
    //  //return new ColumnExtensionAttribute("CellsBold", isBoldFontGetter);
    //  return new CellsBoldExtension(isBoldFontGetter);
    //}

    public static ColumnExtensionAttribute Italic<T>(Getter<bool?, T> isItalicFontGetter)
    {
      return CreateGetter("CellsItalic", isItalicFontGetter);
    }

    public static ColumnExtensionAttribute Italic(Getter<bool?, object> isItalicFontGetter)
    {
      return new ColumnExtensionAttribute("CellsItalic", isItalicFontGetter);
    }

    public static ColumnExtensionAttribute Italic(bool isItalic)
    {
      return Italic(delegate { return isItalic; });
    }

    public static ColumnExtensionAttribute FontSize(float fontSize)
    {
      return new ColumnExtensionAttribute("FontSize", fontSize);
    }

    public static ColumnExtensionAttribute FontName(string fontName)
    {
      return new ColumnExtensionAttribute("FontName", fontName);
    }

    public static ColumnExtensionAttribute ValueToString<T>(Getter<string, T> valueToStringConverter)
    {
      return CreateGetter("ValueToString", valueToStringConverter);
    }

    public static ColumnExtensionAttribute ValueToString(Getter<string, object> valueToStringConverter)
    {
      return new ColumnExtensionAttribute("ValueToString", valueToStringConverter);
    }
  }
}
