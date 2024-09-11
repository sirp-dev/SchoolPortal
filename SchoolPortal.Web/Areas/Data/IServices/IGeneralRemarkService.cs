using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IGeneralRemarkService
    {
        Task Create(NewsLetter model);
        Task<NewsLetter> Get(int? id);

        Task Edit(NewsLetter models);
        Task Delete(int? id);
        Task<List<NewsLetter>> List();
    }
}
