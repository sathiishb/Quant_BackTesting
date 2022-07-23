using Quant.BackTesting.Infrastructure.Data;
using Quant.BackTesting.Infrastructure.Data.Entities;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Service.SyncData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.Service.SyncData.Implementation
{
    public class SyncStockData : ISyncStockData
    {
        private readonly HistoricalDataContext _efContext;

        public SyncStockData(HistoricalDataContext efContext)
        {
            _efContext = efContext;
        }
        public void Sync(List<HistoricalDataModel> data)
        {
            var symbolData = data.Select(s => new SymbolData()
            {
                Close = s.Close,
                DateTime = s.Date,
                High = s.High,
                Low = s.Low,
                Open = s.Open,
                Symbol = s.Symbol,
                Volume = s.Volume
            });

            _efContext.AddRange(symbolData);
            _efContext.SaveChanges();

        }
    }
}
