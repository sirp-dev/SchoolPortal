using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface ISyllableService
    {
        Task Create(Syllable model);
        Task<Syllable> Get(int? id);

        Task Edit(Syllable models);
        Task Delete(int? id);
        Task<List<Syllable>> ListAll();
        Task<List<Syllable>> ListByType(string title);
    }
}
