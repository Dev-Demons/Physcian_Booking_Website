using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Api.Utility
{
    public class RequestHelpers
    {
        public static string GetConfigValue(string stConfigKeyName)
        {
            var stConfigValue = System.Configuration.ConfigurationManager.AppSettings[stConfigKeyName];
            return stConfigValue;
        }
    }
}