using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Binke.Utility
{
    public class RequestHelpers
    {
        public static string GetClientIpAddress()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            try
            {
                var userHostAddress = request.UserHostAddress;
                if (userHostAddress == null) return "0.0.0.0";
                IPAddress.Parse(userHostAddress);

                var xForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(xForwardedFor))
                    return userHostAddress;

                var publicForwardingIps = xForwardedFor.Split(',').Where(ip => !IsPrivateIpAddress(ip)).ToList();

                return publicForwardingIps.Any() ? publicForwardingIps.Last() : userHostAddress;
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true;

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true;

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true;

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        public static string GetBrowserInfo()
        {
            HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            var s = "Browser Capabilities\n"
                       + "Type = " + browser.Type + "\n"
                       + "Name = " + browser.Browser + "\n"
                       + "Version = " + browser.Version + "\n"
                       + "Major Version = " + browser.MajorVersion + "\n"
                       + "Minor Version = " + browser.MinorVersion + "\n"
                       + "Platform = " + browser.Platform + "\n"
                       + "Is Beta = " + browser.Beta + "\n"
                       + "Is Crawler = " + browser.Crawler + "\n"
                       + "Is AOL = " + browser.AOL + "\n"
                       + "Is Win16 = " + browser.Win16 + "\n"
                       + "Is Win32 = " + browser.Win32 + "\n"
                       + "Supports Frames = " + browser.Frames + "\n"
                       + "Supports Tables = " + browser.Tables + "\n"
                       + "Supports Cookies = " + browser.Cookies + "\n"
                       + "Supports VBScript = " + browser.VBScript + "\n"
                       + "Supports JavaScript = " +
                       browser.EcmaScriptVersion + "\n"
                       + "Supports Java Applets = " + browser.JavaApplets + "\n"
                       + "Supports ActiveX Controls = " + browser.ActiveXControls
                       + "\n"
                       + "Supports JavaScript Version = " +
                       browser["JavaScriptVersion"] + "\n";
            return s;
        }

        public static string GetConfigValue(string stConfigKeyName)
        {
            var stConfigValue = System.Configuration.ConfigurationManager.AppSettings[stConfigKeyName];
            return stConfigValue;
        }
        
    }
}