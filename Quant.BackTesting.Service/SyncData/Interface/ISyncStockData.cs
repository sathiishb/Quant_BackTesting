using Quant.BackTesting.Model.Data;
using System.Collections.Generic;

namespace Quant.BackTesting.Service.SyncData.Interface
{
    public interface ISyncStockData
    {
        void Sync(List<HistoricalDataModel> data);
    }
}