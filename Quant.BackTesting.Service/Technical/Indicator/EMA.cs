using NetTrader.Indicator;
using Quant.BackTesting.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.Service.Indicator.Implementation
{
    public class ExponentialMovingAverageCalculator
    {

        public List<double?> CalculateEma(int Length, List<HistoricalDataModel> historicalDataModels)
        {
            var ohlc = historicalDataModels.Select(s => new Ohlc()
            {
                Close = s.Close,
                Date = s.Date,
                High = s.High,
                Low = s.Low,
                Open = s.Open
            });
            var ema = new EMA(Length, false);
            ema.Load(ohlc.ToList());
            var est = ema.Calculate().Values;

            return est;
        }

        //    public static double ExponentialMovingAverageFormula(double todaysPrice, double yesterdaysEMA, double numberOfDays)
        //    {
        //        double multiplier = (2 / (numberOfDays + 1));
        //        return (todaysPrice - yesterdaysEMA) * multiplier + yesterdaysEMA;
        //    }

        //    public static double CalculateExponentialMovingAverage(int numberOfDays, List<HistoricalDataModel> historicalDataList)
        //    {
        //        double yesterdaysEMA = 0;
        //        if (historicalDataList.Count >= numberOfDays)
        //        {
        //            double eMA = 0;
        //            double sumOfEMA = 0;
        //            for (int i = 0; i < numberOfDays; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    yesterdaysEMA = CalculateSimpleMovingAverage(numberOfDays, historicalDataList);
        //                }
        //                eMA = ExponentialMovingAverageFormula(historicalDataList[i].Close, yesterdaysEMA, numberOfDays);
        //                sumOfEMA += eMA;
        //                yesterdaysEMA = eMA;
        //            }

        //            return sumOfEMA / numberOfDays;
        //        }
        //        else
        //            return 0;
        //    }
    }
}
