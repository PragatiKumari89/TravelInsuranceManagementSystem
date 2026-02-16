using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TravelInsuranceManagementSystem.Repo.Models
{
    // Inherits from IdentityUser to use built-in Auth features (Password Hashing, Roles)
    public class User : IdentityUser<int>
    {

        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string FullName { get; set; }

        public string Role { get; set; } = "User";

        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public string Password { get; set; }
    }
}