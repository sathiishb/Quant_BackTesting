using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Infrastructure.Data.Entities
{
    public class SymbolData
    {
        
        public int SymbolDataId { get; set; }

        
        public string Symbol { get; set; }
        public double Open { get; set; }    
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public DateTime DateTime { get; set; }
    }
}
