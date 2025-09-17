using System.ComponentModel.DataAnnotations;

namespace AuthWithIdentityFramework.ViewModels
{
    public class ForgetPasswordOrUsernameVM
    {
        [Required]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;

    }
}
