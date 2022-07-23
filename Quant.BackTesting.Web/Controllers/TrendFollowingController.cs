using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Quant.BackTesting.Model.Strategy;
using Quant.BackTesting.Service.Epplus;
using Quant.BackTesting.TrendFollowingSTBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Controllers
{
    public class TrendFollowingController : Controller
    {

        private readonly IConfiguration _config;
        private readonly IFlowClass _flowClass;
        public TrendFollowingController(
            IConfiguration config,
            IFlowClass flowClass)
        {

            _config = config;
            _flowClass = flowClass;
        }
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetProfile().AccessToken;
            //var pnls = _flowClass.CalculatePoints(token);

            var pnlCollection = new List<ReportModel>();
            var fromDate = new DateTime(2018, 01, 01);
            var toDate = new DateTime(2022, 03, 29);
            var fileName = $"BankNifty__HighandLow_From-{fromDate:MM_dd_yyyy} to {toDate:MM_dd_yyyy}";
            var slice = 90;

            while (fromDate < toDate)
            {

                var slicedToDate = fromDate.AddDays(slice);
                slicedToDate = slicedToDate < toDate ? slicedToDate : toDate;
                var pnls = _flowClass.CalculatePoints(token, fromDate, slicedToDate);
                pnlCollection.AddRange(pnls);
                fromDate = slicedToDate.AddDays(1);
            }
            ExcelConvertor.ExportToExcel(pnlCollection, @"C:\Satz\Market-Data\BackTesting\TrendFollowing", fileName);

            return View();
        }
    }
}
