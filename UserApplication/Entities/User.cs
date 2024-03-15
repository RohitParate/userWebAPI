using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserApplication.Entities
{
    public class User
    {
        [Key] // primary key annotation
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // new identity is created when city is created
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
