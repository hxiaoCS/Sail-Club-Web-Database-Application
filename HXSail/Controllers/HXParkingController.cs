using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HXSail.Models;

namespace HXSail.Controllers
{
    public class HXParkingController : Controller
    {
        private readonly SailContext _context;

        public HXParkingController(SailContext context)
        {
            _context = context;
        }

        // present the list of records from the parking table
        public async Task<IActionResult> Index()
        {
            var sailContext = _context.Parking.Include(p => p.BoatType).OrderBy(p => p.ParkingCode);
            return View(await sailContext.ToListAsync());
        }

        // get the details of the selected record
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                TempData["message"] = "can not get the details because there is no id for this record.";
                return RedirectToAction(nameof(Index));
            }

            var parking = await _context.Parking
                .Include(p => p.BoatType)
                .FirstOrDefaultAsync(m => m.ParkingCode == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // present an empty page to create a new record
        public IActionResult Create()
        {
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(a => a.Name), "BoatTypeId", "Name");
            return View();
        }

        //post-back the new record if it passes and return back the index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParkingCode,BoatTypeId,ActualBoatId")] Parking parking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(parking);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "parking space added: " + parking.ParkingCode;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error adding boat: {ex.GetBaseException().Message}");
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(a => a.Name), "BoatTypeId", "Name", parking.BoatTypeId);
            return View(parking);
        }

        // present a page to edit the selected item
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                TempData["message"] = "can not edit it because there is no id for this record.";
                return RedirectToAction(nameof(Index));
            }

            var parking = await _context.Parking.FindAsync(id);
            if (parking == null)
            {
                return NotFound();
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(a => a.Name), "BoatTypeId", "Name", parking.BoatTypeId);
            return View(parking);
        }

        // post-back the updated record if it passes edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ParkingCode,BoatTypeId,ActualBoatId")] Parking parking)
        {
            if (id != parking.ParkingCode)
            {
                return NotFound();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(parking);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "parking space edited: " + parking.ParkingCode;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error editing boat: {ex.GetBaseException().Message}");
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(a => a.Name), "BoatTypeId", "Name", parking.BoatTypeId);
            return View(parking);
        }

        // present a page of information for the item to be deleted
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                TempData["message"] = "can not delete it because there is no id for this record.";
                return RedirectToAction(nameof(Index));
            }

            var parking = await _context.Parking
                .Include(p => p.BoatType)
                .FirstOrDefaultAsync(m => m.ParkingCode == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // post-back the delete record if it passes the delete and return back to the index page
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var parking = await _context.Parking.FindAsync(id);
            _context.Parking.Remove(parking);
            await _context.SaveChangesAsync();
            TempData["message"] = "parking space deleted: " + parking.ParkingCode;
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingExists(string id)
        {
            return _context.Parking.Any(e => e.ParkingCode == id);
        }
    }
}
