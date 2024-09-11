using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class AccomodationService : IAccomodationService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IFinanceService _financeService = new FinanceService();

        public AccomodationService()
        {

        }

        public AccomodationService(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager, FinanceService financeService)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            _financeService = financeService;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        ///
        public async Task AddHostel(Hostel item)
        {
            db.Hostels.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }


        }


        public async Task AddHostelBed(HostelBed item)
        {
            db.HostelBeds.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task AddRoom(HostelRoom item)
        {
            db.HostelRooms.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task DeleteHostel(int? id)
        {
            var item = await db.Hostels.FirstOrDefaultAsync(x => x.Id == id);
            db.Hostels.Remove(item);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted a hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task DeleteHostelBed(int? id)
        {
            var item = await db.HostelBeds.FirstOrDefaultAsync(x => x.Id == id);
            db.HostelBeds.Remove(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted a hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task DeleteHostelRoom(int? id)
        {
            var item = await db.HostelRooms.FirstOrDefaultAsync(x => x.Id == id);
            db.HostelRooms.Remove(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted a hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task EditHostel(Hostel item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited a hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }



        public async Task EditHostelBed(HostelBed item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited a hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task EditHostelRoom(HostelRoom item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited a hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task<Hostel> GetHostel(int? id)
        {
            var item = await db.Hostels.Include(x => x.HostelRoom).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }


        public async Task<HostelBed> GetHostelBed(int? id)
        {
            var item = await db.HostelBeds.Include(x => x.HostelRoom).Include(x => x.Hostel).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<HostelRoom> GetHostelRoom(int? id)
        {
            var item = await db.HostelRooms.Include(x => x.Hostel).Include(x => x.HostelBed).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }


        public async Task<List<HostelBed>> HostelBedList()
        {
            var item = await db.HostelBeds.Include(x => x.HostelRoom).Include(x => x.Hostel).Include(x => x.Hostel).ToListAsync();
            return item;
        }

        public async Task<List<Hostel>> HostelList()
        {
            var item = await db.Hostels.Include(x => x.HostelRoom).ToListAsync();
            return item;
        }

        public async Task<List<HostelRoom>> HostelRoomList()
        {
            var item = await db.HostelRooms.Include(x => x.Hostel).Include(x => x.HostelBed).ToListAsync();
            return item;
        }


        public async Task AddHostelAllotment(HostelAllotment item, int? id)
        {
            var fin = await _financeService.Get(id);
            var student = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == fin.UserId);
            if (student.user.Gender == "Female")
            {
                var hostel = db.EnrolledHostels.Include(x => x.EnrolledHostelRoom).Where(x => x.Status != HostelStatus.Full && x.HostelType == "Female").Take(1).ToList();
                foreach (var hos in hostel)
                {
                    var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                    //Sessions to enroll for
                    var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);

                    //for all terms in the session
                    //select session 
                    if (currentSession.Term.ToLower().Contains("first"))
                    {
                        sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
                    }
                    else if (currentSession.Term.ToLower().Contains("second"))
                    {
                        sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
                    }
                    else if (currentSession.Term.ToLower().Contains("third"))
                    {
                        sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
                    }
                    foreach (var term in sessionsToEnroll.ToList())
                    {

                        var roomi = db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).Where(x => x.Status != HostelRoomStatus.Full && x.EnrolledHostelId == hos.Id).Take(1).ToList();
                        foreach (var room in roomi)
                        {

                            //Allot Student to a bed
                            var bedi = db.EnrolledHostelBeds.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelRoom).Where(x => x.Status != HostelBedStatus.Taken && x.EnrolledHostelRoomId == room.Id).Take(1).ToList();
                            foreach (var bed in bedi)
                            { //Add student to a hostel
                                HostelAllotment hostelAllotment = new HostelAllotment();
                                hostelAllotment.AllotedDate = DateTime.UtcNow.AddHours(1);
                                hostelAllotment.HostelId = hos.Id;
                                hostelAllotment.HostelRoomId = room.Id;
                                hostelAllotment.HostelBedId = bed.Id;
                                hostelAllotment.SessionId = term.Id;
                                hostelAllotment.UserId = fin.UserId;
                                db.HostelAllotments.Add(hostelAllotment);
                                await db.SaveChangesAsync();

                                var hsRoom = db.EnrolledHostelRooms.FirstOrDefault(x => x.EnrolledHostelId == hos.Id);
                                var roomCount = db.EnrolledHostelRooms.Where(x => x.EnrolledHostelId == hos.Id && x.Status == HostelRoomStatus.Full).ToList();

                                var alloti2 = db.HostelAllotments.Include(x => x.Hostel).Include(x => x.HostelBed)
                                    .Include(x => x.HostelRoom).Include(x => x.Session)
                                    .Include(x => x.User)
                                    .Where(x => x.HostelRoomId == hsRoom.Id && x.SessionId == term.Id);

                                var alloti3 = await db.HostelAllotments.Include(x => x.Hostel)
                                    .Include(x => x.HostelBed).Include(x => x.HostelRoom)
                                    .Include(x => x.Session).Include(x => x.User)
                                    .Where(x => x.HostelRoomId == hsRoom.Id && x.SessionId == term.Id)
                                    .FirstOrDefaultAsync();

                                //Update hostel status to Full
                                if (alloti2.Count() >= hsRoom.NoOfStudent && alloti3.SessionId == term.Id)
                                {
                                    var hs = db.EnrolledHostels.FirstOrDefault(x => x.Id == hos.Id);
                                    hs.Status = HostelStatus.Full;
                                    db.Entry(hs).State = EntityState.Modified;
                                    await db.SaveChangesAsync();

                                }

                                //Update room status to Full
                                if (alloti2.Count() >= hsRoom.NoOfStudent && alloti3.SessionId == term.Id)
                                {
                                    var froom = db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).FirstOrDefault(x => x.Id == room.Id);
                                    froom.Status = HostelRoomStatus.Full;
                                    db.Entry(froom).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }

                                //Update Bed status to Taken
                                var bedd = db.EnrolledHostelBeds.FirstOrDefault(x => x.Id == bed.Id);
                                bedd.Status = HostelBedStatus.Taken;
                                db.Entry(bedd).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                                var fin2 = db.Finances.FirstOrDefault(x => x.Id == id);
                                fin2.AllocationStatus = AllocationStatus.Allocated;
                                db.Entry(fin2).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                            }
                        }
                    }

                    //Add Tracking
                    var userId = HttpContext.Current.User.Identity.GetUserId();
                    if (userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Added a hostel accomodation allotment";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }

                }


            }
            else if (student.user.Gender == "Male")
            {
                var hostel = db.EnrolledHostels.Include(x => x.EnrolledHostelRoom).Where(x => x.Status != HostelStatus.Full && x.HostelType == "Male").Take(1).ToList();
                foreach (var hos in hostel)
                {
                    var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                    //Sessions to enroll for
                    var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);

                    //for all terms in the session
                    //select session 
                    if (currentSession.Term.ToLower().Contains("first"))
                    {
                        sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
                    }
                    else if (currentSession.Term.ToLower().Contains("second"))
                    {
                        sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
                    }
                    else if (currentSession.Term.ToLower().Contains("third"))
                    {
                        sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
                    }
                    foreach (var term in sessionsToEnroll.ToList())
                    {

                        var roomi = db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).Where(x => x.Status != HostelRoomStatus.Full && x.EnrolledHostelId == hos.Id).Take(1).ToList();
                        foreach (var room in roomi)
                        {

                            //Allot Student to a bed
                            var bedi = db.EnrolledHostelBeds.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelRoom).Where(x => x.Status != HostelBedStatus.Taken && x.EnrolledHostelRoomId == room.Id).Take(1).ToList();
                            foreach (var bed in bedi)
                            { //Add student to a hostel
                                HostelAllotment hostelAllotment = new HostelAllotment();
                                hostelAllotment.AllotedDate = DateTime.UtcNow.AddHours(1);
                                hostelAllotment.HostelId = hos.Id;
                                hostelAllotment.HostelRoomId = room.Id;
                                hostelAllotment.HostelBedId = bed.Id;
                                hostelAllotment.SessionId = term.Id;
                                hostelAllotment.UserId = fin.UserId;
                                db.HostelAllotments.Add(hostelAllotment);
                                await db.SaveChangesAsync();

                                var hsRoom = db.EnrolledHostelRooms.FirstOrDefault(x => x.EnrolledHostelId == hos.Id);
                                var roomCount = db.EnrolledHostelRooms.Where(x => x.EnrolledHostelId == hos.Id && x.Status == HostelRoomStatus.Full).ToList();

                                var alloti2 = db.HostelAllotments.Include(x => x.Hostel).Include(x => x.HostelBed)
                                    .Include(x => x.HostelRoom).Include(x => x.Session).Include(x => x.User)
                                    .Where(x => x.HostelRoomId == hsRoom.Id && x.SessionId == term.Id);

                                var alloti3 = await db.HostelAllotments.Include(x => x.Hostel)
                                  .Include(x => x.HostelBed).Include(x => x.HostelRoom)
                                  .Include(x => x.Session).Include(x => x.User)
                                  .Where(x => x.HostelRoomId == hsRoom.Id && x.SessionId == term.Id)
                                  .FirstOrDefaultAsync();

                                if (alloti2.Count() >= hsRoom.NoOfStudent && alloti3.SessionId == term.Id)
                                {
                                    var hs = db.EnrolledHostels.FirstOrDefault(x => x.Id == hos.Id);
                                    hs.Status = HostelStatus.Full;
                                    db.Entry(hs).State = EntityState.Modified;
                                    await db.SaveChangesAsync();

                                }

                                //Update room status to Full
                                if (alloti2.Count() >= hsRoom.NoOfStudent && alloti3.SessionId == term.Id)
                                {
                                    var froom = db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).FirstOrDefault(x => x.Id == room.Id);
                                    froom.Status = HostelRoomStatus.Full;
                                    db.Entry(froom).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }


                                var bedd = db.EnrolledHostelBeds.FirstOrDefault(x => x.Id == bed.Id);
                                bedd.Status = HostelBedStatus.Taken;
                                db.Entry(bedd).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                                var fin2 = db.Finances.FirstOrDefault(x => x.Id == id);
                                fin2.AllocationStatus = AllocationStatus.Allocated;
                                db.Entry(fin2).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                            }
                        }
                    }

                    //Add Tracking
                    var userId = HttpContext.Current.User.Identity.GetUserId();
                    if (userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Added a hostel accomodation allotment";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }

                }
            }


        }

        public async Task<List<HostelAllotment>> HostelAllotmentList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var alot = await db.HostelAllotments.Include(x => x.Hostel)
                .Include(x => x.HostelBed).Include(x => x.HostelRoom)
                .Include(x => x.Session).Include(x => x.User).Where(x => x.SessionId == currentSession.Id).OrderByDescending(x => x.AllotedDate).ToListAsync();
            return alot;
        }

        public async Task<List<HostelAllotment>> StudentHostelAllotmentList()
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var alot = await db.HostelAllotments.Include(x => x.Hostel)
                .Include(x => x.HostelBed).Include(x => x.HostelRoom)
                .Include(x => x.Session).Include(x => x.User).Where(x => x.SessionId == currentSession.Id && x.UserId == uId).OrderByDescending(x => x.AllotedDate).ToListAsync();
            return alot;
        }
        public async Task<HostelAllotment> GetHostelAllotment(int? id)
        {
            var alot = await db.HostelAllotments.Include(x => x.Hostel)
               .Include(x => x.HostelBed).Include(x => x.HostelRoom)
               .Include(x => x.Session).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            return alot;
        }

        public async Task EditHostelAllotment(HostelAllotment item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited a hostel accomodation allotment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task DeleteHostelAllotment(int? id)
        {
            var item = await db.HostelAllotments.FirstOrDefaultAsync(x => x.Id == id);
            db.HostelAllotments.Remove(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted a hostel accomodation allotment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        //Reset Hostel,Rooms and Beds to free
        public async Task RefreshHostel()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x=>x.Status == SessionStatus.Current);
            var hostel = db.Hostels.Include(x => x.HostelRoom).Where(x => x.Status == HostelStatus.Full).ToList();
            foreach (var hos in hostel)
            {
                bool checkHos = CheckHostel(hos.Id, session.Id);
                if(checkHos == false)
                {
                    //Add Enrolled Hostel
                    EnrolledHostel enrHostel = new EnrolledHostel();
                    enrHostel.HostelId = hos.Id;
                    enrHostel.HostelType = hos.HostelType;
                    enrHostel.Name = hos.Name;
                    enrHostel.SessionId = session.Id;
                    enrHostel.Status = HostelStatus.Free;
                    db.EnrolledHostels.Add(enrHostel);
                    await db.SaveChangesAsync();


                    var roomi = db.HostelRooms.Include(x => x.Hostel).Where(x => x.Status == HostelRoomStatus.Full && x.HostelId == hos.Id).ToList();
                    foreach (var room in roomi)
                    {
                        bool checkR = CheckRoom(room.Id);
                        if(checkR == false)
                        {


                            //Add rooms EnrolledRoom
                            EnrolledHostelRoom enrRoom = new EnrolledHostelRoom();
                            enrRoom.EnrolledHostelId = enrHostel.Id;
                            enrRoom.HostelRoomId = room.Id;
                            enrRoom.Name = room.Name;
                            enrRoom.NoOfStudent = room.NoOfStudent;
                            enrRoom.RoomNo = room.RoomNo;
                            enrRoom.Status = HostelRoomStatus.Free;
                            db.EnrolledHostelRooms.Add(enrRoom);
                            await db.SaveChangesAsync();


                            var bedi = db.HostelBeds.Include(x => x.Hostel).Include(x => x.HostelRoom).Where(x => x.Status == HostelBedStatus.Taken && x.HostelRoomId == room.Id).ToList();
                            foreach (var bed in bedi)
                            {
                                bool checkBedi = CheckBed(bed.Id);
                                if(checkBedi == false)
                                {
                                    //Add rooms EnrolledBed
                                    EnrolledHostelBed enrBed = new EnrolledHostelBed();
                                    enrBed.EnrolledHostelId = enrHostel.Id;
                                    enrBed.BedNo = bed.BedNo;
                                    enrBed.EnrolledHostelRoomId = enrRoom.Id;
                                    enrBed.HostelBedId = enrHostel.Id;
                                    enrBed.Status = HostelBedStatus.Free;
                                    db.EnrolledHostelBeds.Add(enrBed);
                                    await db.SaveChangesAsync();

                                }

                            }

                        }


                    }
                   


                    //Add Tracking
                    var userId = HttpContext.Current.User.Identity.GetUserId();
                    if (userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "refresh hostel accomodation allotment to add missing hostels,rooms and beds";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }

                }

            }

        }

        public async Task AddEnrolledHostel(EnrolledHostel item)
        {
            db.EnrolledHostels.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added enrolled hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<EnrolledHostel> GetEnrolledHostel(int? id)
        {
            var item = await db.EnrolledHostels.Include(x => x.EnrolledHostelRoom).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task DeleteEnrolledHostel(int? id)
        {
            var item = await db.EnrolledHostels.FirstOrDefaultAsync(x => x.Id == id);
            db.EnrolledHostels.Remove(item);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted an enrolled hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<EnrolledHostel>> HostelEnrolledList()
        {
            var item = await db.EnrolledHostels.Include(x => x.EnrolledHostelRoom).ToListAsync();
            return item;
        }

        public async Task EditEnrolledHostel(EnrolledHostel item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited an enrolled hostel accomodation";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }


        public async Task AddEnrolledRoom(EnrolledHostelRoom item)
        {
            db.EnrolledHostelRooms.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added enrolled hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<EnrolledHostelRoom> GetEnrolledHostelRoom(int? id)
        {
            var item = await db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelBed).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task DeleteEnrolledHostelRoom(int? id)
        {
            var item = await db.EnrolledHostelRooms.FirstOrDefaultAsync(x => x.Id == id);
            db.EnrolledHostelRooms.Remove(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted an enrolled hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task<List<EnrolledHostelRoom>> EnrolledHostelRoomList()
        {
            var item = await db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelBed).ToListAsync();
            return item;
        }

        public async Task EditEnrolledHostelRoom(EnrolledHostelRoom item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited an enrolled hostel accomodation room";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task AddEnrolledHostelBed(EnrolledHostelBed item)
        {
            db.EnrolledHostelBeds.Add(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<EnrolledHostelBed> GetEnrolledHostelBed(int? id)
        {
            var item = await db.EnrolledHostelBeds.Include(x => x.EnrolledHostelRoom).Include(x => x.EnrolledHostel).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task DeleteEnrolledHostelBed(int? id)
        {
            var item = await db.EnrolledHostelBeds.FirstOrDefaultAsync(x => x.Id == id);
            db.EnrolledHostelBeds.Remove(item);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "deleted ann enrolled hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<EnrolledHostelBed>> EnrolledHostelBedList()
        {
            var item = await db.EnrolledHostelBeds.Include(x => x.EnrolledHostelRoom).Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostel).ToListAsync();
            return item;
        }

        public async Task EditEnrolledHostelBed(EnrolledHostelBed item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "edited an enrolled hostel accomodation bed";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public bool CheckHostel(int hostelId, int sessId)
        {
            var hostel = db.EnrolledHostels.Include(x => x.EnrolledHostelRoom).FirstOrDefault(x => x.HostelId == hostelId && x.SessionId == sessId);
            if (hostel != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckRoom(int roomId)
        {
            var room = db.EnrolledHostelRooms.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelBed).FirstOrDefault(x => x.HostelRoomId == roomId);
            if (room != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckBed(int bedId)
        {
            var room = db.EnrolledHostelBeds.Include(x => x.EnrolledHostel).Include(x => x.EnrolledHostelRoom).FirstOrDefault(x => x.HostelBedId == bedId);
            if (room != null)
            {
                return true;
            }
            return false;
        }
    }
}