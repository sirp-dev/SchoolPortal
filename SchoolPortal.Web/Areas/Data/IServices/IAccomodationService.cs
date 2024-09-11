using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IAccomodationService
    {
        //Hostel
        Task AddHostel(Hostel item);
        Task<Hostel> GetHostel(int? id);
        Task DeleteHostel(int? id);
        Task<List<Hostel>> HostelList();
        Task EditHostel(Hostel item);

        Task RefreshHostel();


        //Hostel Room
        Task AddRoom(HostelRoom item);
        Task<HostelRoom> GetHostelRoom(int? id);
        Task DeleteHostelRoom(int? id);
        Task<List<HostelRoom>> HostelRoomList();
        Task EditHostelRoom(HostelRoom item);

        //Hostel Bed
        Task AddHostelBed(HostelBed item);
        Task<HostelBed> GetHostelBed(int? id);
        Task DeleteHostelBed(int? id);
        Task<List<HostelBed>> HostelBedList();
        Task EditHostelBed(HostelBed item);


        //Hostel Allotment
        Task AddHostelAllotment(HostelAllotment item, int? id);
        Task<HostelAllotment> GetHostelAllotment(int? id);
        Task DeleteHostelAllotment(int? id);
        Task<List<HostelAllotment>> HostelAllotmentList();
        Task<List<HostelAllotment>> StudentHostelAllotmentList();
        Task EditHostelAllotment(HostelAllotment item);


        //Enrolled Hostel
        Task AddEnrolledHostel(EnrolledHostel item);
        Task<EnrolledHostel> GetEnrolledHostel(int? id);
        Task DeleteEnrolledHostel(int? id);
        Task<List<EnrolledHostel>> HostelEnrolledList();
        Task EditEnrolledHostel(EnrolledHostel item);


        // Enrolled Hostel Room
        Task AddEnrolledRoom(EnrolledHostelRoom item);
        Task<EnrolledHostelRoom> GetEnrolledHostelRoom(int? id);
        Task DeleteEnrolledHostelRoom(int? id);
        Task<List<EnrolledHostelRoom>> EnrolledHostelRoomList();
        Task EditEnrolledHostelRoom(EnrolledHostelRoom item);

        //Enrolled Hostel Bed
        Task AddEnrolledHostelBed(EnrolledHostelBed item);
        Task<EnrolledHostelBed> GetEnrolledHostelBed(int? id);
        Task DeleteEnrolledHostelBed(int? id);
        Task<List<EnrolledHostelBed>> EnrolledHostelBedList();
        Task EditEnrolledHostelBed(EnrolledHostelBed item);
    }
}
