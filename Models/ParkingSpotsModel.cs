using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class ParkingSpotsModel
    {
        [Key] // Specifies the primary key for the table
        public int SpotId { get; set; }  // Unique identifier for the parking spot

        [Required] // Ensures the field cannot be null
        public bool IsOccupied { get; set; }  // Whether the spot is occupied or not

        public DateTime? StartTime { get; set; }  // Timestamp when the vehicle occupies the spot

        [Required] // VehicleType must be provided
        [MaxLength(20)] // Limit the length of the string to 20 characters
        public string? VehicleType { get; set; }  // Type of vehicle (CNG, Petrol, Diesel, Electric)

        [Column(TypeName = "decimal(18,2)")] // Defines precision and scale for decimal values
        public decimal AmountCharged { get; set; }  // The amount charged based on the time and vehicle type

        [MaxLength(50)] // Limit the length of the string to 50 characters
        public string? VehicleOwner { get; set; }  // Owner's username (linked to User model)

        public int VehicleId { get; set; }

        // Static user wallet for testing purposes (not mapped to the database)
        private static decimal userWallet = 200;  // Default user wallet amount

        // Method to calculate the amount based on the vehicle type and parked duration
        public void CalculateAmount()
        {
            if (IsOccupied && StartTime.HasValue && VehicleType != "Electric")
            {
                TimeSpan parkedDuration = DateTime.Now - StartTime.Value;

                decimal costPerHour = VehicleType switch
                {
                    "CNG" => 20,
                    "Petrol" => 30,
                    "Diesel" => 40,
                    _ => 0
                };

                // Calculate the amount based on the parked duration (in hours)
                AmountCharged = Math.Ceiling((decimal)parkedDuration.TotalHours) * costPerHour;

                // Deduct the amount from the user wallet (this would be linked to the VehicleOwner)
                if (userWallet >= AmountCharged)
                {
                    userWallet -= AmountCharged;
                }
                else
                {
                    // Handle case if the user doesn't have enough funds in the wallet
                    AmountCharged = userWallet;
                    userWallet = 0;
                }
            }
            else
            {
                // If the vehicle is electric or the spot is not occupied, no charge
                AmountCharged = 0;
            }
        }

        // Static method to simulate the user wallet balance check (not part of the database)
        public static decimal GetUserWalletBalance()
        {
            return userWallet;
        }
    }
}
