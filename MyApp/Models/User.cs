using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class User
    {
        [Key] // Primärnyckel
        public int UsersId { get; set; }

        [Required] // Obligatoriskt fält
        [MaxLength(50)] // Maxlängd
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [EmailAddress] // Validering för e-post
        public string Email { get; set; }
    }
}
