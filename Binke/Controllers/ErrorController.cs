using System.Web.Mvc;

namespace Binke.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            return View();
        }
        public ActionResult ForBidden()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult NotImplemented()
        {
            return View();
        }

        public ActionResult ServiceTemporarilyOverloaded()
        {
            return View();
        }

        public ActionResult ServiceUnavailable()
        {
            return View();
        }

        public ActionResult Custom()
        {
            return View();
        }
    }
}
