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
    //access and maintain the membershipType table
    //Han X.  2018 Sep
    public class HXMembershipTypeController : Controller
    {
        private readonly SailContext _context;

        public HXMembershipTypeController(SailContext context)
        {
            _context = context;
        }

        // get all records from the MemberType table showing on the list
        public async Task<IActionResult> Index()
        {
            var sailContext = _context.MembershipType;
            return View(await _context.MembershipType.ToListAsync());
        }

        // get all detailed information for the selected item
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .FirstOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // Present an empty page to enter a new Membership Type
        public IActionResult Create()
        {
            var sailContext = _context.MembershipType;
            return View();
        }

        //post-back create: add the new record to the file if it passes edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipType);
        }

        // present a page with available information of the selected record to be edited
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType.FindAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        // Post-back edit: edit the selected Membership Type if it passes edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            if (id != membershipType.MembershipTypeName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipTypeExists(membershipType.MembershipTypeName))
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
            return View(membershipType);
        }

        // Present a page with available information of the selected record to be deleted
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .FirstOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // post-back delete: delete the selected record if the action is confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var membershipType = await _context.MembershipType.FindAsync(id);
            _context.MembershipType.Remove(membershipType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Return true if there is a Membership Type with the given ID 
        private bool MembershipTypeExists(string id)
        {
            return _context.MembershipType.Any(e => e.MembershipTypeName == id);
        }
    }
}
