using System;
using System.Drawing;
using System.IO;

using Excel =  Microsoft.Office.Interop.Excel;


/// <summary>
/// Класс для работы с Excel.
/// </summary>
public class ExcelClass : IDisposable
{
    Excel.ApplicationClass _xlApp;
    Excel.Workbook _xlWorkBook;      
    Excel.Worksheet _xlWorkSheet;     
    Excel.Range _range;
    Excel.Pictures _p;
    Excel.Picture _pic;
    private readonly object _misValue = Type.Missing;
    bool _disposed;


    private const string _TEMPLATE_PATH = @"D:\USP\UchetUSP\Templates\";
    //private string templatePath = UchetUSP.Program.PathString + "\\";


    /// <summary>
    /// Конструктор класса.
    /// </summary>
    public ExcelClass()
    {
        _xlApp = new Excel.ApplicationClass();
    }


    //ВИДИМОСТЬ EXCEL
    public bool Visible
    {
        set
        {
            if (false == value)
                _xlApp.Visible = false;

            else
                _xlApp.Visible = true;
        }
    }



    /// <summary>
    /// СОЗДАТЬ НОВЫЙ ДОКУМЕНТ
    /// </summary>
    public void NewDocument()
    {
        _xlWorkBook = _xlApp.Workbooks.Add(_misValue);
        _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.get_Item(1);

        //_xlApp.Visible = true;
        //_xlApp.UserControl = true;
    }

    //СОЗДАТЬ НОВЫЙ ДОКУМЕНТ C ШАБЛОНОМ
    public void NewDocument(string templateName)
    {
        _xlWorkBook = _xlApp.Workbooks.Add(_TEMPLATE_PATH + templateName);
        _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.get_Item(1);
    }

    /// <summary>
    /// Открывает Excel-документ.
    /// </summary>
    /// <param name="fileName">Путь к Excel файлу.</param>
    /// <param name="isVisible">Отображать ли документ при открытии.</param>
    public void OpenDocument(string fileName, bool isVisible)
    {
        _xlWorkBook = _xlApp.Workbooks.Open(fileName, _misValue, _misValue, _misValue, _misValue,
            _misValue, _misValue, _misValue, _misValue, _misValue, _misValue,
            _misValue, _misValue, _misValue, _misValue);
        _xlApp.Visible = isVisible;
        _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.get_Item(1);
    }

    /// <summary>
    /// Возвращает количество листов в книге Excel.
    /// </summary>
    /// <returns></returns>
    public int GetNShieets()
    {
        return _xlWorkBook.Worksheets.Count;
    }


    /// <summary>
    /// СОХРАНИТЬ ДОКУМЕНТ
    /// </summary>
    /// <param name="name"></param>
    public void SaveDocument(string name)
    {
        _xlApp.DisplayAlerts = true;
        if (File.Exists(name))
        {
            File.Delete(name);
        }
        _xlWorkBook.SaveAs(name, Excel.XlFileFormat.xlWorkbookDefault, _misValue, _misValue, _misValue, _misValue, Excel.XlSaveAsAccessMode.xlExclusive, _misValue, _misValue, _misValue, _misValue, _misValue);
    }

    //Выделить ячейки
    public void SelectCells(Object start, Object end)
    {
        if (start == null)
        {
            start = _misValue;
        }

        if (end == null)
        {
            start = _misValue;
        }            
        _range = _xlWorkSheet.Range[start, end];
    }

    //Скопировать выделенные ячейки
    public void CopyTo(Object cell)
    {
        Excel.Range rangeDest = _xlWorkSheet.get_Range(cell, _misValue);
        _range.Copy(rangeDest);
    }

    //Выделить лист
    public void SelectWorksheet(int count)
    {
        if (count <= _xlWorkBook.Worksheets.Count)
        {
            _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.get_Item(count);
        }
        
    }



    //УСТАНОВКА ЦВЕТА ФОНА ЯЧЕЙКИ

    public void SetColor(int color)
    {            
        _range.Interior.Color = color;
        _range.Interior.PatternColorIndex = Excel.Constants.xlAutomatic;
    }

    public void SetColor(string colLetter, int rowNumber, object color)
    {
        _range = _xlWorkSheet.get_Range(colLetter + rowNumber, Type.Missing);
        _range.Interior.Color = color;
        _range.Interior.PatternColorIndex = Excel.Constants.xlAutomatic;
    }

    //ОРИЕНТАЦИИ СТРАНИЦЫ
   
