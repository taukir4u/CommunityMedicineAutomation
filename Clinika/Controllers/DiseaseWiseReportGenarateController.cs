using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Clinika.Models.Gateway;
using Clinika.Models.Relations;
using iTextSharp.text.factories;

namespace Clinika.Controllers
{
    public class DiseaseWiseReportGenarateController : Controller
    {
        Gateway db = new Gateway();
        //
        // GET: /DiseaseWiseReportGenarate/
        public ActionResult DiseaseWiseReport()
        {
            return View();
        }

        public ActionResult AllDisease()
        {
            var allDistrict = db.Diseaseses.Select(p => p);
            return Json(allDistrict, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiseaseStatistic(DateTime fromDate, DateTime toDate, int diseaseId)
        {

            DateTime fromDateTime = Convert.ToDateTime(fromDate);
            DateTime toDateTime = Convert.ToDateTime(toDate);

            List<DiseaseWiseReport> diseaseWiseReports = new List<DiseaseWiseReport>();
            List<int> districtsId = new List<int>();
            if (fromDateTime >= toDateTime)
            {
                var patientCounts = db.PatientList.Where(p => p.DiseasesId == diseaseId && p.DateTime <= fromDateTime && p.DateTime >= toDateTime);
                foreach (PatientCount patientCount in patientCounts)
                {
                    bool alreadyExists = districtsId.Any(x => x == patientCount.DistrictId);
                    if (alreadyExists == false)
                    {
                        districtsId.Add(patientCount.DistrictId);
                    }
                }
                foreach (int aDistrictId in districtsId)
                {
                    DiseaseWiseReport aDiseaseWiseReport = new DiseaseWiseReport();
                    var district = db.Districts.FirstOrDefault(p => p.Id == aDistrictId);
                    var totalPatient = db.PatientList.Count(p => p.DateTime <= fromDateTime && p.DateTime >= toDateTime && p.DiseasesId == diseaseId && p.DistrictId == district.Id);
                    aDiseaseWiseReport.DistrictName = district.Name;
                    aDiseaseWiseReport.TotalPatient = totalPatient;
                    decimal patientPercent = Convert.ToDecimal(totalPatient * 100) / district.Population;
                    aDiseaseWiseReport.PercentOverPopulation = patientPercent;
                    diseaseWiseReports.Add(aDiseaseWiseReport);
                }
            }
            else
            {
                var patientCounts = db.PatientList.Where(p => p.DiseasesId == diseaseId && p.DateTime >= fromDateTime && p.DateTime <= toDateTime).ToList();
                foreach (PatientCount patientCount in patientCounts)
                {
                    bool alreadyExists = districtsId.Any(x => x == patientCount.DistrictId);
                    if (alreadyExists == false)
                    {
                        districtsId.Add(patientCount.DistrictId);
                    }
                }
                foreach (int aDistrictId in districtsId)
                {
                    DiseaseWiseReport aDiseaseWiseReport = new DiseaseWiseReport();

                    var district = db.Districts.FirstOrDefault(p => p.Id == aDistrictId);
                    var totalPatient = db.PatientList.Count(p => p.DateTime >= fromDateTime && p.DateTime <= toDateTime && p.DiseasesId == diseaseId && p.DistrictId == district.Id);
                    aDiseaseWiseReport.DistrictName = district.Name;
                    aDiseaseWiseReport.TotalPatient = totalPatient;
                    decimal patientPercent = Convert.ToDecimal(totalPatient * 100) / district.Population;
                    aDiseaseWiseReport.PercentOverPopulation=patientPercent;
                    diseaseWiseReports.Add(aDiseaseWiseReport);
                }
            }
            return Json(diseaseWiseReports, JsonRequestBehavior.AllowGet);
        }


        public ActionResult StatisticResult(int diseasesId)
        {
           
            List<DiseaseWiseReport> diseaseWiseReports = new List<DiseaseWiseReport>();
            List<int> districtsId = new List<int>();


            var patientCounts = db.PatientList.Where(p => p.DiseasesId == diseasesId);
            foreach (PatientCount patientCount in patientCounts)
            {
                bool alreadyExists = districtsId.Any(x => x == patientCount.DistrictId);
                if (alreadyExists == false)
                {
                    districtsId.Add(patientCount.DistrictId);
                }
            }
            foreach (int aDistrictId in districtsId)
            {
                DiseaseWiseReport aDiseaseWiseReport = new DiseaseWiseReport();
                var district = db.Districts.FirstOrDefault(p => p.Id == aDistrictId);
                var totalPatient = db.PatientList.Count(p => p.DiseasesId == diseasesId && p.DistrictId == district.Id);
                aDiseaseWiseReport.DistrictName = district.Name;
                aDiseaseWiseReport.TotalPatient = totalPatient;
                decimal patientPercent = Convert.ToDecimal(totalPatient * 100) / district.Population;
                aDiseaseWiseReport.PercentOverPopulation = patientPercent;
                diseaseWiseReports.Add(aDiseaseWiseReport);
            }
            return Json(diseaseWiseReports, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SortAscending(DiseaseWiseReport[] originalReport)
        {
           DiseaseWiseReport aDiseaseWiseReport=new DiseaseWiseReport();
           var desc = originalReport.OrderBy(s => s.DistrictName).ToList();
            return Json(desc, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SortBypatients(DiseaseWiseReport[] originalReport)
        {
            DiseaseWiseReport aDiseaseWiseReport = new DiseaseWiseReport();
            var desc = originalReport.OrderBy(s => s.TotalPatient).ToList();
            return Json(desc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SortByparcents(DiseaseWiseReport[] originalReport)
        {
            DiseaseWiseReport aDiseaseWiseReport = new DiseaseWiseReport();
            var desc = originalReport.OrderBy(s => s.PercentOverPopulation).ToList();
            return Json(desc, JsonRequestBehavior.AllowGet);
        }
        }
}