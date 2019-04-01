using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HXSail.Models;
using Microsoft.AspNetCore.Authorization;

namespace HXSail.Controllers
{
    //access and maintain the BoatType table
    // Han X. Sep 2018
    [Authorize(Roles = "Members")]
    public class HXBoatTypeController : Controller
    {
        private readonly SailContext _context;

        public HXBoatTypeController(SailContext context)
        {
            _context = context;
        }

        // Get all records from the BoatType table showing on the list
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var sailContext = _context.BoatType;
            return View(await _context.BoatType.ToListAsync());
        }

        // Get the detail information of the selected Boat Type
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .FirstOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // Present an empty page to enter a new Boat Type
        public IActionResult Create()
        {
            return View();
        }

        // Post-back create: add the new Boat Type to the file if it passes edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boatType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatType);
        }

        // present a page with current information for the selected Boat Type to be edited
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType.FindAsync(id);
            if (boatType == null)
            {
                return NotFound();
            }
            return View(boatType);
        }

        // Post-back edit: edit the current record if it passes edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (id != boatType.BoatTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatTypeExists(boatType.BoatTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(boatType);
        }

        // Present a page with information with the selected record to be deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .FirstOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // Post-back delete: delete the selected item if the action is confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boatType = await _context.BoatType.FindAsync(id);
            _context.BoatType.Remove(boatType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Return true if there is a boat type with the given ID
        private bool BoatTypeExists(int id)
        {
            return _context.BoatType.Any(e => e.BoatTypeId == id);
        }
    }
}
