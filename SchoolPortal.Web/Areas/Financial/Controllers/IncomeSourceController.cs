using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]
    public class IncomeSourceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IIncomeService _incomeService = new IncomeService();

        public IncomeSourceController()
        {

        }

        public IncomeSourceController(IncomeService incomeService)
        {
            _incomeService = incomeService;
        }
        // GET: Financial/Income
        public async Task<ActionResult> Index()
        {
            var income = await _incomeService.IncomeList();
            return View(income);
        }

        // GET: Financial/Income/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _incomeService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Financial/Income/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Financial/Income/Create
        [HttpPost]
        public async Task<ActionResult> Create(Income item)
        {

            if (ModelState.IsValid)
            {
               
                await _incomeService.Create(item);
                TempData["success"] = "Income Added Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Unable to Add Income";
            return View();

        }

        // GET: Financial/Income/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _incomeService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Financial/Income/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Income item)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(item).State = EntityState.Modified;
                //await db.SaveChangesAsync();
                await _incomeService.Edit(item);
                TempData["success"] = "Income Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Income";
            return View(item);
        }

        // GET: Financial/Income/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Income item = await _incomeService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Financial/Income/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //Income item = await db.Incomes.FindAsync(id);
            //db.Incomes.Remove(item);
            //await db.SaveChangesAsync();
            await _incomeService.Delete(id);
            return RedirectToAction("Index");
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