    public void SetOrientation(Excel.XlPageOrientation orientation)
    {
        _xlWorkSheet.PageSetup.Orientation = orientation;
    }

    //УСТАНОВКА РАЗМЕРОВ ПОЛЕЙ ЛИСТА
    public void SetMargin(double left, double right, double top, double bottom)
    {
        //Range.PageSetup.LeftMargin - ЛЕВОЕ
        //Range.PageSetup.RightMargin - ПРАВОЕ 
        //Range.PageSetup.TopMargin - ВЕРХНЕЕ
        //Range.PageSetup.BottomMargin - НИЖНЕЕ

        _xlWorkSheet.PageSetup.RightMargin = right;
        _xlWorkSheet.PageSetup.LeftMargin = left;
        _xlWorkSheet.PageSetup.TopMargin = top;
        _xlWorkSheet.PageSetup.BottomMargin = bottom;

    }


    //УСТАНОВКА РАЗМЕРА ЛИСТА
    public void SetPaperSize(Excel.XlPaperSize size)
    {
        _xlWorkSheet.PageSetup.PaperSize = size;
    }

    //УСТАНОВКА МАСШТАБА ПЕЧАТИ
    public void SetZoom(int percent)
    {
        _xlWorkSheet.PageSetup.Zoom = percent;
                
    }

    
    //ПЕРЕИМЕНОВАТЬ ЛИСТ
    private void ReNamePage(int n, string name)
    {

        _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.get_Item(n);

        _xlWorkSheet.Name = name;
    }

    //ДОБАВЛЕНИЕ ЛИСТА В НАЧАЛО СПИСКА
    public void AddNewPageAtTheStart(string name)
    {          
       _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.Add(_misValue, _misValue, _misValue, _misValue);

       ReNamePage(_xlWorkSheet.Index, name); 
       
    }

    //ДОБАВЛЕНИЕ ЛИСТА В Конец Списка
    public void AddNewPageAtTheEnd(string name)
    {
        _xlWorkSheet = (Excel.Worksheet)_xlWorkBook.Worksheets.Add(_misValue, _xlWorkBook.Worksheets.get_Item(_xlWorkBook.Worksheets.Count), 1, _misValue);

        ReNamePage(_xlWorkSheet.Index, name);

    }

    //ПРИМЕНЕНИЕ ШРИФТА К ЯЧЕЙКЕ
    public void SetFont(Excel.Font font,int colorIndex)
    {            
        _range.Font.Size = font.Size;
        _range.Font.Bold = font.Bold;
        _range.Font.Italic = font.Italic;
        _range.Font.Name = font.Name;
        _range.Font.ColorIndex = colorIndex;
    }


    //ЗАПИСЬ ЗНАЧЕНИЯ В ЯЧЕЙКУ
    public void SetCellValue(string value)
    {
        _range.Value2 = value;
    }
    
    /// <summary>
    /// Записывает значение в ячейку.
    /// </summary>
    /// <param name="cell">Адресс ячейки.</param>
    /// <param name="value">Значение ячейки.</param>
    public void SetCellValue(string cell, string value)
    {
        _range = _xlWorkSheet.Range[cell, _misValue];
        _range.Value2 = value;
    }

    /// <summary>
    /// Записывает значение в ячейку.
    /// </summary>
    /// <param name="rowI">Номер строки.</param>
    /// <param name="colI">Номер столбца.</param>
    /// <param name="value">Значение ячейки.</param>
    public void SetCellValue(int colI, int rowI, string value)
    {
        _range = _xlWorkSheet.Range[_xlWorkSheet.Cells[rowI, colI], _xlWorkSheet.Cells[rowI, colI]];
        _range.Value2 = value;
    }

    /// <summary>
    /// Записывает значение в ячейку.
    /// </summary>
    /// <param name="column">Номер столбца ячейки.</param>
    /// <param name="row">Номер строки ячейки.</param>
    /// <param name="value">Значение ячейки.</param>
    public void SetCellValue(string column, int row, string value)
    {
        SetCellValue(column + row, value);
    }

    /// <summary>
    /// Добавляет к имеющимся данным в ячейке новые
    /// </summary>
    /// <param name="cell">Номер ячейки в формате "A1"</param>
    /// <param name="addValue">Данные</param>
    public void AddValueToCell(string cell, string addValue)
    {
        _range = _xlWorkSheet.get_Range(cell, Type.Missing);
        _range.Value2 += addValue;
    }

