using FluentDateTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Model.Strategy;
using Quant.BackTesting.Model.Transcation;
using Quant.BackTesting.PriceNetural.Epplus;
using Quant.BackTesting.Service.DataApi.Implementation;
using Quant.BackTesting.Service.DataApi.Interface;
using Quant.BackTesting.Service.Indicator.Implementation;
using Quant.BackTesting.Web.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IHistoricalDataPuller _historicalDataPuller;
        public HomeController(ILogger<HomeController> logger,
            IConfiguration config,
            IHistoricalDataPuller historicalDataPuller)
        {
            _logger = logger;
            _config = config;
            _historicalDataPuller = historicalDataPuller;
        }

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetProfile().AccessToken;



            var backTestStartDate = new DateTime(2018, 01, 01);
            var backTestEndDate = new DateTime(2019, 01, 28);
            var fileName = backTestStartDate.ToString("MMM_yyyy") + " to " + backTestEndDate.ToString("MMM_yyyy");
            var investment = 40000;
            List<OrderBookModel> orderBook = new List<OrderBookModel>();
            var startDate = backTestStartDate;
            var endDate = backTestStartDate.AddDays(90);
            while (endDate <= backTestEndDate)
            {
                orderBook.AddRange(GetThreeMonthsResult(token, startDate, endDate, investment));
                startDate = endDate.AddDays(1);
                endDate = endDate.AddDays(90);
            }
            var groupbyReport = orderBook.GroupBy(s => s.EntryTime)
                .Select(s => new
                {
                    date = s.Key,
                    PNL = s.Sum(x => (x.SellPrice - x.BuyPrice) * x.Quantity),
                    Count = s.Count()
                }).ToList();
            ExcelConvertor.ExportToExcel(orderBook, @"C:\Satz\Market-Data\BackTesting\IntraDayMeanReversal", fileName);
            ExcelConvertor.ExportToExcel(groupbyReport, @"C:\Satz\Market-Data\BackTesting\IntraDayMeanReversal", fileName + "Group");

            return View();
        }

        private List<OrderBookModel> GetThreeMonthsResult(string token,
            DateTime startDate,
            DateTime endDate,
            int investment)
        {
            var historicalDataModels = new List<HistoricalDataModel>();
            var emaFinder = new List<MeanReversalFinderModel>();
            var stocks = FutureAndOptionBasket.GetStockName(startDate.Year);
            foreach (var item in stocks)
            {
                var symbol = $"NSE:{item}-EQ";
                var requestHistoricalData = new HistoricalDataRequestModel()
                {
                    Symbol = symbol,
                    FromDateTime = startDate.AddDays(-5),
                    ToDateTime = endDate,
                    TimeFrame = "10"
                };


                historicalDataModels.AddRange(_historicalDataPuller.GetHistoricalData(token, requestHistoricalData));
            }
            var orderBook = new List<OrderBookModel>();

            while (startDate <= endDate)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1);
                    continue;
                }
                else if (historicalDataModels.Any(s => s.Date.Date == startDate.Date) == false)
                {
                    startDate = startDate.AddDays(1);
                    continue;
                }

                emaFinder = new List<MeanReversalFinderModel>();
                foreach (var item in stocks)
                {
                    var symbol = $"NSE:{item}-EQ";
                    var time = startDate.AddHours(9).AddMinutes(25);
                    var curentStockData = historicalDataModels
                        .Where(s => s.Symbol == symbol && s.Date >= startDate.AddBusinessDays(-3) && s.Date < time)
                        .ToList();

                    if (curentStockData.Count < 50)
                    {
                        continue;
                    }
                    // curentStockData = curentStockData.Where(s => ).ToList();
                    var ema = new ExponentialMovingAverageCalculator().CalculateEma(50, curentStockData);
                    double currentEma = ema.LastOrDefault().Value;
                    var ltp = curentStockData.LastOrDefault().Close;

                    emaFinder.Add(new MeanReversalFinderModel()
                    {
                        EMA = currentEma,
                        LTP = ltp,
                        Difference = Math.Abs(((currentEma - ltp) / currentEma) * 100),
                        Symbol = symbol
                    });
                }


                var bestStocks = emaFinder.Where(s => s.Difference > 3)
                    .OrderByDescending(s => s.Difference)
                    .Take(5)
                    .ToList();


                foreach (var item in bestStocks)
                {
                    if (item.EMA > item.LTP)
                    {
                        orderBook.Add(BuyStock(historicalDataModels, startDate, investment, item));
                    }
                    else
                    {
                        orderBook.Add(ShortStock(historicalDataModels, startDate, investment, item));
                    }
                }
                startDate = startDate.AddDays(1);
            }

            return orderBook;
        }

        private static OrderBookModel BuyStock(List<HistoricalDataModel> historicalDataModels,
            DateTime backTestStartDate,
            int investment,
            MeanReversalFinderModel item)
        {
            var order = new OrderBookModel()
            {
                BuyPrice = item.LTP,
                Quantity = (int)Math.Round(investment / item.LTP),
                EntryTime = backTestStartDate.Date.AddHours(9).AddMinutes(26),
                Symbol = item.Symbol,
                StopLoss = item.LTP - (item.LTP * 0.015),
                Target = item.LTP * 1.05
            };
            var currentDayData = historicalDataModels.Where(s => s.Symbol == item.Symbol && s.Date.Date == backTestStartDate.Date && s.Date > backTestStartDate.Date.AddHours(9).AddMinutes(20)).ToList();


            foreach (var candle in currentDayData)
            {
                if (candle.Low <= order.StopLoss)
                {
                    order.SellPrice = order.StopLoss;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
                else if (candle.High >= order.Target)
                {
                    order.SellPrice = order.Target;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
                else if (candle.Date.TimeOfDay >= candle.Date.Date.AddHours(14).AddMinutes(55).TimeOfDay)
                {
                    order.SellPrice = candle.Close;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
            }

            return order;
        }

        private static OrderBookModel ShortStock(List<HistoricalDataModel> historicalDataModels,
           DateTime backTestStartDate,
           int investment,
           MeanReversalFinderModel item)
        {
            var order = new OrderBookModel()
            {
                SellPrice = item.LTP,
                Quantity = (int)Math.Round(investment / item.LTP),
                EntryTime = backTestStartDate.Date.AddHours(9).AddMinutes(26),
                Symbol = item.Symbol,
                StopLoss = item.LTP + (item.LTP * 0.015),
                Target = item.LTP - (item.LTP * 0.05)
            };
            var currentDayData = historicalDataModels.Where(s => s.Symbol == item.Symbol && s.Date.Date == backTestStartDate.Date && s.Date > backTestStartDate.Date.AddHours(9).AddMinutes(20)).ToList();
            //var test = historicalDataModels.Where(s => s.Symbol == item.Symbol && s.Date.Date == backTestStartDate.Date).ToList();

            foreach (var candle in currentDayData)
            {
                if (candle.High >= order.StopLoss)
                {
                    order.BuyPrice = order.StopLoss;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
                else if (candle.Low <= order.Target)
                {
                    order.BuyPrice = order.Target;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
                else if (candle.Date.TimeOfDay >= candle.Date.Date.AddHours(14).AddMinutes(55).TimeOfDay)
                {
                    order.BuyPrice = candle.Close;
                    order.ExitTime = candle.Date;
                    order.PNL = order.CalculatePNL();
                    break;
                }
            }

            return order;
        }






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
