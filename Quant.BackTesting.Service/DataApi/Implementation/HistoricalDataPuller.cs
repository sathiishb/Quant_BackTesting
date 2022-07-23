using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quant.BackTesting.Model.Data;
using Quant.BackTesting.Service.DataApi.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Service.DataApi.Implementation
{
    public class HistoricalDataPuller : IHistoricalDataPuller
    {
        private readonly IConfiguration _config;
        public HistoricalDataPuller(IConfiguration config)
        {
            _config = config;
        }
        public List<HistoricalDataModel> GetHistoricalData(string token, HistoricalDataRequestModel requestHistoricalData)
        {
            var appId = _config.GetSection("Fyers:AppId").Value;
            var url = $"https://api.fyers.in/data-rest/v2/history/?" +
                $"symbol={requestHistoricalData.Symbol}" +
                $"&resolution={requestHistoricalData.TimeFrame}" +
                $"&date_format=1" +
                $"&range_from={requestHistoricalData.FormattedFromDateTime()}" +
                $"&range_to={requestHistoricalData.FormattedToDateTime()}" +
                $"&cont_flag=";

            var client = new RestClient(url)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"{appId}:{token}");
            IRestResponse response = client.Execute(request);
            var historicalData = JsonConvert.DeserializeObject<FyersHistoricalDataModel>(response.Content);
            var historicalDataModels = new List<HistoricalDataModel>();
            foreach (var item in historicalData.Candles)
            {
                DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds(int.Parse(item[0].ToString()));
                DateTime dateTime = dateTimeOffSet.ToLocalTime().DateTime;
                historicalDataModels.Add(new HistoricalDataModel()
                {
                    Symbol = requestHistoricalData.Symbol,
                    Date = dateTime,
                    Open = item[1],
                    High = item[2],
                    Low = item[3],
                    Close = item[4],
                    Volume = item[5],
                });
            }

            return historicalDataModels;
        }
        public List<HistoricalDataModel> GetHistoricalData(string token,
               DateTime startDate,
               DateTime endDate,
               string timeFrame,
               List<string> stocks)
        {

            var historicalData = new List<HistoricalDataModel>();

            foreach (var item in stocks)
            {
                var symbol = $"NSE:{item}-EQ";
                var requestHistoricalData = new HistoricalDataRequestModel()
                {
                    Symbol = symbol,
                    FromDateTime = startDate,
                    ToDateTime = endDate,
                    TimeFrame = timeFrame
                };


                historicalData.AddRange(GetHistoricalData(token, requestHistoricalData));
            }

            return historicalData;
        }

    }
}
