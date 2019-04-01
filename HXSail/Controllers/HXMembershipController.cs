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
    //access and maintain the Membership table
    // Han X. Sep 2018
    public class HXMembershipController : Controller
    {
        private readonly SailContext _context;

        public HXMembershipController(SailContext context)
        {
            _context = context;
        }

        // Show only membership on files for the members, and persist passed memberId or fullName
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
                TempData["message"] = "Please select a member to see its memberships.";
                return Redirect("/HXMember/index");
            }

            var sailContext = _context.Membership.Include(m => m.Member).Include(m => m.MembershipTypeNameNavigation)
                .Where(m => m.MemberId == memberId).OrderByDescending(m => m.Year);
            return View(await sailContext.ToListAsync());
        }

        // show details of a membership record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // Present an empty page for Creating a new record for membership
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName");
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(a => a.MembershipTypeName), "MembershipTypeName", "MembershipTypeName");
            return View();
        }

        //post-back a new record if it passes new record and calculate the fee before it saves to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
            if (ModelState.IsValid)
            {
                var fee = (from record in _context.AnnualFeeStructure
                          where record.Year == DateTime.Now.Year
                          select record.AnnualFee).SingleOrDefault();
 
                var ratio = (from record in _context.MembershipType
                            where record.MembershipTypeName == membership.MembershipTypeName
                            select record.RatioToFull).SingleOrDefault();
                membership.Fee = Convert.ToDouble(fee) * Convert.ToDouble(ratio);
                _context.Add(membership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(a => a.MembershipTypeName), "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // Present a page for editing on the selected item
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership.FindAsync(id);
            if (membership == null)
            {
                return NotFound();
            }
            else if (membership.Year != DateTime.Now.Year)
            {
                TempData["message"] = "can not change a prior year's record";
                return RedirectToAction("index");
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(a => a.MembershipTypeName), "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        //Post back the updated item if it passes edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
            if (id != membership.MembershipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipExists(membership.MembershipId))
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
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(a => a.MembershipTypeName), "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // Present the information of the selected item to be deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // post back the delete item if it passes the delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membership = await _context.Membership.FindAsync(id);
            _context.Membership.Remove(membership);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipExists(int id)
        {
            return _context.Membership.Any(e => e.MembershipId == id);
        }
    }
}
