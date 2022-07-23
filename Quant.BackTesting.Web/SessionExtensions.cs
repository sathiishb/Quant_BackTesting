using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Quant.BackTesting.Web.Constant;
using Quant.BackTesting.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quant.BackTesting.Web
{
    public static class SessionExtensions
    {
        public static T Get<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void Set(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static UserProfileModel GetProfile(this ISession session)
        {
            return session.Get<UserProfileModel>(SessionConstant.Profile);
        }
    }
}
