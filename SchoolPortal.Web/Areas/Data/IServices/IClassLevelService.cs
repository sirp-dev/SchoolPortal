using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IClassLevelService
    {
        Task Create(ClassLevel model);
        Task<ClassLevel> Get(int? id);

        Task Edit(ClassLevel models);
        Task Delete(int? id);
        Task<List<ClassLevelListDto>> ClassLevelList();

        Task<List<ClassLevelListDto>> AllClassLevelList();

        Task<List<ClassStudentsDto>> Students(int? id);
        Task<int> StudentsCount(int? id);

        Task<ClassLevelDetailsDto> ClassLevelDetails(int? id);

        Task GradingOption(int? id);
        Task<string> MoveStudents(int? id, int? sessionid, int classid);

        Task<List<PromotionStudentsDto>> StudentsList();
    }
}
