using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Transcation
{
    public class OrderBookModel
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public double PNL { get; set; }
        public double CalculatePNL()
        {
            PNL = (SellPrice - BuyPrice) * Quantity;
            return PNL;
        }
        public double StopLoss { get; set; }
        public double Target { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
    }
}