    //ОБЪЕДИНЕНИЕ ЯЧЕЕК
    public void SetMerge()
    {
        _range.Merge(_misValue);
    }

    //УСТАНОВКА ШИРИНЫ СТОЛБЦОВ
    public void SetColumnWidth(double width)
    {
        _range.ColumnWidth = width;           
    }

    //УСТАНОВКА НАПРАВЛЕНИЯ ТЕКСТА
    public void SetTextOrientation(int orientation)
    {
        _range.Orientation = orientation;           
    }

    //ВЫРАВНИВАНИЕ ТЕКСТА В ЯЧЕЙКЕ ПО ВЕРТИКАЛИ
    public void SetVerticalAlignment(int alignment)
    {
        _range.VerticalAlignment = alignment;
    }

    //ВЫРАВНИВАНИЕ ТЕКСТА В ЯЧЕЙКЕ ПО ГОРИЗОНТАЛИ
    public void SetHorisontalAlignment(int alignment)
    {
        _range.HorizontalAlignment = alignment;   
    }



    //ПЕРЕНОС СЛОВ В ЯЧЕЙКЕ
    public void SetWrapText(bool value)
    {
        _range.WrapText = value;            
    }

    //УСТАНОВКА ВЫСОТЫ СТРОКИ
    public void SetRowHeight(double height)
    {
        _range.RowHeight = height;         
    }

    //УСТАНОВКА ВИДА ГРАНИЦ
    public void SetBorderStyle(int color, Excel.XlLineStyle lineStyle, Excel.XlBorderWeight weight)
    {
        _range.Borders.ColorIndex = color;
        _range.Borders.LineStyle = lineStyle;
        _range.Borders.Weight = weight;
    }

    //ЧТЕНИЕ ЗНАЧЕНИЯ ИЗ ЯЧЕЙКИ
    public string GetValue()
    {
        return _range.Value2.ToString();
    }

    /// <summary>
    /// Возвращает данные из ячейки.
    /// </summary>
    /// <param name="cellAdress">Номер ячейки в формате "A1".</param>
    /// <returns></returns>
    public object GetCellValue(string cellAdress)
    {
        return GetCellValue(_xlWorkSheet.get_Range(cellAdress, Type.Missing));
    }

    private object GetCellValue(Excel.Range range)
    {
        return range.Value2;
    }

    /// <summary>
    /// Возвращает данные из ячейки в виде строки. Если ячейка без данных, возвращается пустая строка.
    /// </summary>
    /// <param name="iRow">Номер строки.</param>
    /// <param name="iCol">Номер столбца.</param>
    /// <returns></returns>
    public string GetCellStringValue(int iCol, int iRow)
    {
        object o =
            GetCellValue((Excel.Range)_xlWorkSheet.Cells[iRow, iCol]);
        return o == null ? "" : o.ToString();
    }

    /// <summary>
    /// Возвращает данные из ячейки в виде строки. Если ячейка без данных, возвращается пустая строка.
    /// </summary>
    /// <param name="cellAdress">Номер ячейки в формате "A1".</param>
    /// <returns></returns>
    public string GetCellStringValue(string cellAdress)
    {
        object o = GetCellValue(cellAdress);
        return o == null ? "" : o.ToString();
    }

    /// <summary>
    /// Возвращает данные из ячейки в виде строки. Если ячейка без данных, возвращается пустая строка.
    /// </summary>
    /// <param name="colLetter">Буква столбца ячейки.</param>
    /// <param name="nRow">Номер строки ячейки.</param>
    /// <returns></returns>
    public string GetCellStringValue(string colLetter, int nRow)
    {
        return GetCellStringValue(colLetter + nRow);
    }

    public bool CellIsWhiteSpace(string cellAdress)
    {
        string cell = GetCellStringValue(cellAdress);
        if (cell == null)
        {
            return true;
        }
        if (cell.Trim() == "")
        {
            return true;
        }
        return false;
    }

    public bool CellIsWhiteSpace(string colLetter, int nRow)
    {
        return CellIsWhiteSpace(colLetter + nRow);
    }

    /// <summary>
    /// Возвращает true, если ячейка пустая("") или без данных(NULL).
    /// </summary>
    /// <param name="cellAdress">Номер ячейки в формате "A1".</param>
    /// <returns></returns>
    public bool CellIsNullOrVoid(string cellAdress)
    {
        return GetCellStringValue(cellAdress) == "";
    }

