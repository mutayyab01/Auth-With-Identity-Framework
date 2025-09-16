using AuthWithIdentityFramework.Repository.Interface;
using AuthWithIdentityFramework.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AuthWithIdentityFramework.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (checkEmail != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists");
                        return View(model);
                    }
                    var user = new IdentityUser()
                    {
                        UserName = model.Email,
                        Email = model.Email,
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        bool status = await _emailSender.SendEmailAsync(model.Email, "Registration Success", "Welcome to our site Your registration is successful.");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser checkEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (checkEmail == null)
                    {
                        ModelState.AddModelError("Email", "Email Not Fount! Please Register First!");
                        return View(model);
                    }
                    if (await _userManager.CheckPasswordAsync(checkEmail, model.Password) == false)
                    {
                        ModelState.AddModelError("LoginError", "Invalid Credentials");
                        return View(model);
                    }
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("Intruder", "Invalid Login Attempt!");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
