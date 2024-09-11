using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class PinService : IPinService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

      

        public async Task<PinInfoDto> Details(int? id)
        {
            var item = await db.PinCodeModels.FirstOrDefaultAsync(x => x.Id == id);
            if(item.EnrollmentId == null && item.StudentPin != null)
            {
                var student = await db.StudentProfiles.Include(j => j.user).FirstOrDefaultAsync(o => o.StudentRegNumber == item.StudentPin);
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = "NONE",
                    StudentFullName = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                    //StudentFullName = "NONE",
                    StudentPin = item.StudentPin,
                  
                };
                return output;
            }
            else if (item.StudentPin != null)
            {
                var enrolment = await db.Enrollments.Include(c => c.Session).Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == item.EnrollmentId);
                var session = await db.Sessions.FirstOrDefaultAsync(c => c.Id == enrolment.SessionId);
                var student = await db.StudentProfiles.Include(j => j.user).FirstOrDefaultAsync(o => o.StudentRegNumber == item.StudentPin && o.Id == enrolment.StudentProfileId);
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = session.SessionYear + " " + session.Term,
                    StudentFullName = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                    StudentPin = item.StudentPin,
                    EnrollmentId = enrolment.Id
                };
                return output;
            }
            else
            {
                var output = new PinInfoDto
                {
                    PinNumber = item.PinNumber,
                    SerialNumber = item.SerialNumber,
                    BatchNumber = item.BatchNumber,
                    Usage = item.Usage,
                    SessionUsed = "NONE",
                    StudentFullName = "NONE",
                    StudentPin = "NONE"
                };
                return output;
            }



        }

        public async Task<List<PinCodeModel>> List(string searchString, string currentFilter, int? page)
        {
            var pins = from pin in db.PinCodeModels.Include(x => x.Session).OrderByDescending(x => x.PinNumber)
                       select pin;

            if (!String.IsNullOrEmpty(searchString))
            {
                pins = pins.Where(p => p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.PinNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.SerialNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.BatchNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.SessionId.ToString().ToUpper().Contains(searchString.ToUpper())
                    );
            }
            return await pins.ToListAsync();
        }

        public async Task<int> TotalPin()
        {
            var pin = await db.PinCodeModels.CountAsync();
            return pin;
        }

        public async Task<int> TotalUnUsedPin()
        {
            var pin = await db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin == null).CountAsync();
            return pin;
        }

        public async Task<int> TotalUsedPin()
        {
            var pin = await db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin != null).CountAsync();
            return pin;
        }

        public async Task<List<PinCodeModel>> UnUsedPin(string searchString, string currentFilter, int? page)
        {
            var pins = from pin in db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin == null).OrderByDescending(x => x.PinNumber)
                       select pin;

            if (!String.IsNullOrEmpty(searchString))
            {
                pins = pins.Where(p => p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.PinNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.SerialNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.BatchNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.SessionId.ToString().ToUpper().Contains(searchString.ToUpper())
                    );
            }
            return await pins.ToListAsync();
        }

        public async Task<List<PinCodeModel>> UsedPin(string searchString, string searchStringSession, string currentFilter, int? page)
        {
            var pins = from pin in db.PinCodeModels.Include(x => x.Session).Where(x => x.StudentPin != null).OrderByDescending(x => x.PinNumber)
                       select pin;

            if (!String.IsNullOrEmpty(searchString) || !String.IsNullOrEmpty(searchStringSession))
            {
                pins = pins.Where(p => p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.PinNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.SerialNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.BatchNumber.ToUpper().Contains(searchString.ToUpper())
                    || p.StudentPin.ToUpper().Contains(searchString.ToUpper())
                    || p.SessionId.ToString().ToUpper().Contains(searchStringSession.ToUpper())
                    );
            }
            return await pins.ToListAsync();
        }
    }
}