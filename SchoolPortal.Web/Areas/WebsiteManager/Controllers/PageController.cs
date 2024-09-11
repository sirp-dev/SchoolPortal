using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using SchoolPortal.Web.Models.Entities;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using SchoolPortal.Web.Models.Dtos;

namespace SchoolPortal.Web.Areas.WebsiteManager.Controllers
{
    public class PageController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            //popup modal to show information

            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 3055).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.modal = style;

            }
            catch (Exception c) { }

            try
            {

                var query = db.Posts.Include(x => x.PostImages).ToList();
                //
                var blog = query.Select(c => new PostByteDto()
                {
                    Id = c.Id,
                    Title = c.Title,
                    Content = c.Content,
                    PreviewContent = c.PreviewContent,
                    DatePosted = c.DatePosted,
                    Status = c.Status,
                    WhoCanSeePost = c.WhoCanSeePost,
                    PostedBy = c.PostedBy,
                    SortOrder = c.SortOrder,
                    PostImagess = c.PostImages.OrderByDescending(x => x.Id).FirstOrDefault().ImageContent
                }).ToList();


                //
                var check = db.WebsiteSettings.FirstOrDefault();
                if (check.ShowBlogInHome == true)
                {
                    blog = blog.Where(x => x.Status == PostStatus.Published).OrderByDescending(x => x.DatePosted).ThenByDescending(x => x.SortOrder).Take(3).ToList();

                    if (blog.Count() < 3)
                    {
                        int remainingcount = 3 - blog.Count();

                        string endurl = "http://iskools.com/api/ApiUrl?size=" + remainingcount;

                        string apiUrl = String.Format(endurl);

                        WebRequest requestObj = WebRequest.Create(apiUrl);
                        requestObj.Method = "GET";

                        HttpWebResponse responseGet = null;
                        responseGet = (HttpWebResponse)requestObj.GetResponse();
                        string result = null;
                        List<PostDto> post = new List<PostDto>();
                        using (Stream stream = responseGet.GetResponseStream())
                        {
                            StreamReader sr = new StreamReader(stream);
                            result = sr.ReadToEnd();
                            post = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<PostDto>>(result));

                            sr.Close();
                        }

                        var secondlistofpost = post.ToList();
                        ViewBag.secondpost = secondlistofpost;
                    }
                    else
                    {

                    }
                    ViewBag.blog = blog;
                }


            }
            catch (Exception f)
            {

            }
            return View();
        }

        //content page: pages from category session
        public async Task<ActionResult> ContentPage(int id, string title)
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            //content page
            var content = await db.CategoryPages.Include(x => x.ContentPages).FirstOrDefaultAsync(x => x.Publish == Models.Entities.PagePublish.Publish && x.Id == id);
            return View(content);
        }
        //item page: pages from content page
        public async Task<ActionResult> itemPage(int id, string title)
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }
            var content = await db.ContentPages.Include(x => x.CategoryPage).FirstOrDefaultAsync(x => x.Publish == Models.Entities.PagePublish.Publish);
            return View(content);
        }

        //page for hall of fame
        public async Task<ActionResult> PageFame(int id, string title)
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            //content page
            var content = await db.HallOfFames.FirstOrDefaultAsync(x => x.Id == id);
            return View(content);
        }
        //menu with html tags
        public ActionResult _MenuStrinIitem()
        {
            //slider css
            try
            {

                var css = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 1020).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.csssmenu = css.ContentHome;
            }
            catch (Exception c) { }

            var menu = db.CategoryPages.Include(x => x.ContentPages).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.Dropdown
              || x.MenuDescription == Models.Entities.MenuDescription.MainMenu && x.SortOrder == 1000001);
            return PartialView(menu);
        }

        //top header
        public ActionResult _MenuTopHeader()
        {
            var menu = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.MenuTopHeader);
            return PartialView(menu);
        }

        //body short pages
        public ActionResult _HomeBody()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).Where(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HomeBody).ToList();
            return PartialView(item);
        }

        //content above footer but in layout
        public ActionResult _AboveFooter()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).Where(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.AboveFooter).ToList();
            return PartialView(item);
        }

        //footer items
        public ActionResult _Footer()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.Footer);
            return PartialView(item);
        }

        //hall of fame
        public ActionResult _HallOfFame()
        {

            var check = db.WebsiteSettings.FirstOrDefault();
            if (check.ShowHallOfFameInHome == true)
            {
                var item = db.HallOfFames.OrderByDescending(x => x.SortOrder).ToList();
                ViewBag.hall = item;
            }
            else
            {

            }

            return PartialView();
        }

        public ActionResult HallOfFame()
        {
            //header tag
            try
            {
                var itemm = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = itemm.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            //popup modal to show information

            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 3055).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.modal = style;

            }
            catch (Exception nv) { }
            var item = db.HallOfFames.OrderByDescending(x => x.SortOrder).ToList();
            return View(item);
        }

        public ActionResult _PageCrumb()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 9001).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.DetailPageCrumb);
            return PartialView(item);
        }
        //blog
        public ActionResult _BlogCrumb()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 9101).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.DetailPageCrumb);
            return PartialView(item);
        }
        //blog
        public ActionResult _BlogDetailCrumb()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 9111).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.DetailPageCrumb);
            return PartialView(item);
        }

        //blog in home

        // GET: Posts
        public ActionResult _BlogHome()
        {
            //school 



            return PartialView();
        }

        //fame
        public ActionResult _FameCrumb()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 2399).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.DetailPageCrumb);
            return PartialView(item);
        }
        //fame
        public ActionResult _FameDetailCrumb()
        {
            var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 2499).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.DetailPageCrumb);
            return PartialView(item);
        }
        public ActionResult _Slider()
        {
            //slider js
            try
            {
                var js = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 10002).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jsslider = js.ContentHome;
            }
            catch (Exception c) { }
            //slider css
            try
            {

                var css = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 10001).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.cssslider = css.ContentHome;
            }
            catch (Exception c) { }
            //slider images list
            try
            {

                var slider = db.ImageSlider.Where(x => x.CurrentSlider == true).ToList();
                ViewBag.slider = slider;
            }
            catch (Exception c) { }
            return PartialView();
        }

        public ActionResult PageLinks()
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            var settings = db.Settings.FirstOrDefault().WebsiteLink;
            ViewBag.url = settings;
            var pages = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.None).ToList();
            return View(pages);
        }

        public ActionResult Help()
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            var settings = db.Settings.FirstOrDefault().WebsiteLink;
            ViewBag.url = settings;
            var pages = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.Publish == Models.Entities.PagePublish.Publish).ToList();
            return View(pages);
        }

        #region preview

        //content page: pages from category session
        public async Task<ActionResult> ContentView(int id, string title)
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            //content page
            var content = await db.CategoryPages.Include(x => x.ContentPages).FirstOrDefaultAsync(x => x.Publish == Models.Entities.PagePublish.Publish && x.Id == id);
            return View(content);
        }
        //item page: pages from content page
        public async Task<ActionResult> itemView(int id, string title)
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }
            var content = await db.ContentPages.Include(x => x.CategoryPage).FirstOrDefaultAsync(x => x.Publish == Models.Entities.PagePublish.Publish);
            return View(content);
        }


        #endregion
    }
}