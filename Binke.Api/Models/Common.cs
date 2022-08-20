using Binke.Api.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Doctyme.Model;
using System.Threading.Tasks;
using System.Configuration;

namespace Binke.Api.Models
{
    public class Common
    {
        #region ReadEmailTemplate

        public static string ReadEmailTemplate(string templateName)
        {
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath($@"\\App_Data\\EmailTemplate\\{templateName}")))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{url}", RequestHelpers.GetConfigValue("physicalLocalURL"));
                return body;
            }
            catch (Exception ex)
            {
                //LogError(ex, "ReadEmailTemplate");
                return "";
            }
        }

        public static string ReadReportTemplate(string templateName)
        {
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath($@"\\App_Data\\ReportTemplate\\{templateName}")))
                {
                    body = sr.ReadToEnd();
                }
                return body;
            }
            catch (Exception ex)
            {
               // LogError(ex, "ReadReportTemplate");
                return "";
            }
        }

        #endregion

        public async static Task<bool> ForgotPassword(ApplicationUser user, string code)
        {
            try
            {
                string companyUrl = ConfigurationManager.AppSettings["CompanySite"];
                var callbackUrl = $"{companyUrl}" + "Account/ResetPassword?userId=" + user.Id + "&code=" + code;
                var body = Models.Common.ReadEmailTemplate("ForgetPassword.Html");
                body = body.Replace("{UserName}", $@"{user.FullName}")
                    .Replace("{action_url}", callbackUrl);
                await SendMail.SendEmailAsync(user.Email, "", "", "", body, "Reset Password").ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}