using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IImageService
    {
        Task<int> Create(HttpPostedFileBase upload);
        Task<ImageModel> Get(int? id);
        Task Edit(int id, HttpPostedFileBase upload);
        Task Delete(int? id);

        Task PostImageCreate(List<HttpPostedFileBase> upload, int PostId);
        Task<List<PostImage>> PostImageGet(int? PostId);
        Task PostImageDelete(int? PostId);

    }
}
