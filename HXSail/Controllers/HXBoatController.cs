using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HXSail.Models;
using Microsoft.AspNetCore.Http;

namespace HXSail.Controllers
{
    public class HXBoatController : Controller
    {
        private readonly SailContext _context;

        public HXBoatController(SailContext context)
        {
            _context = context;
        }

        // Present the list of boats from the boat table
        public async Task<IActionResult> Index(Int32? memberId, string fullName)
        {
            if (memberId != null)
            {
                HttpContext.Session.SetString(nameof(memberId), memberId.ToString());
                HttpContext.Session.SetString(nameof(fullName), fullName);
            }
            else if (HttpContext.Session.GetString(nameof(memberId)) != null)
            {
                memberId = Convert.ToInt32(HttpContext.Session.GetString(nameof(memberId)));
                fullName = HttpContext.Session.GetString(nameof(fullName));
            }
            else if (Request.Cookies["memberId"] != null)
            {
                memberId = Convert.ToInt32(Request.Cookies["memberId"]);
                fullName = Request.Cookies["fullName"];
            }
            else
            {
                TempData["message"] = "Please select a member to see their boats.";
                return Redirect("/HXMember/index");
            }
            var sailContext = _context.Boat.Include(b => b.BoatType).Include(b => b.Member).Include(b => b.ParkingCodeNavigation)
                                .Where(b => b.MemberId == memberId).OrderBy(b => b.BoatClass);
            return View(await sailContext.ToListAsync());
        }

        // Get the details of a boat
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .FirstOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // Present a page to create a new boat record
        public IActionResult Create()
        {
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(a => a.Name), "BoatTypeId", "Name");
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName");

            ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(a => a.ActualBoatId == null || a.ParkingCode == "").OrderBy(a => a.ParkingCode), "ParkingCode", "ParkingCode");

            return View();
        }

        // post-back the created record if it passes and return back to the index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(boat);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "new boat added: " + boat.BoatId;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error adding new boat: {ex.GetBaseException().Message}");
            }
            Create();
            return View(boat);
        }

        // present a page to edit the selected record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            if (boat.ParkingCode == null || boat.ParkingCode == "")
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(a => a.ActualBoatId == null || a.ParkingCode == "").OrderBy(a => a.ParkingCode), "ParkingCode", "ParkingCode", boat.ParkingCode);
            }
            else
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(a => a.BoatTypeId == boat.BoatTypeId).OrderBy(a => a.ParkingCode), "ParkingCode", "ParkingCode", boat.ParkingCode);
            }
            return View(boat);
        }

        // post-back the updated record if it passes edits and return back to the index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            if (id != boat.BoatId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(boat);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "boat edited: " + boat.BoatId;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error editing boat: {ex.GetBaseException().Message}");
            }

            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            if (boat.ParkingCode == null || boat.ParkingCode == "")
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(a => a.ActualBoatId == null || a.ParkingCode == "").OrderBy(a => a.ParkingCode), "ParkingCode", "ParkingCode", boat.ParkingCode);
            }
            else
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(a => a.BoatTypeId == boat.BoatTypeId).OrderBy(a => a.ParkingCode), "ParkingCode", "ParkingCode", boat.ParkingCode);
            }

            return View(boat);
        }

        // present a page of information for the item to be deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .FirstOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // post-back the deleted record if it passes the delete and return back to the index page
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boat = await _context.Boat.FindAsync(id);
            _context.Boat.Remove(boat);
            await _context.SaveChangesAsync();
            TempData["message"] = "boat deleted: " + boat.BoatId;
            return RedirectToAction(nameof(Index));
        }

        private bool BoatExists(int id)
        {
            return _context.Boat.Any(e => e.BoatId == id);
        }
    }
}
