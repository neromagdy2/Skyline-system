using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Airport_Managment_SYS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Airport_Managment_SYS.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var user = _userManager.Users.ToList();
            return View(user);
        }
        public async Task<IActionResult> LockUnlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(1);
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));

        }




        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            await _userManager.CreateAsync(new ApplicationUser { UserName = collection["name"], Email = collection["email"], PhoneNumber =collection["phonenumber"], PasswordHash = collection["password"] });
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new EditUserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvailableRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = StaticVariables.ADMIN, Text = "Admin" },
                    new SelectListItem { Value = StaticVariables.USER, Text = "User" }
                }
            };

            // Get current user role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                viewModel.SelectedRole = currentRoles.First();
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserVM model)
        {
            if (id != model.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.AvailableRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = StaticVariables.SUPER_ADMIN, Text = "Super Admin" },
                    new SelectListItem { Value = StaticVariables.ADMIN, Text = "Admin" },
                    new SelectListItem { Value = StaticVariables.USER, Text = "User" }
                };
                return View(model);
            }

            // Update role if changed
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var currentRole = currentRoles.FirstOrDefault();

                if (currentRole != model.SelectedRole)
                {
                    // Remove current role
                    if (currentRole != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, currentRole);
                    }
                    // Add new role
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }

        
        
}
