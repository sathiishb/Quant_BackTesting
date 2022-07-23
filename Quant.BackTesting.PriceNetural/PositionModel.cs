using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.PriceNetural
{
    public class PositionModel
    {
        public DateTime DateTime { get; set; }

        public string Date => DateTime.ToString("MMMM dd yyyy hh:mm tt");
        public string Symbol { get; set; }
        public decimal SpotPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal PNL { get; set; }
       
        
    }
}
