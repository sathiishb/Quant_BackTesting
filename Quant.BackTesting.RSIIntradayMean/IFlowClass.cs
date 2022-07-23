using System;

namespace Quant.BackTesting.RSIIntradayMean
{
    public interface IFlowClass
    {
        void RsiMeanReversalStrategy(string token, DateTime fromDate, DateTime endDate);
    }
}