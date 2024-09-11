using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IImageGalleryService
    {
        Task New(ImageGallery models, HttpPostedFileBase upload);
        Task Delete(int? id);
        Task<List<ImageGallery>> List();
        Task AddToGallery(ImageGallery models);
    }
}
