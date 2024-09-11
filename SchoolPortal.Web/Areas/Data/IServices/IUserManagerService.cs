using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IUserManagerService
    {
        Task<string> NewStaff(RegisterViewModel model);
        Task<StaffProfile> GetStaff(int? id);
       
       // Task Edit(NewStaffDto models);
        Task Delete(int? id);
        Task<ApplicationUser> GetUserByUserId(string id);
        Task<ApplicationUser> GetUserByUserEmail(string email);
        Task<string> NewStudent(RegisterViewModel model, string LastSchoolAttended, string ParentName, string ParentPhone, string ParentAddress, string ParentOccupation);

        Task<string> BatchReg(RegisterViewModel model, string LastSchoolAttended, string ParentName, string ParentPhone, string ParentAddress, string ParentOccupation);
        Task<StudentProfile> GetStudent(int? id);
        Task<string> New(RegisterViewModel model, string LastSchoolAttended);

        Task<List<StudentProfile>> ListStudent(string searchString, string currentFilter, int? page);
        Task<List<StaffProfile>> ListStaff(string searchString, string currentFilter, int? page);
        Task<List<StaffProfile>> ListNonActiveStaff(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> AllUsers(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> GraduatedUsers(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> DropoutUsers(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> Users();

        Task<bool> IsUsersinRole(string userid, string role);
        Task<bool> UpdateUser(ApplicationUser model);
        Task AddUserToRole(string userId, string rolename);
        Task RemoveUserToRole(string userId, string rolename);

        Task<List<ApplicationUser>> UserAll();
        Task<List<ApplicationUser>> Active(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> Expelled(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> Withdrawn(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> Archived(string searchString, string currentFilter, int? page);
        Task<List<ApplicationUser>> Suspeneded(string searchString, string currentFilter, int? page);




    }
}
