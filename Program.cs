using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ParkingManagementSystem.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register the database context with MySQL configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32)) // Specify MySQL version
    )
);

builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // No issuer validation
            ValidateAudience = false, // No audience validation
            ValidateLifetime = true, // Ensure the token is valid
            ValidateIssuerSigningKey = true, // Validate signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8E+W4s47N8zWzQ3I2Uq7e+EnRj7GpHLOQ6X9y1cnwwk=")) // Secret key for signing
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
