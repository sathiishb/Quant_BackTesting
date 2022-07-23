using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quant.BackTesting.Web.Constant;
using Quant.BackTesting.Web.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
      
        public LoginController(IConfiguration config)
        {
            _config = config;           
        }
        public IActionResult Index(string s, int code, string auth_code)
        {
            var appId = _config.GetSection("Fyers:AppId").Value;
            var hostUrl = _config.GetSection("Fyers:HostUrl").Value;
            ViewBag.Url = FyersConstant.GetUrl(appId, hostUrl);

            if (auth_code != null)
            {
                AccessToken(auth_code);
                if (ViewBag.Error == null)
                {
                    return RedirectToAction("Index", "FTG");
                }
            }

            return View();
        }

        public void AccessToken(string auth_code)
        {
            var client = new RestClient("https://api.fyers.in/api/v2/validate-authcode")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var tokenrequest = new AccessTokenRequestModel()
            {
                AppIdHash = _config.GetSection("Fyers:AppIdHash").Value,
                GrantType = "authorization_code",
                Code = auth_code
            };
            var body = JsonConvert.SerializeObject(tokenrequest);


            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var token = JsonConvert.DeserializeObject<AccessTokenResponseModel>(response.Content);

            if (token.S != "ok")
            {
                ViewBag.Error = $"Error in token Request: {token.Message}";
                return;
            }

           

            var userProfile = new UserProfileModel()
            {
                AccessToken = token.AccessToken,
                Name = "test"
            };

            HttpContext.Session.Set(SessionConstant.Profile, userProfile);


        }
    }
}
