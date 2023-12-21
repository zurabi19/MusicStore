using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore__1_.Models;
using System.Security.Claims;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicStore__1_.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<IActionResult> CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }
            }
            return View(model);
        }
         
        public async Task<IActionResult> ListRoles()
        {
            List<RoleViewModel> roles = new List<RoleViewModel>();
            foreach (var role in await roleManager.Roles.ToListAsync())
            {
                RoleViewModel roleViewModel = new RoleViewModel();
                roleViewModel.RoleName = role.Name; 
                roleViewModel.RoleId = role.Id;
                roles.Add(roleViewModel);
            }
            return View(roles);
        }

        public async Task<IActionResult> EditRole(string RoleId)
        {
            var role = await roleManager.FindByIdAsync(RoleId);
            if (role is null)
            {
                return NotFound();
            }

            EditRoleModelView model = new EditRoleModelView();
            model.RoleId = RoleId;
            model.RoleName = role.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModelView model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role is null)
            {
                return NotFound();
            }

            role.Name = model.RoleName;
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            List<UserInRoleViewModel> users = new List<UserInRoleViewModel>();
            foreach (var user in await userManager.Users.ToListAsync())
            {
                UserInRoleViewModel model = new UserInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.IsChecked = true;
                }
                else { model.IsChecked = false; }
                users.Add(model);
            }

            UsersInRoleRealViewModel viewModel = new UsersInRoleRealViewModel()
            {
                RoleId = roleId,
                Users = users
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(UsersInRoleRealViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            for(int i = 0; i < model.Users.Count; i++)
            {
                IdentityResult result = null;
                var user = await userManager.FindByIdAsync(model.Users[i].UserId);
                if (model.Users[i].IsChecked && !await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model.Users[i].IsChecked && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if(result.Succeeded)
                {
                    if(  i < (model.Users.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new {roleId = model.RoleId});
                    }
                }
            }
            return RedirectToAction("EditRole", new { roleId = model.RoleId });
        }

        public async Task<IActionResult> ManageClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingClaims = await userManager.GetClaimsAsync(user);

            ManageClaimsViewModel model = new ManageClaimsViewModel();
            model.UserId = userId;
            model.UserClaims = new List<UserClaim>();

            foreach (var claim in StoredClaims.claims.ToList())
            {
                UserClaim userClaim = new UserClaim()
                {
                    ClaimType = claim.Type
                };

                if (existingClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.UserClaims.Add(userClaim); 
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ManageClaims(ManageClaimsViewModel model)
        {
            var user =await userManager.FindByIdAsync(model.UserId);
            if(user == null)
            {
                return NotFound();
            }

            for ( int i = 0; i < model.UserClaims.Count; i++ )
            {
                if (User.Claims.Any(c => c.Type == model.UserClaims[i].ClaimType && !model.UserClaims[i].IsSelected))
                {
                    await userManager.RemoveClaimAsync(user, StoredClaims.claims[i]);
                }else if(!User.Claims.Any(c => c.Type == model.UserClaims[i].ClaimType && model.UserClaims[i].IsSelected))
                {
                    await userManager.AddClaimAsync(user, StoredClaims.claims[i]);
                }
            }

            return RedirectToAction("Index", "Store");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesDenied()
        {
            return View();
        }
    }
}



