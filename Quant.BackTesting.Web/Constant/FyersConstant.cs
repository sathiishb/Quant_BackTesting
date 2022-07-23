using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Constant
{
    public class FyersConstant
    {
        public const string Status = "ok";
        public static string GetUrl(string appId, string hostUrl)
        {
            //http://192.168.1.4
            var url = $@"https://api.fyers.in/api/v2/generate-authcode?client_id={appId}&redirect_uri={hostUrl}Login&response_type=code&state=sample_state";
            return url;
        }
    }
}
