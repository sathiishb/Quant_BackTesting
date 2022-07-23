using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Service.DataApi.Interface;
using Quant.BackTesting.Service.Epplus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.TrendFollowingSTBT
{
    public class FlowClass : IFlowClass
    {
        private readonly IHistoricalDataPuller _historicalDataPuller;

        public FlowClass(IHistoricalDataPuller historicalDataPuller)
        {
            _historicalDataPuller = historicalDataPuller;
        }

        public List<ReportModel> CalculatePoints(string token)
        {
            var fromDate = new DateTime(2021, 10, 01);
            var endDate = new DateTime(2021, 12, 31);

            return CalculatePoints(token, fromDate, endDate);
        }
        public List<ReportModel> CalculatePoints(string token, DateTime fromDate, DateTime endDate)
        {

            var symbol = $"NSE:NIFTY50-INDEX";
            var requestHistoricalData = new HistoricalDataRequestModel()
            {
                Symbol = symbol,
                FromDateTime = fromDate,
                ToDateTime = endDate.AddWorkDays(5),
                TimeFrame = "5"
            };

            var historicalData = _historicalDataPuller.GetHistoricalData(token, requestHistoricalData);

            var groupByDate = historicalData
                .GroupBy(s => s.Date.Date).ToList();
            var pnls = new List<ReportModel>();
            // for (int i = 0; i < groupByDate.Count - 1; i++)
            int i = 0;
            while (groupByDate[i].Key <= endDate)
            {
                try
                {
                    var entryDay = groupByDate[i];


                    //var open = entryDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(09, 15, 00)).Open;
                    //var close = entryDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(15, 20, 00)).Open;
                    //var OpenCloseDifference = close - open;
                    //var isBullish = OpenCloseDifference > 0;


                    //High and low
                    var low = entryDay.Min(s => s.Low);
                    var High = entryDay.Max(s => s.High);
                    var close = entryDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(15, 20, 00)).Open;
                    var highMinuclose = High - close;
                    var lowMinusClose = close - low;
                    var isBullish = lowMinusClose > highMinuclose;
                    var exitDay = groupByDate[i + 1];
                    var entryPrice = entryDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(15, 20, 00)).Open;
                    var exitPrice = exitDay.FirstOrDefault(s => s.Date.TimeOfDay == new TimeSpan(09, 20, 00)).Open;
                    double pnl = isBullish ? exitPrice - entryPrice : entryPrice - exitPrice;

                    pnls.Add(new ReportModel()
                    {
                        DateTime = entryDay.Key,
                        IsBullish = isBullish,
                        PNL = pnl
                    });
                }
                catch (Exception) { }

                i++;
            }



            //ExcelConvertor.ExportToExcel(pnls, @"C:\Satz\Market-Data\BackTesting\TrendFollowing", $"From-{fromDate:MM_dd_yyyy} to {endDate:MM_dd_yyyy}");

            //var sum = pnls.Sum(s => s.PNL);
            return pnls;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime AddWorkDays(this DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday)
                //&&!newDate.IsHoliday())
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        public static bool IsHoliday(this DateTime date)
        {
            // You'd load/cache from a DB or file somewhere rather than hardcode
            DateTime[] holidays =
            new DateTime[] {
      new DateTime(2010,12,27),
      new DateTime(2010,12,28),
      new DateTime(2011,01,03),
      new DateTime(2011,01,12),
      new DateTime(2011,01,13)
            };

            return holidays.Contains(date.Date);
        }
    }

    public class ReportModel
    {
        public bool IsBullish { get; set; }
        public DateTime DateTime { get; set; }
        public double PNL { get; set; }
    }
}
