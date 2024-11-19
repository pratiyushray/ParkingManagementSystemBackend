using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<ParkingSpotsModel> ParkingSpots { get; set; }

    }
}
