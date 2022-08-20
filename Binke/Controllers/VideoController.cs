using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Binke.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        public ActionResult VideoPage(int? DoctorId,int? PatientId)
        {
            ViewBag.Channel = DoctorId.ToString()+"."+PatientId.ToString() ;
            return View();
        }
    }
}
