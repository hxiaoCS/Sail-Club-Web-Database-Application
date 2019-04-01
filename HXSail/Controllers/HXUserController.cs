using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HXSail.Data;
using HXSail.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HXSail.Controllers
{
    //access and maintain the user table
    [Authorize(Roles = "administrators")]
    public class HXUserController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _applicationDb;
        List<ApplicationUser> appUsers = new List<ApplicationUser>();
        //private ApplicationUser appUser;

        public HXUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDb)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _applicationDb = applicationDb;
        }

        //present a list with all users with designated order
        public IActionResult Index(bool sortingLocked = false)
        {
            var users = userManager.Users;
            foreach(var user in users)
            {

                ApplicationUser appUser = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    IsAdmin = userManager.IsInRoleAsync(user, "administrators").Result,
                    IsMember = userManager.IsInRoleAsync(user, "members").Result,
                    IsLocal = user.PasswordHash != null,
                    IsLocked = (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                };
                appUsers.Add(appUser);
            }
            
            if (sortingLocked == true)
            {
                return View(appUsers.OrderByDescending(a => a.IsLocked).ThenBy(a => a.UserName));
            }
            else 
                return View(appUsers.OrderBy(a => a.IsMember).ThenBy(a => a.UserName));
        }

        //remove the user from its roles and then delete it
        public async Task<IActionResult> DeleteUsers(string userName)
        {
            try
            {
                IdentityUser user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    TempData["message"] = "no key supplied to delete this user";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var roles = await userManager.GetRolesAsync(user);
                    IdentityResult removeResult = await userManager.RemoveFromRolesAsync(user, roles);
                    if (!removeResult.Succeeded)
                    {
                        TempData["message"] = $"error removing '{user.UserName}' from all roles: {removeResult.Errors.FirstOrDefault().Description}";
                        return RedirectToAction(nameof(Index));
                    }   
                    else
                    {
                        IdentityResult result = await userManager.DeleteAsync(user);
                        if (result.Succeeded)
                            TempData["message"] = "user '" + user.UserName + "' deleted";
                        else
                            TempData["message"] = $"error deleting user '{user.UserName}': {result.Errors.FirstOrDefault().Description}";

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception removing user from all roles & deleting the user: {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        //add user to the role: members
        public async Task<IActionResult> AddUserToMember(string userName)
        {
            IdentityUser user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                TempData["message"] = $"no key supplied to add the user to members";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                IdentityResult result = await userManager.AddToRoleAsync(user, "members");

                if (result.Succeeded)
                {
                    TempData["message"] = $"user '{user.UserName}' added to members";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = $"errors adding user '{user.UserName}' to members";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception on adding the user to members: {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        //present a page with input field to reset password
        public IActionResult ResetPassword(string userId, string userName)
        {
            //IdentityUser user = userManager.FindByNameAsync(userName).Result;
            //if (user == null)
            //{
            //    TempData["message"] = "errors on resetting the password: no key supplied";
            //    return RedirectToAction(nameof(ResetPassword));
            //}
            return View();
        }

        //it changes the user's password when the resetting request passes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (resetPassword == null)
            {
                TempData["message"] = "no value supplied to reset the password";
                return View();
            }
            else
            {
                if (resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    TempData["message"] = "Password does not match with confirm password";
                    return View();
                }
            }
            
            try
            {
                IdentityUser user = userManager.FindByNameAsync(resetPassword.UserName).Result;
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, resetPassword.Password);
                if (result.Succeeded)
                {
                    TempData["message"] = "Password reset successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = "Error while resetting the password";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception on password resetting: {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(Index));

        }

        //it lock or unlock the user's account while pressing on the button
        public IActionResult LockOrUnlock(string userName)
        {
            ApplicationUser appUser = new ApplicationUser();
            IdentityUser user = userManager.FindByNameAsync(userName).Result;
            try
            {
                if (user != null)
                {
                    if (user.LockoutEnd != null && user.LockoutEnd> DateTime.UtcNow)
                    {
                        user.LockoutEnd = DateTime.UtcNow;
                        user.AccessFailedCount = 0;
                        _applicationDb.SaveChangesAsync();
                        TempData["message"] = $"user '{user.UserName}' now is unlocked";
                        return RedirectToAction(nameof(Index));
                    }
                    else if (user.LockoutEnd == null || user.LockoutEnd <= DateTime.UtcNow)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddYears(20);
                        _applicationDb.SaveChangesAsync();

                        TempData["message"] = $"user '{user.UserName}' now is locked";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception on lock or unlock the user {ex.GetBaseException().Message}";
            }
            return RedirectToAction(nameof(Index));
        }
            
    }
}