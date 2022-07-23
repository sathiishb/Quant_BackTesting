using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Strategy
{
    public class MeanReversalFinderModel
    {
        public double LTP { get; set; }
        public double EMA { get; set; }
        public double Difference { get; set; }
        public string Symbol { get; set; }
    }
}
