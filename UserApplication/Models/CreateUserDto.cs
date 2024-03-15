using System.ComponentModel.DataAnnotations;

namespace UserApplication.Models
{
    /// <summary>
    /// DTO for User creation
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// Name of user
        /// </summary>
        [Required(ErrorMessage = "Please enter valid name")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password of user
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
