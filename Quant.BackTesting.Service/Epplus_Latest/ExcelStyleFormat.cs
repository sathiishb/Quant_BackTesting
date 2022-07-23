using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quant.BackTesting.Service.Epplus
{
    public static class ExcelStyleFormat
    {
        #region Code used to the set the style from the user

        /// <summary>
        /// Code used to the set the style from the user
        /// </summary>
        /// <param name="dataToBind">Record to Export</param>
        /// <param name="FileName">Name of the file</param>
        /// <param name="ExcelStyleProperty">Excel style</param>
        /// <param name="SheetName">First SheetName</param>
        public static Byte[] GetStyleAddedExcel<T>(List<T> dataToBind, ExcelStyleProperty ExcelStyleProperty, string SheetName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create Worksheet based on the given sheetname or filename.

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(SheetName);

                //Set the working area of the excel based on the data on the List.                
                int columnCount = typeof(T).GetProperties().Count();
                string LastColumn = GetExcelColumnName(columnCount - 1);

                //Header range
                StringBuilder HeaderRange = new StringBuilder("A1:" + LastColumn + "1");

                //Complete record range
                StringBuilder FullRange = new StringBuilder("A1:" + LastColumn + (dataToBind.Count + 1).ToString());

                //Load the record from the Datatable to Excel.
                ws.Cells["A1"].LoadFromCollection(dataToBind, true);

                //Fixed Rows and columns
                if (ExcelStyleProperty.FixedRow != 0 & ExcelStyleProperty.FixedColumn != 0)
                {
                    ws.View.FreezePanes(ExcelStyleProperty.FixedRow, ExcelStyleProperty.FixedColumn);
                }

                //HeaderColor,Pattern and Alignment
                Color HeaderBGColor = ExcelStyleProperty.HeaderBGColor;
                if (HeaderBGColor != Color.White)
                {
                    ws.Cells[HeaderRange.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[HeaderRange.ToString()].Style.Fill.BackgroundColor.SetColor(HeaderBGColor);
                }

                ws.Row(1).Style.VerticalAlignment = ExcelStyleProperty.HeaderVerticalAlign;
                ws.Cells.AutoFitColumns(ExcelStyleProperty.ColumnMinWidth);
                ws.Cells.AutoFitColumns(ExcelStyleProperty.ColumnMinWidth, ExcelStyleProperty.ColumnMaxWidth);
                ws.Row(1).Height = ExcelStyleProperty.HeaderHeight;
                //ws.Cells.Style.WrapText = true;               


                ws.Cells[FullRange.ToString()].Style.Border.Left.Style = ExcelStyleProperty.AllBorder;
                ws.Cells[FullRange.ToString()].Style.Border.Top.Style = ExcelStyleProperty.AllBorder;
                ws.Cells[FullRange.ToString()].Style.Border.BorderAround(ExcelStyleProperty.BoxBorder);

                
                Byte[] fileBytes = pck.GetAsByteArray();

                return fileBytes;
            }
        }

        #endregion

        #region Get columnName

        /// <summary>
        /// Get the column name in excel format
        /// </summary>
        /// <param name="columnIndex">column index</param>
        /// <returns>Column name in excel format</returns>
        public static String GetExcelColumnName(int columnIndex)
        {
            if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex: " + columnIndex);
            }
            Stack<char> stack = new Stack<char>();
            while (columnIndex >= 0)
            {
                stack.Push((char)('A' + (columnIndex % 26)));
                columnIndex = (columnIndex / 26) - 1;
            }
            return new String(stack.ToArray());
        }

        #endregion
    }
}
