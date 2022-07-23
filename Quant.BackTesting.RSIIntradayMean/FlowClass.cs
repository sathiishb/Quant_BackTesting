using NetTrader.Indicator;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Service.DataApi.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.RSIIntradayMean
{
    //https://squareoff.in/rsi-intraday-trading-bot/
    //Rules are: 
    //1) Current day Closing price of the day > 200 day moving average
    //2) 2 period RSI > 50
    //3) closing price should be at least 3% above from the previous day close
    //4) Entry – short the stock the next day(Limit order) at 1% higher from the closing price
    //5) stop loss 3% and Target is 6%

    public class FlowClass : IFlowClass
    {
        private readonly IHistoricalDataPuller _historicalDataPuller;
        public FlowClass(IHistoricalDataPuller historicalDataPuller)
        {
            _historicalDataPuller = historicalDataPuller;
        }
        public void RsiMeanReversalStrategy(string token, DateTime fromDate, DateTime endDate)
        {
            //var backTestStartDate = new DateTime(2022, 01, 01);
            //var backTestEndDate = new DateTime(2022, 01, 28);
            var fileName = "RSIMeanReversal_" + fromDate.ToString("MMM_yyyy") + " to " + endDate.ToString("MMM_yyyy");
            var investment = 40000;

            var stocks = FutureAndOptionBasket.GetStockName(2021);
            foreach (var item in stocks)
            {
                var symbol = $"NSE:{item}-EQ";
                var requestHistoricalData = new HistoricalDataRequestModel()
                {
                    Symbol = symbol,
                    FromDateTime = fromDate.AddDays(-50),
                    ToDateTime = endDate,
                    TimeFrame = "D"
                };


                var historicalData = _historicalDataPuller.GetHistoricalData(token, requestHistoricalData);
                var quotes = historicalData.Select(s => new Ohlc()
                {
                    Close = s.Close,
                    Date = s.Date,
                    High = s.High,
                    Low = s.Low,
                    Open = s.Open,

                }).ToList();
               // IEnumerable<RsiResult> results = quotes.Use(CandlePart.Close).GetRsi(2);

                RSI rsi = new RSI(14);
                List<Ohlc> ohlcList = new List<Ohlc>();
                // fill ohlcList
                rsi.Load(quotes);
                RSISerie serie = rsi.Calculate();
            }


        }
    }
}