    /// <summary>
    /// Возвращает true, если ячейка пустая("") или без данных(NULL).
    /// </summary>
    /// <param name="iRow">Номер строки.</param>
    /// <param name="iCol">Номер столбца.</param>
    /// <returns></returns>
    public bool CellIsNullOrVoid(int iCol, int iRow)
    {
        return GetCellStringValue(iCol, iRow) == "";
    }

    /// <summary>
    /// Возвращает true, если ячейка пустая("") или без данных(NULL).
    /// </summary>
    /// <param name="colLetter">Буква столбца ячейки.</param>
    /// <param name="nRow">Номер строки ячейки.</param>
    /// <returns></returns>
    public bool CellIsNullOrVoid(string colLetter, int nRow)
    {
        return CellIsNullOrVoid(colLetter + nRow);
    }

    /// <summary>
    /// Возвращает номер цвета фона ячейки.
    /// </summary>
    /// <param name="colLetter">Буква столбца ячейки.</param>
    /// <param name="nRow">Номер строки ячейки.</param>
    /// <returns></returns>
    public int GetCellColorIndex(string colLetter, int nRow)
    {
        _range = _xlWorkSheet.get_Range(colLetter + nRow, Type.Missing);
        return (int)_range.Interior.ColorIndex;
    }


    /// <summary>
    /// Закрыть документ с сохранением.
    /// </summary>
    public void CloseDocumentSave()
    {
        CloseDocument(true);
    }

    /// <summary>
    /// Закрыть документ
    /// </summary>
    /// <param name="save">C сохранением</param>
    public void CloseDocument(bool save)
    {
        if (_xlApp.ActiveWorkbook != null)
        {
            _xlWorkBook.Close(save, _misValue, _misValue);
        }
        _xlApp.Quit();
    }


    //Завершить процесс
    public void Exit()
    {
        _xlApp.Quit();
    }


    private void releaseObject(object obj)
    {
        if (obj == null)
            return;
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        }
        finally
        {
            GC.Collect();
        }
    }

    //ЗАПИСЬ КАРТИНКИ В ЯЧЕЙКУ
    public void AddPicture(string path, string cellAdress)
    {
        SelectCells(cellAdress, cellAdress);
        _p = _xlWorkSheet.Pictures(_misValue) as Excel.Pictures;           
        _pic = _p.Insert(path, _misValue);
        _pic.Left = Convert.ToDouble(_range.Left);
        _pic.Top = Convert.ToDouble(_range.Top);            
    }

    /// <summary>
    /// Метод копирует выбранный диапазон ячеек на новое место
    /// </summary>
    /// <param name="start">Левая верхняя ячейка диапазона</param>
    /// <param name="end">Правая нижняя ячейка диапазона</param>
    /// <param name="destination">Ячейка нового местоположения</param>
    public void CopyCells(object start, object end, object destination)
    {
        Excel.Range rangeDest = _xlWorkSheet.Range[destination, _misValue];
        _range = _xlWorkSheet.Range[start, end];
        _range.Copy(rangeDest);
    }

    /// <summary>
    /// Метод делает жирным выделенные ячейки
    /// </summary>
    /// <param name="start">Начало диапазона</param>
    /// <param name="end">Конец диапазона</param>
    public void SetBold(object start, object end)
    {
        SelectCells(start, end);
        _range.Font.Bold = true;
    }

    /// <summary>
    /// Метод выравнивает ширину столбцов по max ячейке
    /// </summary>
    /// <param name="columnName">Наименование столбца/столбцов в формате "А:А"</param>
    public void SetAutoFit(string columnName)
    {
        _range = _xlWorkSheet.Range[columnName, _misValue];
        _range.Columns.AutoFit();
    }

    /// <summary>
    /// Метод добавляет пустую строку.
    /// </summary>
    /// <param name="rowNum">Номер строки</param>
    public void AddRow(int rowNum)
    {
        _range = (Excel.Range)_xlWorkSheet.Rows[rowNum, _misValue];
        _range.Select();
        _range.Insert(Excel.XlInsertShiftDirection.xlShiftDown, _misValue);

    }

    //Уничтожение объекта
    //без этого процесс висит
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            //handle.Dispose();
            // Free any other managed objects here.
            //
        }

        // Free any unmanaged objects here.
        releaseObject(_pic);
        releaseObject(_p);
        releaseObject(_range);
        releaseObject(_xlWorkSheet);
        releaseObject(_xlWorkBook);
        releaseObject(_xlApp);
        _disposed = true;
    }

}
