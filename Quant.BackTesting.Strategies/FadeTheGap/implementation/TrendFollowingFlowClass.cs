using Quant.BackTesting.Model.Data;
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

        public List<ReportModel> GetGapUpStocks(List<HistoricalDataModel> data, TimeSpan endTime,
            TimeSpan previousDayCheck)
        {
            var investment = 80000;
            //var entryTime = new TimeSpan(09, 30, 00);
            var historicalDataByDate = data
              .GroupBy(s => s.Date.Date).ToList();


            var fTGModel = new List<FTGModel>();


            foreach (var currentDay in historicalDataByDate)
            {
                //Not looping the first date
                if (currentDay.Key == historicalDataByDate.FirstOrDefault().Key) continue;

                try
                {
                    var symbols = currentDay.GroupBy(s => s.Symbol).ToList();
                    foreach (var item in symbols)
                    {
                        var currentDate = currentDay.Key;
                        var entryDay = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(9,15,0)
                            && s.Symbol == item.Key)?.Open;

                        var previousDay = historicalDataByDate.FirstOrDefault(s => s.Key == currentDate.AddDays(-1));
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

            var gapUpStocks = fTGModel.Where(s => s.ChangePercentage >= 2)
                .GroupBy(s => s.DateTime)
                .ToList();

            return ApplyStrategy(endTime, investment, historicalDataByDate, gapUpStocks);



        }

        public List<ReportModel> ApplyStrategy(TimeSpan endTime, int investment,
            List<IGrouping<DateTime, HistoricalDataModel>> historicalData,
            List<IGrouping<DateTime, FTGModel>> GapUpStocks)
        {
            var pnls = new List<ReportModel>();
            foreach (var item in GapUpStocks)
            {

                var topFive = item.OrderByDescending(s => s.ChangePercentage)
                    .Take(4);

                foreach (var top in topFive)
                {
                    var currentDay = historicalData.FirstOrDefault(s => s.Key.Date == top.DateTime.Date)
                         .Where(x => x.Symbol == top.Symbol)
                         .ToList();
                    var bullish = top.Change > 0;
                    var openPrice = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(9, 15, 0)).Open;
                    var LimitPrice = bullish ? openPrice * 0.99 : openPrice * 1.01;
                    var exitByEodPrice = currentDay.FirstOrDefault(s => s.Date.TimeOfDay == endTime).Close;

                    double entryPrice = 0;
                    double Sl = 0;
                    double exitPrice = 0;
                    int qty = 0;

                    // if (exitTimePrice.HasValue == false) continue;


                 
                    if (bullish == false)
                    {
                        foreach (var fiveMinuteData in currentDay)
                        {
                            if (entryPrice == 0 && fiveMinuteData.High >= LimitPrice)
                            {
                                entryPrice = LimitPrice;
                                Sl = entryPrice * 1.015;
                                qty = int.Parse(Math.Round(investment / entryPrice).ToString());
                            }
                            if (entryPrice != 0 && fiveMinuteData.High >= Sl)
                            {
                                exitPrice = Sl;
                                break;
                            }
                        }
                        exitPrice = exitPrice == 0 ? exitByEodPrice : exitPrice;
                        pnls.Add(new ReportModel()
                        {
                            DateTime = currentDay.First().Date,
                            IsBullish = true,
                            PNL = qty * (LimitPrice - exitPrice),
                            Symbol = top.Symbol
                        });
                    }
                    else
                    {
                       
                        foreach (var fiveMinuteData in currentDay)
                        {
                            if (entryPrice == 0 && fiveMinuteData.Low <= LimitPrice)
                            {
                                entryPrice = LimitPrice;
                                Sl = entryPrice * 0.985;
                                qty = int.Parse(Math.Round(investment / entryPrice).ToString());
                            }
                            if (entryPrice != 0 && fiveMinuteData.Low <= Sl)
                            {
                                exitPrice = Sl;
                                break;
                            }
                        }
                        exitPrice = exitPrice == 0 ? exitByEodPrice : exitPrice;
                        pnls.Add(new ReportModel()
                        {
                            DateTime = currentDay.First().Date,
                            IsBullish = false,
                            PNL = qty * (exitPrice - LimitPrice),
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
