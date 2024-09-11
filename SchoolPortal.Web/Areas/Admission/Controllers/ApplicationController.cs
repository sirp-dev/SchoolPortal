using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models.Dtos;

namespace SchoolPortal.Web.Areas.Admission.Controllers
{

    public class ApplicationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IRegistrationDataService _registerServices = new RegistrationDataService();
        private IImageService _imageServices = new ImageService();
        private ISettingService _settingService = new SettingService();

        public ApplicationController()
        { }
        public ApplicationController(RegistrationDataService registerServices, ImageService imageService, SettingService settingService)
        {
            _registerServices = registerServices;
            _imageServices = imageService;
            _settingService = settingService;
        }

        // GET: Admission/Admission
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.StudentDatas.ToListAsync());
        //}

        // GET: Registration
        public ActionResult Apply()
        {
            var sett = db.Settings.FirstOrDefault();
            if (sett.AdmissionPinOption == AdmissionPinOption.UsedPin)
            {
                return RedirectToAction("Index", "Application", new { area = "Admission" });
            }
            else if (sett.AdmissionPinOption == AdmissionPinOption.NoPin)
            {
                return RedirectToAction("Index2", "Application", new { area = "Admission" });


            }
            return View();
        }


        public ActionResult Index()

        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(AdmissionPinDto model)
        {
            if (ModelState.IsValid)
            {
                //var sett = db.Settings.FirstOrDefault();
                //if(sett.AdmissionPinOption == AdmissionPinOption.UsedPin)
                //{
                try
                {
                    //var checkPin = await _registerServices.GetPin();

                    var checkPin = db.PinCodeModels.FirstOrDefault(pin => pin.PinNumber == model.PinNumber && pin.SerialNumber == model.SerialNumber);
                    var formData1 = db.StudentDatas.FirstOrDefault(pin => pin.RegistrationNumber == checkPin.StudentPin);
                    if (checkPin == null)
                    {
                        TempData["Error"] = "The PIN Number does not exist.Please check the numbers you entered and try again.";
                        return RedirectToAction("Index");
                    }
                    else if (formData1 != null)
                    {

                        return RedirectToAction("Printout_Form", new { id = formData1.Id });
                    }
                    else
                    {
                        //check if it has been used
                        if (checkPin.StudentPin == null) //it has not been used
                        {
                            TempData["SerialId"] = model.PinNumber;
                            //Redirect to the form to fill it

                            return RedirectToAction("Register", "Application", new { refid = model.PinNumber });
                        }
                        else
                        {
                            //check if the form has been completed before
                            var formData = db.StudentDatas.FirstOrDefault(x => x.RegistrationNumber == checkPin.StudentPin);
                            //var formData = await _registerServices.FormData();

                            if (formData != null)
                            {
                                //Redirect to form again to re-fill it
                                return RedirectToAction("Printout_Form", new { id = formData.Id });
                            }
                            else
                            {
                                TempData["Error"] = "There is an error. Please try again or contact the Administrator.";
                                return RedirectToAction("Index");
                            }

                        }
                    }
                }
                catch
                {
                    TempData["Error"] = "There is an error. Please try again or contact the Administrator.";
                    return RedirectToAction("Index");
                }

                //}
                //else if (sett.AdmissionPinOption == AdmissionPinOption.NoPin)
                //{
                //    return RedirectToAction("Index2");


                //}

            }
            return View(model);
        }


        public ActionResult Index2()

        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Index2(AdmissionPinDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var formData = await _registerServices.FormEmail();
                    var formData = db.StudentDatas.FirstOrDefault(x => x.EmailAddress == model.EmailAddress);
                    if (model.EmailAddress == null)
                    {
                        return RedirectToAction("Index2");
                    }

                    if (formData.EmailAddress == null)
                    {
                        return RedirectToAction("FillForm");
                    }

                    if (formData.EmailAddress != null)
                    {
                        //Redirect to form again to re-fill it
                        return RedirectToAction("Printout_Form", new { id = formData.Id });
                    }
                    else
                    {
                        TempData["Error"] = "There is an error. Please try again or contact the Administrator.";
                        return RedirectToAction("Index2");
                    }

                }

                catch
                {
                    TempData["Error"] = "There is an error. Please try again or contact the Administrator.";
                    return RedirectToAction("Index2");
                }

            }
            return View(model);
        }

        //[HttpPost]
        //public async Task<ActionResult> Index2(AdmissionPinDto model)
        //{
        //    //var formData = db.StudentDatas.FirstOrDefault(x => x.EmailAddress == model.EmailAddress);

        //    var formData = await _registerServices.FormEmail();
        //    if (formData != null)
        //    {
        //        //Redirect to form again to re-fill it
        //        return RedirectToAction("AcknowledgmentPage2", new { id = formData.EmailAddress });
        //    }
        //    else
        //    {
        //        return RedirectToAction("FillForm");
        //    }
        //}



        // GET: Admission/StudentDatas/Create
        public ActionResult Register(string refid)
        {
            if (refid == null)
            {
                return RedirectToAction("Index");
            }

            var pincode = db.PinCodeModels.FirstOrDefault(x => x.PinNumber == refid);
            var studentId = db.StudentDatas.FirstOrDefault(x=>x.RegistrationNumber == pincode.StudentPin);

            if (pincode != null && studentId != null)
            {
                return RedirectToAction("Printout_Form", new { id = studentId.Id });
            }

            ViewBag.Serial = refid;

            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");

            return View();
        }

        // POST: Admission/StudentDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(StudentData studentData, HttpPostedFileBase upload, string refid)
        {
            if (ModelState.IsValid)
            {

                //var pincode = db.PinCodeModels.FirstOrDefault(x => x.PinNumber == refid);

                //if (pincode == null)
                //{
                //    return RedirectToAction("Index");
                //}



                await _registerServices.Create(studentData,refid);
                var Imageid = await _imageServices.Create(upload);
                studentData.ImageId = Imageid;
                await _registerServices.Edit(studentData);
                return RedirectToAction("Printout_Form", new { id = studentData.Id });
            }

            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", studentData.StateOfOrigin);
            TempData["Error"] = "Registration not successful try again or contact the Administrator.";
            return View(studentData);
        }



        // GET: Admission/StudentDatas/Create
        public ActionResult FillForm(string refid)
        {
            if (refid == null)
            {
                return RedirectToAction("Index2");
            }

            var studentId = db.StudentDatas.FirstOrDefault(x => x.EmailAddress == refid);

            if (studentId != null)
            {
                return RedirectToAction("Printout_Form", new { id = studentId.Id });
            }

            ViewBag.Email = refid;

            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");

            return View();
        }

        // POST: Admission/StudentDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> FillForm(StudentData studentData, HttpPostedFileBase upload, string refid)
        {
            if (ModelState.IsValid)
            {

                //var formData = db.StudentDatas.FirstOrDefault(x => x.EmailAddress == studentData.EmailAddress);
                //if (formData.EmailAddress != null)
                //{
                //    TempData["Error"] = "The email address has been used try another email or contact the Administrator.";
                //    return RedirectToAction("FillForm");
                //}
                await _registerServices.Create(studentData,refid);
                var Imageid = await _imageServices.Create(upload);
                studentData.ImageId = Imageid;
                await _registerServices.Edit(studentData);
                return RedirectToAction("Printout_Form", new { id = studentData.Id });

            }

            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", studentData.StateOfOrigin);

            return View(studentData);
        }


        // GET: Admission/StudentDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.GetEdit(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }

        // POST: Admission/StudentDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StudentData studentData, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                // studentData.DateOfBirth = DateTime.ParseExact(studentData.DateOfBirth, "dd/MM/yyyy", null);
                if (upload != null && upload.ContentLength > 0)
                {
                    var image = await _imageServices.Get(studentData.ImageId);
                    if (image.ImageContent != null)
                    {
                        await _imageServices.Delete(studentData.ImageId);
                    }
                    var newImageId = await _imageServices.Create(upload);
                    studentData.ImageId = newImageId;
                }
                await _registerServices.Edit(studentData);

                return RedirectToAction("Printout_Form", new { id = studentData.Id });
            }

            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", studentData.StateOfOrigin);
            return View(studentData);
        }





        //public ActionResult Register(string id)
        //{

        //    if (id == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Serial = id;

        //    ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");


        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Register(StudentData model, string id)
        //{
        //    if (ModelState.IsValid)
        //    {


        //        var pincode = db.PinCodeModels.FirstOrDefault(x => x.PinNumber == id);

        //        if (pincode == null)
        //        {
        //            return RedirectToAction("Index");
        //        }




        //        db.StudentDatas.Add(model);
        //    test:
        //        var number = db.StudentDatas.Count() + 1;
        //        var studentNumber = "000" + number.ToString();
        //        var registrationNumber = "HRSSU/015/000" + number;
        //        var checkNumber = db.StudentDatas.FirstOrDefault(x => x.RegistrationNumber == registrationNumber);
        //        if (checkNumber == null)
        //        {
        //            pincode.StudentPin = registrationNumber;
        //            model.RegistrationNumber = registrationNumber;
        //            model.ApplicationDate = DateTime.Now;
        //            model.ExamScore = 0;
        //        }
        //        else
        //        {
        //            goto test;
        //        }


        //        db.SaveChanges();
        //        return RedirectToAction("AcknowledgmentPage", new { id = model.Id });
        //    }

        //    //List<SelectListItem> sex = new List<SelectListItem>();
        //    //sex.Add(new SelectListItem { Text = "Male", Value = "Male" });
        //    //sex.Add(new SelectListItem { Text = "Female", Value = "Female" });
        //    //ViewBag.Gender = new SelectList(sex, "Value", "Text", supplementarydata.Gender);


        //    ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");



        //    return View(model);

        //}



        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs.Include(x => x.States)
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Printout(int? id)
        {
            var sett = db.Settings.FirstOrDefault();
            ViewBag.Name = sett.SchoolName;
            ViewBag.Slogan = sett.SchoolAddress;
            ViewBag.Exam = sett;

            var img = await _settingService.GetSetting();
            ViewBag.Logo = img.Image;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.Get(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);

        }


        [HttpGet]
        public async Task<ActionResult> Printout_Form(int? id)
        {

            var sett = db.Settings.FirstOrDefault();
            ViewBag.Name = sett.SchoolName;
            ViewBag.Slogan = sett.SchoolAddress;
            ViewBag.ExamDate = sett.ScreeningDate;
            ViewBag.Exam = sett;

            //SettingLayoutDto schimage = new SettingLayoutDto();
            //ViewBag.Logo = schimage;

            var img = await _settingService.GetSetting();
            ViewBag.Logo = img.Image;



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.Get(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
