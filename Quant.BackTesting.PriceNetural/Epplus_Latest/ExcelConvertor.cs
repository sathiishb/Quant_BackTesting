using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Quant.BackTesting.PriceNetural.Epplus
{
    public static class ExcelConvertor
    {

        #region Extension methods for the datatable

        /// <summary>
        /// Export the List Record to Excel
        /// </summary>
        /// <param name="dataToBind">Record to Export</param>
        /// <param name="fileName">Name of the file</param>

        public static void ExportToExcel<T>(this List<T> dataToBind, string path, string fileName)
        {

            string sheetName = fileName;
            Byte[] fileBytes = ExcelStyleFormat.GetStyleAddedExcel(dataToBind, new ExcelStyleProperty(), sheetName);
            string fullpath = path + "//" + fileName + ".xlsx";
            File.WriteAllBytes(fullpath, fileBytes);
        }

        /// <summary>
        /// Export the List Record to Excel
        /// </summary>
        /// <param name="dataToBind">Record to Export</param>
        /// <param name="FileName">Name of the file</param>
        /// <param name="ExcelStyleProperty">Excel style</param>
        //public static void ExportToExcel<T>(this List<T> dataToBind, string FileName, ExcelStyleProperty ExcelStyleProperty)
        //{
        //    string sheetName = FileName;

        //    Byte[] fileBytes = ExcelStyleFormat.GetStyleAddedExcel(dataToBind, ExcelStyleProperty, sheetName);

        //    DownloadExcel(fileBytes, FileName);
        //}

        ///// <summary>
        ///// Extension methods passed the data to originl function
        ///// </summary>
        ///// <param name="dtDataToBind">Record to Export</param>
        ///// <param name="FileName">Name of the file</param>
        ///// <param name="ExcelStyleProperty">Excel style</param>
        ///// <param name="SheetName">First SheetName</param>
        //public static void ExportToExcel<T>(this List<T> dataToBind, string FileName, ExcelStyleProperty ExcelStyleProperty, string SheetName)
        //{

        //    Byte[] fileBytes = ExcelStyleFormat.GetStyleAddedExcel(dataToBind, ExcelStyleProperty, SheetName);
        //    DownloadExcel(fileBytes, FileName);
        //}

        #endregion



        #region ExportingFile
        /// <summary>
        /// Exporting the file 
        /// </summary>
        /// <param name="fileBytes">Excelfile in byte </param>
        /// <param name="FileName">Name of the Excel</param>
        //private static void DownloadExcel(Byte[] fileBytes, string FileName)
        //{
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.Buffer = true;
        //    HttpContext.Current.Response.Charset = "";
        //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".xlsx");
        //    HttpContext.Current.Response.BinaryWrite(fileBytes);
        //    HttpContext.Current.Response.Flush();
        //    HttpContext.Current.Response.End();
        //}

        //public static void DownloadZip(Byte[] fileBytes, string FileName)
        //{
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.Buffer = true;
        //    HttpContext.Current.Response.Charset = "";
        //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
        //    HttpContext.Current.Response.BinaryWrite(fileBytes);
        //    HttpContext.Current.Response.Flush();
        //    HttpContext.Current.Response.End();
        //}

        #endregion


    }
}