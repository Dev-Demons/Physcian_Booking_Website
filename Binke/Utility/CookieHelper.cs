using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Binke.Utility
{
    public class CookieHelper
    {
        public static NameValueCollection GetCookieValues(string cookieName)
        {
            return HttpContext.Current.Request.Cookies[cookieName] != null ? HttpContext.Current.Request.Cookies[cookieName].Values : new NameValueCollection();
        }
        public static string GetCookieValue(string cookieName)
        {
            return HttpContext.Current.Request.Cookies[cookieName] != null ? HttpContext.Current.Request.Cookies[cookieName].Value : "";
        }
        public static void CreateUserCookie(string cookieName, string cookieValue, int? expirationDays)
        {
            DeleteCookie(cookieName);
            var cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue
            };
            if (expirationDays != null) cookie.Expires = DateTime.UtcNow.AddDays(expirationDays.Value);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void CreateCookie(string cookieName, Dictionary<string, string> keyValue, int? expirationDays)
        {
            DeleteCookie(cookieName);
            var cookie = new HttpCookie(cookieName);

            foreach (var val in keyValue)
            {
                cookie[val.Key] = val.Value;
            }

            if (expirationDays != null) cookie.Expires = DateTime.UtcNow.AddDays(expirationDays.Value);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void DeleteCookie(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null) return;
            cookie.Expires = DateTime.UtcNow.AddDays(-2);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static bool CookieExists(string cookieName)
        {
            var exists = false;
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
                exists = true;
            return exists;
        }

        public static Dictionary<string, string> GetAllCookies()
        {
            var cookies = new Dictionary<string, string>();
            foreach (var key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                var requestCookie = HttpContext.Current.Request.Cookies[key];
                if (requestCookie != null)
                    cookies.Add(key, requestCookie.Value);
            }
            return cookies;
        }

        public static void DeleteAllCookies()
        {
            var x = HttpContext.Current.Request.Cookies;
            foreach (HttpCookie cook in x)
            {
                DeleteCookie(cook.Name);
            }
        }
    }

    public class CookieKey
    {
        public static string LoggedInUserId = "NetworkLoggedInUserId";
        //public static string CookieUserName = "NetworkUserName";
        //public static string CookiePassword = "NetworkPassword";
        public static string CookieUserType = "CookieUserType";
        public static string CookieThemeType = "CookieThemeType";

        public static string RetUrl = "RetUrl";
    }
}