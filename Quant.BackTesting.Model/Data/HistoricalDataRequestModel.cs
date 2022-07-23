using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Data
{
    public class HistoricalDataRequestModel
    {
        public string Symbol { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public string FormattedFromDateTime()
        {
            return FromDateTime.ToString("yyyy-MM-dd");
        }
        
        public string FormattedToDateTime()
        {
            return ToDateTime.ToString("yyyy-MM-dd");
        }
        public string TimeFrame { get; set; }
    }
}
