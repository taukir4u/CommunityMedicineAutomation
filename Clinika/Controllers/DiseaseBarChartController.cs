using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clinika.Models.Gateway;
using Clinika.Models.Relations;

namespace Clinika.Controllers
{
    public class DiseaseBarChartController : Controller
    {
        Gateway db = new Gateway();
        //
        // GET: /DiseaseBarChart/
        public ActionResult Chart()
        {
            return View();
        }

        public ActionResult DiseaseInfo(int districtId)
        {
            Report aReport = new Report();
            var patient = db.PatientList.Where(p => p.DistrictId == districtId).ToList();
            List<int> allDiseaseId = new List<int>();
            foreach (PatientCount aPatient in patient)
            {
               
                var diseaseId = allDiseaseId.Any(p => p == aPatient.DiseasesId);
                if (diseaseId==true)
                {
                    allDiseaseId.Add(aPatient.DiseasesId);
                    var disease = db.Diseaseses.FirstOrDefault(p => p.DiseasesId == aPatient.DiseasesId);
                    aReport.DieasesName.Add( disease.Name);
                    var affectedPeople =db.PatientList.Count(p => p.DiseasesId == aPatient.DiseasesId && p.DistrictId == districtId);
                    aReport.DiseaseAffected.Add(affectedPeople);
                }
            }
            return Json(aReport, JsonRequestBehavior.AllowGet);
        }



        internal class Report
        {
            public List<string> DieasesName { set; get; }
            public List<int> DiseaseAffected { set; get; }
        }

        public ActionResult AllDistrict()
        {
            var allDistrict = db.Districts.Select(p => p);
            return Json(allDistrict, JsonRequestBehavior.AllowGet);
        }
    }
}