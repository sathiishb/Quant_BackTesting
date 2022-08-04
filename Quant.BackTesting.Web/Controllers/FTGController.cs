using Microsoft.AspNetCore.Mvc;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Model.Strategy;
using Quant.BackTesting.PriceNetural.Epplus;
using Quant.BackTesting.Service.DataApi.Interface;
using Quant.BackTesting.Service.SyncData.Interface;
using Quant.BackTesting.Strategies.FadeTheGap.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Controllers
{
    public class FTGController : Controller
    {
        public IFadeTheGapFlowClass _fadeTheGapFlowClass { get; set; }
        private readonly IHistoricalDataPuller _historicalDataPuller;
        private readonly IHistoricalStockData _historicalStockData;
        private readonly ITrendFollowingFlowClass _trendFollowingFlowClass;
        public ISyncStockData _syncStockData { get; set; }
        public FTGController(IFadeTheGapFlowClass fadeTheGapFlowClass,
            ISyncStockData syncStockData,
            IHistoricalDataPuller historicalDataPuller,
            IHistoricalStockData historicalStockData,
            ITrendFollowingFlowClass trendFollowingFlowClass)
        {
            _fadeTheGapFlowClass = fadeTheGapFlowClass;
            _syncStockData = syncStockData;
            _historicalDataPuller = historicalDataPuller;
            _historicalStockData = historicalStockData;
            _trendFollowingFlowClass = trendFollowingFlowClass; 
        }

        //var token = HttpContext.Session.GetProfile().AccessToken;        
        //var stocks = FutureAndOptionBasket.GetStockName(backTestStartDate.Year);
        //var data = _historicalDataPuller.GetHistoricalData(token, backTestStartDate, backTestEndDate, "5", stocks);
        //_syncStockData.Sync(data);

        public IActionResult Index()
        {
            var backTestStartDate = new DateTime(2022, 01, 01);
            var backTestEndDate = new DateTime(2022, 06, 30);

            //var backTestStartDate = DateTime.Now.AddDays(-1);
            //var backTestEndDate = DateTime.Now;
            var endTime = new TimeSpan(15, 10, 00);
            var previousDayCheck = new TimeSpan(15, 25, 00);
            var fileName = backTestStartDate.ToString("MMM_yyyy") + " to " + backTestEndDate.ToString("MMM_yyyy") + $" EndTime {endTime.Hours}_{endTime.Minutes} PreDay{previousDayCheck.Hours}_{previousDayCheck.Minutes}";
            var token = HttpContext.Session.GetProfile().AccessToken;

            var data = _historicalStockData.GetFnoStockData(backTestStartDate, backTestEndDate);
            
            //var stocks = FutureAndOptionBasket.GetStockName(backTestStartDate.Year);
            //var data = _historicalDataPuller.GetHistoricalData(token, backTestStartDate, backTestEndDate, "5", stocks);
            //var ftg = _fadeTheGapFlowClass.GetStrategyPnl(data, endTime, previousDayCheck,token);
            var ftg = _trendFollowingFlowClass.GetGapUpStocks(data, endTime, previousDayCheck);

            var group = ftg.GroupBy(s => s.DateTime)
                .Select(s => new ReportModel()
                {
                    DateTime = s.Key,
                    PNL = s.Sum(x => x.PNL)
                }).ToList();

            ExcelConvertor.ExportToExcel(group, @"C:\Satz\Market-Data\BackTesting\FadeTheGap", fileName);

            return View();
        }
    }
}
