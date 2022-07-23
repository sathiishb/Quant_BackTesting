using Quant.BackTesting.Model.Strategy;
using System;
using System.Collections.Generic;

namespace Quant.BackTesting.TrendFollowingSTBT
{
    public interface IFlowClass
    {
        List<ReportModel> CalculatePoints(string token,DateTime dateTime,DateTime endDate );
        List<ReportModel> CalculatePoints(string token);
    }
}