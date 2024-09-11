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
using System.Net.NetworkInformation;
using System.Management;

namespace SchoolPortal.Web.Areas.SuperUser.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    public class ApprovedDevicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SuperUser/ApprovedDevices
        public async Task<ActionResult> Index()
        {
            ViewBag.mc1 = GetMAC();
            ViewBag.mc2 = GetMACAddress();
            ViewBag.mc3 = GetMACAddress2();
            return View(await db.ApprovedDevices.ToListAsync());
        }
        public string GetMACAddress2()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        private string GetMAC()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        public string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty) // only return MAC Address from first card   
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }
        // GET: SuperUser/ApprovedDevices/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedDevice approvedDevice = await db.ApprovedDevices.FindAsync(id);
            if (approvedDevice == null)
            {
                return HttpNotFound();
            }
            return View(approvedDevice);
        }

        // GET: SuperUser/ApprovedDevices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperUser/ApprovedDevices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MacAddress,ImelNumber,IpAddress,Date,DeviceThatAddedThis")] ApprovedDevice approvedDevice)
        {
            if (ModelState.IsValid)
            {
                db.ApprovedDevices.Add(approvedDevice);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(approvedDevice);
        }

        // GET: SuperUser/ApprovedDevices/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedDevice approvedDevice = await db.ApprovedDevices.FindAsync(id);
            if (approvedDevice == null)
            {
                return HttpNotFound();
            }
            return View(approvedDevice);
        }

        // POST: SuperUser/ApprovedDevices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MacAddress,ImelNumber,IpAddress,Date,DeviceThatAddedThis")] ApprovedDevice approvedDevice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(approvedDevice).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(approvedDevice);
        }

        // GET: SuperUser/ApprovedDevices/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedDevice approvedDevice = await db.ApprovedDevices.FindAsync(id);
            if (approvedDevice == null)
            {
                return HttpNotFound();
            }
            return View(approvedDevice);
        }

        // POST: SuperUser/ApprovedDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ApprovedDevice approvedDevice = await db.ApprovedDevices.FindAsync(id);
            db.ApprovedDevices.Remove(approvedDevice);
            await db.SaveChangesAsync();
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
