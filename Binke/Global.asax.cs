using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Binke.Utility;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Unity;
using Unity.AspNet.Mvc;

namespace Binke
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastException                = Server.GetLastError();

            IErrorLogService errorLogService = UnityConfig.Container.Resolve<IErrorLogService>();
            var errorModel                   = lastException.ParseException("Global.asax");
            errorLogService.InsertData(errorModel);
            errorLogService.SaveData();

            // Temprorly commented : 12 Aug 2021 - Ramesh Palanivel : Avoid unneccesary email on support@doctyme.com
            //string supportEmail              = ConfigurationManager.AppSettings["supportemail"];
            //StringBuilder strErrorBuilder     = new StringBuilder();
            //strErrorBuilder.Append($"<p><h2>REQUEST</h2>{Request.ToRaw()}</p><hr />");
            //strErrorBuilder.Append($"<p><h2>ERROR</h2>{lastException.ToString()}</p><hr />");


            //SendMail.SendEmail(supportEmail, string.Empty, string.Empty, string.Empty, strErrorBuilder.ToString(), $"ERROR REPORT : {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm:ss")}");

            if (Response.StatusCode != 404)
            {
                Common.LogError(Context.Server.GetLastError(), "Web");
            }
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}
