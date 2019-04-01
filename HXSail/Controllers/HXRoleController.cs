using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace HXSail.Controllers
{
    //access and maintain the role table
    [Authorize(Roles = "administrators")]
    public class HXRoleController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public HXRoleController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(a => a.Name);
            return View(roles);
        }


        //post-back action for creating a new role and return to index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (roleName == null) roleName = "";
            roleName = roleName.ToString().Trim();

            //check if it is blanks
            if (roleName == "")
            {
                TempData["message"] = "Role name can not be empty.";
                return RedirectToAction("index");
            }
            //check if the name already exists
            else if (await roleManager.RoleExistsAsync(roleName))
            {
                TempData["message"] = $"Proposed role name '{roleName}' is already on file.";
                return RedirectToAction("index");
            }

            try
            {
                IdentityResult identityResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (identityResult.Succeeded)
                {
                    TempData["message"] = $"role created: {roleName}";
                    return RedirectToAction(nameof(Index));
                }
                else
                    TempData["message"] = $"error creating role: {identityResult.Errors.FirstOrDefault().Description}";
            }
            catch (Exception ex)
            {
                TempData["message"] = $"error creating role: {ex.GetBaseException().Message}";
            }
            return RedirectToAction("index");
        }

        //present a confirmation page for the item to be deleted, and return back to the index page
        public IActionResult DeleteRole(string roleId, string roleName)
        {
            if (roleName != null)
            {
                HttpContext.Session.SetString(nameof(roleName), roleName);
            }
            else if (HttpContext.Session.GetString(nameof(roleName)) != null)
            {
                roleName = HttpContext.Session.GetString(nameof(roleName));
            }
            else if (Request.Cookies["roleName"] != null)
            {
                roleName = Request.Cookies["roleName"];
            }
            else
            {
                TempData["message"] = "Please select a role to be deleted.";
                return RedirectToAction("index");
            }

            if (roleId == null)
            {
                TempData["message"] = "No key supplied to delete this role.";
                return RedirectToAction(nameof(Index));
            }
            
            var roles = roleManager.Roles.OrderBy(a => a.Name).FirstOrDefault(a => a.Name == roleName);
            if (roles == null)
            {
                TempData["message"] = "role key not found";
                return RedirectToAction(nameof(Index));
            }
            return View(roles);
        }

        //it confirm the delete and save to database if the deletition request passes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(IdentityRole role)
        {
            if (role.Name == null) role.Name = "";
            role.Name = role.Name.ToString().Trim();

            try
            {
                var userInRole = await userManager.GetUsersInRoleAsync(role.Name);
                IdentityRole roleToDelete = await roleManager.FindByNameAsync(role.Name);
                if (role.Name == "administrators")
                {
                    TempData["message"] = "administrators can not be deleted.";
                    return RedirectToAction(nameof(Index));
                }
                else if (userInRole == null)
                {
                    if(await roleManager.RoleExistsAsync(role.Name))
                    {
                        IdentityResult result = await roleManager.DeleteAsync(roleToDelete);
                        if (!result.Succeeded)
                            throw new Exception($"problem deleting role: {result.Errors.FirstOrDefault().Description}");
                        TempData["message"] = $"role '{role.Name}' deleted";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ViewBag.usersInRole = new SelectList(userInRole, "Id", "UserName");
                    IdentityResult result = await roleManager.DeleteAsync(roleToDelete);
                    if (!result.Succeeded)
                        throw new Exception($"problem deleting role: {result.Errors.FirstOrDefault().Description}");
                    TempData["message"] = $"role '{role.Name}' deleted";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception deleting role: {ex.GetBaseException().Message}.";
            }
            return RedirectToAction("index");
        }

        //present a page with all users in the selected role and a dropdown list to add more users 
        public async Task<IActionResult> UsersInRole(string roleId, string roleName)
        {
            if (roleName != null)
            {
                HttpContext.Session.SetString(nameof(roleName), roleName);
            }
            else if (HttpContext.Session.GetString(nameof(roleName)) != null)
            {
                roleName = HttpContext.Session.GetString(nameof(roleName));
            }
            else if (Request.Cookies["roleName"] != null)
            {
                roleName = Request.Cookies["roleName"];
            }
            else
            {
                TempData["message"] = "Please select a role to view its users.";
                return RedirectToAction("index");
            }


            var users = userManager.Users.OrderBy(a => a.UserName);
            var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
            List<IdentityUser> usersNotInRole = new List<IdentityUser>();
            foreach (var item in users)
            {
                if (!usersInRole.Contains(item))
                {
                    usersNotInRole.Add(item);
                }
            }
            ViewBag.usersNotInRole = new SelectList(usersNotInRole, "Id", "UserName");

            return View(usersInRole);
        }

        //it adds more users to the selected role when userId and roleName passes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotInRoleUser(string userId, string roleName)
        {
            try
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                IdentityResult addResult = await userManager.AddToRoleAsync(user, roleName);
                if (addResult.Succeeded)
                {
                    TempData["message"] = $"user '{user.UserName}' added to role '{roleName}'";
                    return RedirectToAction(nameof(UsersInRole));
                }
                else
                {
                    TempData["message"] = $"problem deleting role: {addResult.Errors.FirstOrDefault().Description}";
                    return RedirectToAction(nameof(UsersInRole));
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception on adding the selected user to this role: {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        //it removes the selected user from the current role
        public async Task<IActionResult> RemoveFromRole(string userId, string userName, string roleName)
        {
            try
            {
                var user = userManager.FindByIdAsync(userId).Result;
                IdentityResult result = await userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                    throw new Exception($"problem removing user from role: {result.Errors.FirstOrDefault().Description}");
                TempData["message"] = $"user '{userName}' removed from the role";
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception on remove the user from role: {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(UsersInRole));
        }
    }
}