using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IGradingService
    {
        Task Create(Grading model);
        Task<Grading> Get(int? id);
        Task Delete(int? id);
        Task<List<Grading>> List();
        Task Edit(Grading models);


        ////
        Task Add(GradingDetails model);
        Task<GradingDetails> GetGrade(int? id);
        Task DeleteGrade(int? id);
        Task EditGrade(GradingDetails models);


    }
}
