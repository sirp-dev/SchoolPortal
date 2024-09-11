using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System.IO;
using SchoolPortal.Web.Models.Dtos;
using Newtonsoft.Json;

namespace SchoolPortal.Web.Areas.WebsiteManager.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public async Task<ActionResult> Latest()
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


            //blog style

            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 2005).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styleblog = style.ContentHome;

            }
            catch (Exception c) { }

            //blog js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 2055).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jsblog = style.ContentHome;

            }
            catch (Exception c) { }

           
                List<PostByteDto> blog = new List<PostByteDto>();
                try
                {
                    var query = db.Posts.Include(x => x.PostImages).ToList();
                    //
                    blog = query.Select(c => new PostByteDto()
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

                }catch(Exception c) { }
                //
                var check = db.WebsiteSettings.FirstOrDefault();
                if (check.ShowBlogInHome == true)
                {
                    blog = blog.Where(x => x.Status == PostStatus.Published).OrderByDescending(x => x.DatePosted).ThenByDescending(x => x.SortOrder).Take(3).ToList();

                    try
                    {
                        int remainingcount = 1000;

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
                    }catch(Exception c) { }
                    ViewBag.blog = blog;
                }


            
           
            return View();
        }

        // GET: Posts/Details/5
        public async Task<ActionResult> Read(int? id, string source, string title)
        {
            ViewBag.source = source;
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


            //blog detail style

            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 2555).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styledetailsblog = style.ContentHome;

            }
            catch (Exception c) { }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (source == "xyz")
            {
                var post1 = await db.Posts.Include(x => x.PostImages).FirstOrDefaultAsync(x => x.Id == id);
                PostDto output = new PostDto
                {
                    Id = post1.Id,
                    Title = post1.Title,
                    Content = post1.Content,
                    PreviewContent = post1.PreviewContent,
                    DatePosted = post1.DatePosted,
                    Status = post1.Status,
                    WhoCanSeePost = post1.WhoCanSeePost,
                    PostedBy = post1.PostedBy,
                    SortOrder = post1.SortOrder,
                    PostByteImages = post1.PostImages.OrderByDescending(x=>x.Id).FirstOrDefault().ImageContent

                };

                if (output == null)
                {
                    return HttpNotFound();
                }
                return View(output);
            }
            else if (source == "abc")
            {

                try
                {
                   
                    string endurl = "http://iskools.com/api/ApiUrl/" + id;
                    
                    string apiUrl = String.Format(endurl);

                    WebRequest requestObj = WebRequest.Create(apiUrl);
                    requestObj.Method = "GET";

                    HttpWebResponse responseGet = null;
                    responseGet = (HttpWebResponse)requestObj.GetResponse();
                    string result = null;
                    using (Stream stream = responseGet.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream);
                        result = sr.ReadToEnd();
                        PostDto c = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<PostDto>(result));


                        var output = new PostDto
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
                            PostImage = c.PostImage

                        };
                        sr.Close();
                        if (output == null)
                        {
                            return HttpNotFound();
                        }
                        return View(output);

                    }

                   
                }

                catch (WebException webex)
                {
                    WebResponse errResp = webex.Response;
                    using (Stream respStream = errResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream);
                        string text = reader.ReadToEnd();
                    }
                }


               
            }
            
            
            return HttpNotFound();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
