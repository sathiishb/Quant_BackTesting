using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Data
{
    public class HistoricalDataModel
    {
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }

        public string Symbol { get; set; }
    }
}
