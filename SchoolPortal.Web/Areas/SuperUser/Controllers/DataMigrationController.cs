using HsPortal.Data.Entities;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.SuperUser.Controllers
{
    public class DataMigrationController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: SuperUser/DataMigration
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> MoveClassesAndSubject()
        {
            var oldclass = await old.ClassLevels.Include(x => x.Subjects).ToListAsync();
            foreach (var i in oldclass)
            {
                var setting = await db.Settings.FirstOrDefaultAsync();

                Models.Entities.ClassLevel model = new Models.Entities.ClassLevel();
                model.ClassName = i.ClassName;
                model.UserId = "a1c045ee-873b-481a-b9f1-75e1f110473e";
                model.Passmark = setting.Passmark;
                model.PromotionByTrial = setting.PromotionByTrial;
                model.AccessmentScore = setting.AccessmentScore;
                model.ExamScore = setting.ExamScore;

                if (model.ClassName.Substring(0, 2) == "PG")
                {
                    model.ClassName = "P" + model.ClassName;
                }
                if (model.ClassName.Substring(0, 3) == "JSS" || model.ClassName.Substring(0, 3) == "SSS" || model.ClassName.Substring(0, 3) == "NUR" || model.ClassName.Substring(0, 3) == "PRI" || model.ClassName.Substring(0, 3) == "PRE" || model.ClassName.Substring(0, 3) == "PPG")
                {
                    if (model.ClassName.Substring(0, 2) == "PP")
                    {
                        model.ClassName = model.ClassName.Remove(0, 1);
                    }

                    

                    var currentSession = db.Sessions.FirstOrDefault(x => x.Status == Models.Entities.SessionStatus.Current);
                    model.SessionId = currentSession.Id;
                    db.ClassLevels.Add(model);
                    try
                    {
                    await db.SaveChangesAsync();

                    }catch(Exception c)
                    {

                    }

                    TimeTable timeTable = new TimeTable();
                    timeTable.ClassLevelId = model.Id;
                    timeTable.Monday = "Monday";
                    timeTable.Tuesday = "Tuesday";
                    timeTable.Wednessday = "Wednessday";
                    timeTable.Thursday = "Thursday";
                    timeTable.Friday = "Friday";
                    timeTable.Time10_11 = "10am - 11am";
                    timeTable.Time11_12 = "11am - 12pm";
                    timeTable.Time12_13 = "12pm - 1pm";
                    timeTable.Time13_14 = "1pm - 2pm";
                    timeTable.Time14_15 = "2pm - 3pm";
                    timeTable.Time15_16 = "3pm - 4pm";
                    timeTable.Time16_17 = "4pm - 5pm";
                    timeTable.Time17_18 = "5pm - 6pm";
                    timeTable.Time6_7 = "6am - 7am";
                    timeTable.Time7_8 = "7am - 8am";
                    timeTable.Time8_9 = "8am - 9am";
                    timeTable.Time9_10 = "9am - 10am";

                    timeTable.Name = model.ClassName;
                    db.TimeTables.Add(timeTable);
                    await db.SaveChangesAsync();

                    //subjects
                   
                    foreach (var s in model.Subjects.ToList())
                    {
                        Models.Entities.Subject smodel = new Models.Entities.Subject();
                        smodel.SubjectName = s.SubjectName;
                        smodel.ExamScore = s.ExamScore;
                        smodel.TestScore = s.TestScore;
                        smodel.RequiresPass = s.RequiresPass;
                        smodel.PassMark = s.PassMark;
                        


                        smodel.ClassLevelId = model.Id;
                        db.Subjects.Add(smodel);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                }
            }
           
            return View();
        }

    }
}