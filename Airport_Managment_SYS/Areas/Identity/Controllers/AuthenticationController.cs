using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Airport_Managment_SYS.ViewModels;
using Airport_Managment_SYS.Areas.Identity.ViewModels;
using Airport_Managment_SYS.Utilities;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Identity.Controllers
{ 
    [Area("Identity")]
    public class AuthenticationController : Controller
    {
       private readonly IRepository<Nationalities> _NationalitiesRepository;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthenticationController(IRepository<Nationalities> nationalitiesRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _NationalitiesRepository= nationalitiesRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }



        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            var model = new RegisterVM();
            model.Nationalities = _NationalitiesRepository.GetAsync().Result;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {

            if (!ModelState.IsValid)
            {
                registerVM.Nationalities = _NationalitiesRepository.GetAsync().Result;
                return View(registerVM);
              
            }
            ApplicationUser user = new ApplicationUser()
            {

                UserName = registerVM.UserName,
                Email = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                NationalitiesId = registerVM.NationalitiesId > 0 ? registerVM.NationalitiesId : null,
                 
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }

            // Default newly registered users to "User" (Customer) role
            var roleResult = await _userManager.AddToRoleAsync(user, StaticVariables.USER);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Authentication",
                new { area = "Identity", token, userId = user.Id },
                Request.Scheme);
            await _emailSender.SendEmailAsync(registerVM.Email, "Confirm your email",
                $"Please confirm your Authentication by <a href='{link}'>clicking here</a>.");
            return RedirectToAction("Login");

        }
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                TempData["Error"] = "Invalid User";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Email confirmation failed";
            }
            else
            {
                TempData["Success"] = "Email confirmed successfully";
            }
            return RedirectToAction("Login");
        }
        //[HttpPost]
        //public async Task<IActionResult> ConfirmEmail()
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        // if (user == null)
        //    {
        //       TempData["Error"] = "Invalid User";
        //    }
        //  var result=  await _userManager.ConfirmEmailAsync(user, token);
        //   if (!result.Succeeded)
        //    {
        //        TempData["Error"] = "Email confirmation failed";
        //    }else
        //    {
        //        TempData["Success"] = "Email confirmed successfully";
        //    }
        //    return RedirectToAction("Login");
        //}
        public IActionResult Login()
        {
            return View(new LoginVM());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {

            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginVM);
            }

            //var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            //if (!isPasswordValid)
            //{
            //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //    return View(loginVM);
            //}
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, true, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your Authentication is locked out. Please try again later.");
                    return View(loginVM);
                }
                else if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "You need to confirm your email before logging in.");
                    return View(loginVM);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(loginVM);
                }
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(model.EmailOrUsername);

            if (user == null || !user.EmailConfirmed)
            {
                TempData["Error"] = "Invalid user or email not confirmed";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(
                "ChangePassword",
                "Authentication",
                new { email = user.Email, token = token },
                Request.Scheme
            );

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Reset your password by <a href='{link}'>clicking here</a>"
            );

            TempData["Success"] = "Password reset link sent to your email";
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid password reset link";
                return RedirectToAction("Login");
            }

            return View(new ChangePasswordVM
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid user");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Authentication", new { area = "Identity" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
