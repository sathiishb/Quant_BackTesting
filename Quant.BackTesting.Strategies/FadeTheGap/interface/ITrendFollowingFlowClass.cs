using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Model.Strategy;
using System;
using System.Collections.Generic;

namespace Quant.BackTesting.Strategies.FadeTheGap.Interface
{
    public interface ITrendFollowingFlowClass
    {
        List<ReportModel> GetGapUpStocks(List<HistoricalDataModel> data, TimeSpan endTime, TimeSpan previousDayCheck);
    }
}