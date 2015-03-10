using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml;
using Clinika.Models.DatabaseObject;
using CliniKa.Models.DatabaseObject;
using Clinika.Models.Relations;
using Clinika.Models.Gateway;

namespace Clinika.Controllers
{
    public class TreatmentController : Controller
    {
        private Gateway db = new Gateway();
        // GET: /PatientInformation/
        public ActionResult Index()
        {
            var treatmentses = db.Treatmentses.Include(t => t.ADiseases).Include(t => t.ADose).Include(t => t.AMeal).Include(t => t.AMedicine);
            return View(treatmentses.ToList());
        }

        // GET: /PatientInformation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatmentses.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            return View(treatments);
        }

        // GET: /PatientInformation/Create
        public ActionResult Create()
        {
            ViewBag.DiseasesId = new SelectList(db.Diseaseses, "DiseasesId", "Name");
            ViewBag.DoseId = new SelectList(db.Doses, "DoseId", "Name");
            ViewBag.MealId = new SelectList(db.Meals, "MealId", "Name");
            ViewBag.MedicineId = new SelectList(db.Medicines, "MedicineId", "Name");
            return View();
        }

        // POST: /PatientInformation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TreatmentsId,VoterId,Name,Address,DateOfBirht,ServiceGiven,Observation,Date,DoctorEntryId,DiseasesId,MedicineId,DoseId,MealId,QuantityGiven,Note")] Treatments treatments)
        {
            var medicines = db.Medicines;
            List<Medicine> allMedicine = new List<Medicine>();
            foreach (Medicine medicine in medicines)
            {
                Medicine aMedicine = new Medicine();
                aMedicine.Name = medicine.Name + "-" + medicine.Power + medicine.PowerUnit;
                aMedicine.MedicineId = medicine.MedicineId;
                allMedicine.Add(aMedicine);

            }
            if (ModelState.IsValid)
            {
                db.Treatmentses.Add(treatments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DiseasesId = new SelectList(db.Diseaseses, "DiseasesId", "Name", treatments.DiseasesId);
            ViewBag.DoseId = new SelectList(db.Doses, "DoseId", "Name", treatments.DoseId);
            ViewBag.MealId = new SelectList(db.Meals, "MealId", "Name", treatments.MealId);
            ViewBag.MedicineId = new SelectList(allMedicine, "MedicineId", "Name", treatments.MedicineId);
            return View(treatments);
        }

        // GET: /PatientInformation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatmentses.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            ViewBag.DiseasesId = new SelectList(db.Diseaseses, "DiseasesId", "Name", treatments.DiseasesId);
            ViewBag.DoseId = new SelectList(db.Doses, "DoseId", "Name", treatments.DoseId);
            ViewBag.MealId = new SelectList(db.Meals, "MealId", "Name", treatments.MealId);
            ViewBag.MedicineId = new SelectList(db.Medicines, "MedicineId", "Name", treatments.MedicineId);
            return View(treatments);
        }

        // POST: /PatientInformation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TreatmentsId,VoterId,Name,Address,DateOfBirht,ServiceGiven,Observation,Date,DoctorEntryId,DiseasesId,MedicineId,DoseId,MealId,QuantityGiven,Note")] Treatments treatments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(treatments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DiseasesId = new SelectList(db.Diseaseses, "DiseasesId", "Name", treatments.DiseasesId);
            ViewBag.DoseId = new SelectList(db.Doses, "DoseId", "Name", treatments.DoseId);
            ViewBag.MealId = new SelectList(db.Meals, "MealId", "Name", treatments.MealId);
            ViewBag.MedicineId = new SelectList(db.Medicines, "MedicineId", "Name", treatments.MedicineId);
            return View(treatments);
        }

