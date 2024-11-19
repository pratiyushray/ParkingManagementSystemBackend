namespace ParkingManagementSystem.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;
    using ParkingManagementSystem.Models;
    using ParkingManagementSystem.Data;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BCrypt.Net;
    using System.Text;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.IdentityModel.Tokens;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "User with this email already exists." });
            }

            // Hash the password (optional but recommended for production)
            user.Password = BCrypt.HashPassword(user.Password);


            // Add the user to the database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        // Optional: Additional endpoints can be added here
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Verify the password
            if (!BCrypt.Verify(login.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Generate JWT
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.Role
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User") // Default role to "User" if null
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
