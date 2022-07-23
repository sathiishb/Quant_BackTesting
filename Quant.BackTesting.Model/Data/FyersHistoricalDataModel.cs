using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Model.Data
{
    public class FyersHistoricalDataModel
    {
        [JsonProperty("s")]
        public string S { get; set; }

        [JsonProperty("candles")]
        public List<List<double>> Candles { get; set; }
    }
}
