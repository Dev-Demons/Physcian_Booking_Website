using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doctyme.Model;
using Binke.Models;
using Doctyme.Repository;
using Doctyme.Repository.Services;
namespace Binke.Controllers
{
    public class GetDrugController : Controller
    {
        // GET: GetDrug
        public ActionResult Index()
        {
            return View();
        }

        // GET: GetDrug/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GetDrug/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GetDrug/Create
        [HttpPost,Route("SearchDrugs/GetDrugsdt")]
        public JsonResult Searchdrugs(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                List<GetDrugsModal> dr;
                DoctymeDbContext dbcon = new DoctymeDbContext();
                GetDrugRepository r = new GetDrugRepository(dbcon);
               dr=  r.getdrugs(Request.Form["MedicineName"].ToString()).ToList();
                   return Json(new JsonResponse() { Status = 200, Message = "1", Data = dr }, JsonRequestBehavior.AllowGet); ;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        // GET: GetDrug/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GetDrug/Edit/5
        [HttpPost,Route("SearchDrugs/Getpharmadt")]
        public JsonResult searchpharma( FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                List<getpharmamodel> dr;
                DoctymeDbContext dbcon = new DoctymeDbContext();
                GetDrugRepository r = new GetDrugRepository(dbcon);
                dr = r.getpharma("all").ToList();
                return Json(new JsonResponse() { Status = 200, Message = "1", Data = dr }, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: GetDrug/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GetDrug/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
