using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Models
{

    public class AccessTokenRequestModel
    {

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("appIdHash")]
        public string AppIdHash { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

}
