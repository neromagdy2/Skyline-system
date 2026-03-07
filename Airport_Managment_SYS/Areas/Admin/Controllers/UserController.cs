using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airport_Managment_SYS.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");

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
            if (id ==null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();
               
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IFormCollection collection)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.UserName = collection["name"];
            user.Email = collection["email"];
            user.PhoneNumber = collection["phonenumber"];
            await _userManager.UpdateAsync(user);
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
