using System.ComponentModel.DataAnnotations;

namespace PBDASHBOARD.User.Requests
{
    public class RegistrationForm
    {
       
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }

        public string GroupName { get; set; }
        public string Role { get; set; }
    }
}