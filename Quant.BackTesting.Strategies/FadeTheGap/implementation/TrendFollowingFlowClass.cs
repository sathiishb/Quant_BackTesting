﻿using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Model.Strategy;
using Quant.BackTesting.Service.DataApi.Interface;
using Quant.BackTesting.Strategies.FadeTheGap.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.Strategies.FadeTheGap.implementation
{
    public class TrendFollowingFlowClass : ITrendFollowingFlowClass
    {
        private readonly IHistoricalDataPuller _historicalDataPuller;

        public TrendFollowingFlowClass(IHistoricalDataPuller historicalDataPuller)
        {
            _historicalDataPuller = historicalDataPuller;
        }

        public List<ReportModel> GetStrategyPnl(List<HistoricalDataModel> data, TimeSpan endTime,
            TimeSpan previousDayCheck)
        {


            var investment = 80000;
            var entryTime = new TimeSpan(09, 30, 00);
            var groupByDate = data
              .GroupBy(s => s.Date.Date).ToList();


            var fTGModel = new List<FTGModel>();
            var pnls = new List<ReportModel>();

            foreach (var currentDay in groupByDate)
            {
                //Not looping the first date
                if (currentDay.Key == groupByDate.FirstOrDefault().Key) continue;

                try
                {
                    var symbols = currentDay.GroupBy(s => s.Symbol).ToList();
                    foreach (var item in symbols)
                    {
                        var currentDate = currentDay.Key;
                        var entryDay = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == entryTime
                            && s.Symbol == item.Key)?.Open;

                        var previousDay = groupByDate.FirstOrDefault(s => s.Key == currentDate.AddDays(-1));
                        double? previousDayClose = previousDay.FirstOrDefault(s => s.Date.TimeOfDay == previousDayCheck && s.Symbol == item.Key)?.Close;


                        if (entryDay.HasValue == false || previousDayClose.HasValue == false || previousDayClose.Value < 50) continue;


                        var change = entryDay.Value - previousDayClose.Value;
                        var changeInPercentage = Math.Abs((change / previousDayClose.Value) * 100);


                        fTGModel.Add(new FTGModel()
                        {
                            ChangePercentage = changeInPercentage,
                            DateTime = currentDate,
                            Symbol = item.Key,
                            Change = change
                        });
                    }



                }
                catch (Exception) { }
            }

            var result = fTGModel.Where(s => s.ChangePercentage >= 3)
                .GroupBy(s => s.DateTime)
                .ToList();

            foreach (var item in result)
            {

                var topFive = item.OrderByDescending(s => s.ChangePercentage)
                    .Take(4);

                foreach (var top in topFive)
                {
                    var currentDay = groupByDate.FirstOrDefault(s => s.Key.Date == top.DateTime.Date)
                         .Where(x => x.Symbol == top.Symbol)
                         .ToList();
                    var entryPrice = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == entryTime).Open;
                    var exitTimePrice = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == endTime)?.Close;



                    if (exitTimePrice.HasValue == false) continue;

                    var bullish = top.Change > 0;
                    var qty = Math.Round(investment / entryPrice);
                    if (bullish == false)
                    {
                        var Sl = entryPrice * 1.02;
                        double exitPrice = 0;

                        foreach (var fiveMinuteData in currentDay)
                        {
                            if (fiveMinuteData.High >= Sl)
                            {
                                exitPrice = Sl;
                                break;
                            }
                        }
                        exitPrice = exitPrice == 0 ? exitTimePrice.Value : exitPrice;
                        pnls.Add(new ReportModel()
                        {
                            DateTime = currentDay.First().Date,
                            IsBullish = true,
                            PNL = qty * (entryPrice - exitPrice),
                            Symbol = top.Symbol
                        });
                    }
                    else
                    {
                        var Sl = entryPrice * 0.98;
                        double exitPrice = 0;
                        foreach (var fiveMinuteData in currentDay)
                        {
                            if (fiveMinuteData.Low <= Sl)
                            {
                                exitPrice = Sl;
                                break;
                            }
                        }
                        exitPrice = exitPrice == 0 ? exitTimePrice.Value : exitPrice;
                        pnls.Add(new ReportModel()
                        {
                            DateTime = currentDay.First().Date,
                            IsBullish = false,
                            PNL = qty * (exitPrice - entryPrice),
                            Symbol = top.Symbol
                        });
                    }

                }

            }

            return pnls;

        }


    }

    //public class FTGModel
    //{
    //    public double ChangePercentage { get; set; }
    //    public string Symbol { get; set; }
    //    public DateTime DateTime { get; set; }
    //    public double Change { get; set; }
    //}
}
