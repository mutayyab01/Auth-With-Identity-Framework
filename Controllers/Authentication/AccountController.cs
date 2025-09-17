using AuthWithIdentityFramework.Repository.Interface;
using AuthWithIdentityFramework.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AuthWithIdentityFramework.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
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
            Response response = new Response();
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
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = userId, token = code }, protocol: HttpContext.Request.Scheme);

                        string emailBody = GetEmailBody(model.Email, "Email Confirmation Link", confirmationLink, "EmailConfirmation");
                        bool status = await _emailSender.SendEmailAsync(model.Email, "Email Confirmation Link", emailBody);
                        if (status)
                        {
                            response.StatusCode = "Success";
                            response.Message = "Registration Successful! Please check your email to confirm your account.";
                            return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                        }
                        //await _signInManager.SignInAsync(user, isPersistent: false);
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
                        ModelState.AddModelError(string.Empty, "Invalid Credentials");
                        return View(model);
                    }
                    bool confirmStatus = await _userManager.IsEmailConfirmedAsync(checkEmail);
                    if (!confirmStatus)
                    {
                        ModelState.AddModelError(string.Empty, "Email Not Confirmed! Please Confirm the Email First! then Login Again.");
                        return View(model);
                    }
                    else
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                    }
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

        public string GetEmailBody(string? username, string? title, string? callbackUrl, string? EmailTemplateName)
        {
            string LoginURL = _configuration.GetValue<string>("URLs:LoginURL");
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", $"{EmailTemplateName}.cshtml");
            string htmlString = System.IO.File.ReadAllText(path);
            htmlString = htmlString.Replace("{{title}}", title);
            htmlString = htmlString.Replace("{{Username}}", username);
            htmlString = htmlString.Replace("{{url}}", LoginURL);
            htmlString = htmlString.Replace("{{callbackUrl}}", callbackUrl);
            return htmlString;
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordOrUsernameVM model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("UserId");
            ModelState.Remove("Token");
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, Token = code }, protocol: HttpContext.Request.Scheme);
                string emailBody = GetEmailBody("", "Reset Password", callbackUrl, "ResetPassword");

                //bool isSendEmail = await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //    $"Please reset your password by clicking here: <a href='{callbackUrl}' style='background-color:#04aa6d;border:none;color:white;padding:10px;" +
                //    $"text-align:center;text-decoration:none;display:inline-block;font-size:16px;margin:4px 2px;cursor:pointer;border-radius:10px;'> Click Here</a>");
                bool isSendEmail = await _emailSender.SendEmailAsync(model.Email, "Reset Password", emailBody);
                if (isSendEmail)
                {
                    Response response = new Response()
                    {
                        StatusCode = "Success",
                        Message = "Reset Password link"
                    };
                    return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                }
            }

            return View();
        }
        public IActionResult ForgetPasswordConfirmation(Response response)
        {

            return View(response);
        }

        public IActionResult ResetPassword(string userId, string Token)
        {
            var model = new ForgetPasswordOrUsernameVM
            {
                Token = Token,
                UserId = userId
            };

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgetPasswordOrUsernameVM forget)
        {
            Response response = new Response();
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View(forget);
            }
            var user = await _userManager.FindByIdAsync(forget.UserId);
            if (user == null)
            {
                return View(forget);
            }
            var result = await _userManager.ResetPasswordAsync(user, forget.Token, forget.Password);
            if (result.Succeeded)
            {
                response.Message = "Your Password has been Successfully Reset";
                return RedirectToAction("ForgetPasswordConfirmation", response);
            }
            return View(forget);

        }
        public async Task<IActionResult> ConfirmEmail(string userId, string Token)
        {
            Response response = new Response();
            if (userId != null && Token != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return View("Error");
                }
                var result = await _userManager.ConfirmEmailAsync(user, Token);
                if (result.Succeeded)
                {
                    response.Message = "Thank you for Confirming Your Email!";
                    return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                }
            }
            return View("Error");
        }


    }
}
