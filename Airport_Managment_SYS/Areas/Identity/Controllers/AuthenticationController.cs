using Airport_Managment_SYS.Areas.Identity.ViewModels;
using Airport_Managment_SYS.Utilities;
using Airport_Managment_SYS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        [HttpGet]
        public async Task<IActionResult> Register()
        {

            //var model = new RegisterVM();

            //model.Nationalities = _context.Nationalities
            //    .OrderBy(n => n.Name)
            //    .Select(n => new SelectListItem
            //    {
            //        Value = n.Id.ToString(),
            //        Text = n.Name
            //    })
            //    .ToList();
            var nationalities= await _NationalitiesRepository.GetAsync();
            return View(new RegisterVM()
            {
                Nationalities =nationalities
            });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {

            if (!ModelState.IsValid)
            {
                registerVM.Nationalities = await _NationalitiesRepository.GetAsync();
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
                registerVM.Nationalities = await _NationalitiesRepository.GetAsync();
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
            
           bool isAdmin= await _userManager.IsInRoleAsync(user, StaticVariables.SUPER_ADMIN);
                
                if (isAdmin) 
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return RedirectToAction("Index", "Home", new { area = "Customer" });
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

   [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Authentication", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Try signing in with an external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/Customer/Home/Index");
            }

            // If the user cannot log in, try finding them by email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var username = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create a new user if they do not exist
                    Random random = new Random();
                    int r = random.Next(1000, 9999);
                    user = new ApplicationUser
                    {
                        UserName = username.Replace(" ","")+r.ToString(),
                        Email = email,
                        EmailConfirmed = true
                    };
                    var createUserResult = await _userManager.CreateAsync(user);

                    if (!createUserResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user.");
                        return RedirectToAction(nameof(Login));
                    }
                    var roleResult = await _userManager.AddToRoleAsync(user, StaticVariables.USER);
                    
                  if(!roleResult.Succeeded )
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Ensure the external login is linked
                var existingLogins = await _userManager.GetLoginsAsync(user);
                var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == info.LoginProvider);

                if (!hasGoogleLogin)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error linking external login.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? "/Customer/Home/Index");
            }

            return RedirectToAction(nameof(Login));
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
