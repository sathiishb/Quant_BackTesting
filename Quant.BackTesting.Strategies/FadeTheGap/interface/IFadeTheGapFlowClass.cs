using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Model.Strategy;
using System;
using System.Collections.Generic;

namespace Quant.BackTesting.Strategies.FadeTheGap.Interface
{
    public interface IFadeTheGapFlowClass
    {
        //void GetData(string token);

        List<ReportModel> GetStrategyPnl(List<HistoricalDataModel> data,TimeSpan endTime,
            TimeSpan previousDayCheck,
            string token);
        List<HistoricalDataModel> GetHistoricalDataForFNO(string token,
            DateTime backTestStartDate,
            DateTime backTestEndDate,
            int slice,
            string timeframe);
    }
}