        // GET: /PatientInformation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Treatments treatments = db.Treatmentses.Find(id);
            if (treatments == null)
            {
                return HttpNotFound();
            }
            return View(treatments);
        }

        // POST: /PatientInformation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Treatments treatments = db.Treatmentses.Find(id);
            db.Treatmentses.Remove(treatments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult GetVoterInformation(string voterId)
        {
            string url = "http://nerdcastlebd.com/web_service/voterdb/index.php/voters/voter/" + voterId + "";

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(response.GetResponseStream());
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("voter");

            VoterInformation aVoter = new VoterInformation();

            foreach (XmlNode node in nodes)
            {
                aVoter.VoterId = node.SelectSingleNode("id").InnerText;
                aVoter.Name = node.SelectSingleNode("name").InnerText;
                aVoter.Address = node.SelectSingleNode("address").InnerText;
                aVoter.DateOfbirth = node.SelectSingleNode("date_of_birth").InnerText;
            }
            var servicegiven = db.TreatmentRelations.Count(p => voterId == p.VoterId);
            aVoter.Servicegiven = servicegiven;

            return Json(aVoter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDoctor(string serviceCenterCode)
        {
            var doctors = db.Doctors.Where(p => p.ServiceCenterCode == serviceCenterCode).ToList();
            return Json(doctors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TreatmentAPatientEntry(TreatmentRelation aTreatmentRelation, TreatmentMedicinegiven[] medicinegivens, PatientInformation[] patientInformations)
        {
            var voterId = "";
            foreach (PatientInformation patient in patientInformations)
            {
                var patientInfo = db.PatientInformations.FirstOrDefault(p => p.VoterId == patient.VoterId);
                if (patientInfo == null)
                {
                    PatientInformation aPatientInformation = new PatientInformation();
                    aPatientInformation.VoterId = patient.VoterId;
                    aPatientInformation.Name = patient.Name;
                    aPatientInformation.Address = patient.Address;
                    aPatientInformation.DateOfBirth = Convert.ToDateTime(patient.DateOfBirth);
                    aPatientInformation.DateOfBirth = patient.DateOfBirth;
                    db.PatientInformations.Add(patient);
                    voterId = aPatientInformation.VoterId;
                }
                else
                {
                    voterId = patient.VoterId;
                }

            }
            db.SaveChanges();
            aTreatmentRelation.DateOfObservation = Convert.ToDateTime(aTreatmentRelation.DateOfObservation);
            aTreatmentRelation.ServiceCenterCode = aTreatmentRelation.ServiceCenterCode;
            aTreatmentRelation.VoterId = voterId;
            db.TreatmentRelations.Add(aTreatmentRelation);
            db.SaveChanges();
            int diseaseId = 0;
            foreach (TreatmentMedicinegiven medicinegiven in medicinegivens)
            {
                TreatmentMedicinegiven aMedicinegiven = new TreatmentMedicinegiven();
                diseaseId = medicinegiven.DiseasesId;
                aMedicinegiven.DoseId = medicinegiven.DoseId;
                aMedicinegiven.MealId = medicinegiven.MealId;
                aMedicinegiven.DiseasesId = medicinegiven.DiseasesId;
                aMedicinegiven.MedicineId = medicinegiven.MedicineId;
                aMedicinegiven.Note = medicinegiven.Note;
                aMedicinegiven.QuantityGiven = medicinegiven.QuantityGiven;
                aMedicinegiven.VoterId = voterId;
                aMedicinegiven.SevcieCenterCode = aTreatmentRelation.ServiceCenterCode;
                db.TreatmentMedicinegivens.Add(aMedicinegiven);
                db.SaveChanges();
            }
            PatientCount aPatientCount = new PatientCount();
            var patientChk = db.PatientList.FirstOrDefault(p => p.VoterId == voterId && p.DiseasesId == diseaseId);
            if (patientChk == null)
            {
                var ServcieCenter = db.ServiceCenters.FirstOrDefault(p => p.Code == aTreatmentRelation.ServiceCenterCode);
                if (ServcieCenter != null)
                {
                    aPatientCount.DistrictId = ServcieCenter.DistrictId;
                    aPatientCount.DiseasesId = diseaseId;
                    aPatientCount.VoterId = voterId;
                    aPatientCount.DateTime = aTreatmentRelation.DateOfObservation;
                    db.PatientList.Add(aPatientCount);
                    db.SaveChanges();
                }

            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChekMedicineAvailability(string serviceCenterCode, int medicineId, int quantityGiven)
        {
            var servciceCenter = db.ServiceCenters.FirstOrDefault(p => p.Code == serviceCenterCode);
            var getAllMedicineInStock = db.AllocateMedicines.Where(p => p.ServiceCenterId == servciceCenter.Id && p.MedicineId == medicineId).ToList();
            var medicinestock = 0.0;
            var medicineGiven = 0;
            foreach (AllocateMedicine medicine in getAllMedicineInStock)
            {
                medicinestock += medicine.Quantity;
            }
            var medicineGivens =db.TreatmentMedicinegivens.Where(p => p.SevcieCenterCode == serviceCenterCode && p.MedicineId == medicineId).ToList();
           
                foreach (TreatmentMedicinegiven medicinegiven in medicineGivens)
                {
                    medicineGiven += medicinegiven.QuantityGiven;
                }

            var check = medicinestock - (medicineGiven+quantityGiven);
            if (check>=0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var quantityHas = medicinestock - (medicineGiven);
                var medicine = db.Medicines.FirstOrDefault(p => p.MedicineId == medicineId);
                var report = new { quantity = quantityHas, medicineName = medicine.Name };
                return Json(report, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult GetHistory()
        {
            string voterIdCheck = Session["voterId"].ToString();
            var history = db.TreatmentRelations.Where(p => p.VoterId == voterIdCheck);
            var serviceCenters = db.ServiceCenters.ToList();

            List<Information> informations = new List<Information>();

            foreach (TreatmentRelation relation in history)
            {

                Information aInformation = new Information();
                //string code = Convert.ToString(relation.ServiceCenterCode);
                var servciecenter = serviceCenters.FirstOrDefault(p => p.Code == relation.ServiceCenterCode);

                aInformation.ServcieCenterCode = servciecenter.Code;
                aInformation.ServiceCenterName = servciecenter.Name;

                aInformation.Dates = relation.DateOfObservation;
                informations.Add(aInformation);
            }
            return Json(informations, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Show()
        {
            return View();
        }

        public ActionResult ShowHistory(string voterId)
        {
            Session["voterId"] = voterId;
           return View("Show");
        }

        internal class Information
        {
            public DateTime Dates { set; get; }
            public string ServiceCenterName { set; get; }
            public string ServcieCenterCode { set; get; }
        }

        internal class VoterInformation
        {
            public string VoterId { set; get; }
            public string Name { set; get; }
            public string Address { set; get; }
            public string DateOfbirth { set; get; }
            public int Servicegiven { set; get; }
        }
    }
}
