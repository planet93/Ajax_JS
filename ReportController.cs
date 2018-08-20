using System.Web.Mvc;
using Phoenix.Infrastructure.ViewModel.Classifier;
using Phoenix.Web.Logic;
using System.Collections.Generic;
using System;
using Phoenix.Infrastructure.ViewModel.Report;
using System.Runtime.Caching;

namespace Phoenix.Web.Controllers {
	public class ReportController : Controller {

        private readonly MemoryCache _memoryCache = MemoryCache.Default;
        public ActionResult Index() {
			return View();
		}


        //public ActionResult ActualReport(long month, long [] years = null, long [] countries = null, long clientgroup=0, long client=0, long scenario=0, long spend3=0, long spend2=0, long spend=0,long groupcatbrand=0,long catbrand=0,long brand=0)
        //{
        [HttpPost]
        public JsonResult ActualReport(FilterReport f)
        {
            var memoryKey = Guid.NewGuid().ToString();
            _memoryCache.Add(memoryKey, f, DateTime.Now.AddMinutes(30));
            //ReportManager rm = new ReportManager();
            //List<ActualReportView> list = rm.GetActualReport();
            //ViewBag.Export = true;
            //Exporter exporter = new Exporter();
            //Exporter.ToExcel(Response, "ActualReport" + DateTime.Now.ToString("dd.MM.yyyy"));

            return Json(new { Key = memoryKey });
        }

        public ActionResult SaveActualReport(string keylist)
        {
            var res = (FilterReport)_memoryCache.Get(keylist);
            _memoryCache.Remove(keylist);
            ReportManager rm = new ReportManager();
            List<ActualReportView> list = rm.GetActualReport(res);
            ViewBag.Export = true;
            Exporter exporter = new Exporter();
            Exporter.ToExcel(Response, "ActualReport" + DateTime.Now.ToString("dd.MM.yyyy"));
            return View(list);
        }

    }
}