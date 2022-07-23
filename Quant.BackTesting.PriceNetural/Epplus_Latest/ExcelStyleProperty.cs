using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel;
using OfficeOpenXml.Style;

namespace Quant.BackTesting.PriceNetural.Epplus
{
    public class ExcelStyleProperty
    {          
       
        private int _FixedRow;
        public int FixedRow
        {
            get { return _FixedRow; }
            set
            {
                _FixedRow = value + 1;
            }
        }

        private int _FixedColumn;
        public int FixedColumn
        {
            get { return _FixedColumn; }
            set
            {
                _FixedColumn = value + 1;

            }
        }

        private int _ColumnMinWidth = 20;
        public int ColumnMinWidth
        {
            get { return _ColumnMinWidth; }
            set
            {
                if (ColumnMinWidth != 0)
                {
                    _ColumnMinWidth = value;
                }


            }
        }    

        private int _ColumnMaxWidth = 50;
        public int ColumnMaxWidth
        {
            get { return _ColumnMaxWidth; }
            set
            {
                if (ColumnMaxWidth != 0)
                {
                    _ColumnMaxWidth = value;
                }


            }
        }


        private Color _HeaderBGColor = Color.White;
        public Color HeaderBGColor
        {
            get { return _HeaderBGColor; }
            set { _HeaderBGColor = value; }
        }

        private ExcelBorderStyle _AllBorder = ExcelBorderStyle.None;
        public ExcelBorderStyle AllBorder
        {
            get { return _AllBorder; }
            set { _AllBorder = value; }
        }

        private ExcelBorderStyle _BoxBorder = ExcelBorderStyle.None;
        public ExcelBorderStyle BoxBorder
        {
            get { return _BoxBorder; }
            set { _BoxBorder = value; }
        }

        private int _HeaderHeight = 15;
        public int HeaderHeight
        {
            get { return _HeaderHeight; }
            set { _HeaderHeight = value; }
        }

        private ExcelVerticalAlignment _HeaderVerticalAlign = ExcelVerticalAlignment.Center;
        public ExcelVerticalAlignment HeaderVerticalAlign
        {
            get { return _HeaderVerticalAlign; }
            set { _HeaderVerticalAlign = value; }
        }
       
        
    }
}