using System.ComponentModel.DataAnnotations;

namespace UserApplication.Models
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Please enter valid name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Password { get; set; }
    }
}
