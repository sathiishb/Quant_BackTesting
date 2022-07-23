using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Strategy
{
    public class ReportModel
    {
        public bool IsBullish { get; set; }
        public DateTime DateTime { get; set; }
        
        public double PNL { get; set; }
        public string Symbol { get; set; }
    }
}
