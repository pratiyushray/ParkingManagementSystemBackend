using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    [Table("Users")] // Explicitly map to the Users table in MySQL
    public class User
    {
        [Key] // Specifies this property as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment in the database
        public int UserId { get; set; }

        [Required] // Makes this field mandatory
        [StringLength(50)] // Limits the length of the username
        public string Username { get; set; }

        [Required]
        [StringLength(255)] // Long enough for hashed passwords
        public string Password { get; set; } // Store hashed passwords, not plain text

        [Required]
        [EmailAddress] // Ensures valid email format
        [StringLength(100)] // Reasonable length for an email address
        public string Email { get; set; }

        [Phone] // Ensures valid phone format
        [StringLength(15)] // Example: "+1234567890" fits within 15 characters
        public string? Phone { get; set; }

        [StringLength(20)] // Example: "Admin", "User"
        public string? Role { get; set; } = "User"; // Default role is User
    }
}
