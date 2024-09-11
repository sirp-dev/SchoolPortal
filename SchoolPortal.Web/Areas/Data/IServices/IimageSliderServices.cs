using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IimageSliderServices
    {
        Task New(ImageSlider models, HttpPostedFileBase upload);
        Task Delete(int? id);
        Task<List<ImageSlider>> List();
        Task AddToSlider(ImageSlider models);
    }
}
