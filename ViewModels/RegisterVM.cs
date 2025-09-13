using System.ComponentModel.DataAnnotations;

namespace AuthWithIdentityFramework.ViewModels
{
    public class RegisterVM
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; } = default!;
        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]

        public string Password { get; set; } = default!;
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        //[DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = default!;



    }
}
