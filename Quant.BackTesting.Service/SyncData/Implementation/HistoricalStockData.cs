using Quant.BackTesting.Infrastructure.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Service.SyncData.Interface;

namespace Quant.BackTesting.Service.SyncData.Implementation
{
    public class HistoricalStockData : IHistoricalStockData
    {
        private readonly HistoricalDataContext _efContext;

        public HistoricalStockData(HistoricalDataContext efContext)
        {
            _efContext = efContext;
        }
        public List<HistoricalDataModel> GetFnoStockData(DateTime fromDate, DateTime ToDate)
        {
            var data = _efContext.Stocks
                 .Where(s => s.DateTime >= fromDate && s.DateTime <= ToDate)
                 .Select(s => new HistoricalDataModel()
                 {
                     Close = s.Close,
                     Date = s.DateTime,
                     High = s.High,
                     Low = s.Low,
                     Open = s.Open,
                     Symbol = s.Symbol,

                 }).OrderBy(s=>s.Date)
                 .ToList();
            return data;
        }
    }
}
