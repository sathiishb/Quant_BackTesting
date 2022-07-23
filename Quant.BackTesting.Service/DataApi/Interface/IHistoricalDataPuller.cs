using Quant.BackTesting.Model.Data;
using System;
using System.Collections.Generic;

namespace Quant.BackTesting.Service.DataApi.Interface
{
    public interface IHistoricalDataPuller
    {
        List<HistoricalDataModel> GetHistoricalData(string token, HistoricalDataRequestModel requestHistoricalData);
        public List<HistoricalDataModel> GetHistoricalData(string token,
               DateTime startDate,
               DateTime endDate,
               string timeFrame,
               List<string> stocks);
    }
}