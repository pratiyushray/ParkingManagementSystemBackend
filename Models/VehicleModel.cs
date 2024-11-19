using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace ParkingManagementSystem.Models
{
    [Table("Vehicles")] // Explicitly map to the Vehicles table in MySQL
    public class Vehicle
    {
        [Key] // Specifies this property as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment in the database
        public int VehicleId { get; set; }

        [Required] // Makes this field mandatory
        [StringLength(100)] // Limits the length of the vehicle name
        public string VehicleName { get; set; }

        [Required]
        [StringLength(20)] // Limits the length of the vehicle number (e.g., "ABC-1234")
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(20)] // Limits the length of the vehicle type (e.g., "Electric")
        public string VehicleType { get; set; } // Valid values: "CNG", "Petrol", "Diesel", "Electric"

        [Required]
        [ForeignKey("User")] // Sets up a foreign key relationship with the User table
        public int? UserId { get; set; }

        
        // Navigation property to link with the User
        [JsonIgnore]
        public User? User { get; set; }
    }
}
