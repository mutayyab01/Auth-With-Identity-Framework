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
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please Enter First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please Enter Last Name")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please Select Gender")]
        public string? Gender { get; set; }
        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Please Select Date on Birth")]
        public DateTime? BirthDate { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        [Display(Name = "Active")]
        public bool Status { get; set; }
        [Required(ErrorMessage = "Please Enter Username")]
        public string Username { get; set; } = default!;
    }
}
