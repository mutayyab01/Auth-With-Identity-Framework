using System.ComponentModel.DataAnnotations;

namespace AuthWithIdentityFramework.ViewModels
{
    public class LoginVM
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; } = default!;
        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
