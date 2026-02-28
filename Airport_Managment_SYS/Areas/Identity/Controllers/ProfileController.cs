using Airport_Managment_SYS.ViewModels;
using Airport_Managment_SYS.Areas.Identity.ViewModels;
using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment_SYS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles = StaticVariables.SUPER_ADMIN + "," + StaticVariables.ADMIN + "," + StaticVariables.USER + ",Customer")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbcontext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbcontext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.ApplicationUsers
                .Include(u => u.National)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
                return Challenge();

            var roles = await _userManager.GetRolesAsync(user);

            return View(new ProfileVM
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                NationalitiesId = user.NationalitiesId,
                National = user.National,
                Roles = roles
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _context.ApplicationUsers
                .Include(u => u.National)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            
            if (user == null)
                return Challenge();

            // Get all nationalities for dropdown
            var nationalities = await _context.Nationalities.OrderBy(n => n.Name).ToListAsync();
            ViewBag.Nationalities = new SelectList(nationalities, "Id", "Name");

            return View(new EditProfileVM
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                NationalitiesId = user.NationalitiesId,
                National = user.National
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            if (!ModelState.IsValid)
            {
                // Get nationalities again for dropdown in case of validation error
                var nationalities = await _context.Nationalities.OrderBy(n => n.Name).ToListAsync();
                ViewBag.Nationalities = new SelectList(nationalities, "Id", "Name");
                return View(model);
            }

            // Update username via UserManager to keep identity normalized fields consistent
            if (!string.Equals(user.UserName, model.UserName, StringComparison.Ordinal))
            {
                var setUserName = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserName.Succeeded)
                {
                    foreach (var error in setUserName.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }

            user.PhoneNumber = model.PhoneNumber;
            user.NationalitiesId = model.NationalitiesId > 0 ? model.NationalitiesId : null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            // If username changed, refresh auth cookie
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Profile updated successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordAuthenticatedVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordAuthenticatedVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password changed successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

