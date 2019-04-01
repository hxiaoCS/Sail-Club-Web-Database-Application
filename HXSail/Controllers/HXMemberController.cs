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
    //access and maintain the Member table
    // Han X. Sep 2018
    public class HXMemberController : Controller
    {
        private readonly SailContext _context;

        public HXMemberController(SailContext context)
        {
            _context = context;
        }

        // Get all records from the table showing on the list
        public async Task<IActionResult> Index()
        {
            var sailContext = _context.Member.Include(m => m.ProvinceCodeNavigation).OrderBy(a => a.FullName);
            return View(await sailContext.ToListAsync());
        }

        // Get the detail information of a selected item
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // Present an empty table for creating a new item
        public IActionResult Create()
        {
            var members = _context.Member;
            foreach (var item in members)
            {
                item.FullName = item.FirstName + " " + item.LastName;
            }
            
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        //post-back the created item if it passes new record and return back to the index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "new member added: " + member.FullName;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error applying the create: {ex.GetBaseException().Message}");
            }

            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", member.ProvinceCode);
            return View(member);
        }

        //present a page with the selected item to be edited 
        public async Task<IActionResult> Edit(int? id)
        {
            TempData["fullName"] = _context.Member.Where(a => a.MemberId == id).Select(a => a.FullName).FirstOrDefault();

            if (id == null)
            {
                TempData["message"] = "no key supplied for the record to be updated";
                return RedirectToAction("index");
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                TempData["message"] = "record key not found";
                return RedirectToAction("index");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a => a.Name), "ProvinceCode", "Name");
            return View(member);
        }

       //post-back the updated record if it passes edits and return back to the index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {
            if (id != member.MemberId)
            {
                TempData["message"] = "no key supplied for the record to be updated";
                return RedirectToAction("edit");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"member updated: {member.FullName}";
                    return RedirectToAction(nameof(Index));
                }  
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",
                    $"error applying the update: {ex.GetBaseException().Message}");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a => a.Name), "ProvinceCode", "Name");
            return View(member);
        }

        // Present information for the selected item to be deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "No key provided for member to be deleted";
                return RedirectToAction(nameof(Index));
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                TempData["message"] = "member key not found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // post back the delete operation if it passes delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            try
            {
                var member = await _context.Member.FindAsync(id);
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
                TempData["message"] = $"member deleted: {member.FullName}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",
                    $"error applying to the delete: {ex.GetBaseException().Message}");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}
