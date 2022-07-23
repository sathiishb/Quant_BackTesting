using Microsoft.AspNetCore.Mvc;
using Quant.BackTesting.RSIIntradayMean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Controllers
{
    public class RSIIntradayMeanReversalController : Controller
    {
        private readonly IFlowClass _flowClass;
        public RSIIntradayMeanReversalController(IFlowClass flowClass)
        {
            _flowClass = flowClass;
        }
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetProfile().AccessToken;
            _flowClass.RsiMeanReversalStrategy(token, DateTime.Now.AddDays(-2), DateTime.Now);
            return View();
        }
    }
}
