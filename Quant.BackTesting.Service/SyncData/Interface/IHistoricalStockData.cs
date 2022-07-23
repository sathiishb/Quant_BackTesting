using Quant.BackTesting.Model.Data;
using System;
using System.Collections.Generic;

namespace Quant.BackTesting.Service.SyncData.Interface
{
    public interface IHistoricalStockData
    {
        List<HistoricalDataModel> GetFnoStockData(DateTime fromDate, DateTime ToDate);
    }
}