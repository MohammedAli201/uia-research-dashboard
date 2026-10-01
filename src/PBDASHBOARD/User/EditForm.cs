using System.ComponentModel.DataAnnotations;

namespace PBDASHBOARD.Services
{
    public class EditForm
    {

        [Required]
        public string Email { get; set; }
        
        [Required]

        public string ToGroup { get; set; }
       
    }
}