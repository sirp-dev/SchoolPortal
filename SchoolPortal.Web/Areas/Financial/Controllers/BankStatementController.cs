using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]
    public class BankStatementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IBankStatementService _bankStatementService = new BankStatementService();

        public BankStatementController(BankStatementService bankStatementService)
        {
            _bankStatementService = bankStatementService;
        }

        public BankStatementController()
        {

        }
        // GET: Financial/BankStatement
        public async Task<ActionResult> Index()
        {
            var item = await _bankStatementService.BankStatementList();
            return View(item);
        }

        public ActionResult CreateStatement()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateStatement(BankDetails bank)
        {
            if (ModelState.IsValid)
            {
                await _bankStatementService.Create(bank);
                TempData["success"] = "Bank Statement Created Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to create Bank Statement";
            return RedirectToAction("Index");
        }
    }
}