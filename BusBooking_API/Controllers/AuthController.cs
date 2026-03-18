using BusBooking_API.DTOs;
using BusBooking_API.Model;
using BusBooking_API.Model.Database;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _config;

        private readonly IEmailService _emailService;

        private static Dictionary<string, TempUser> otpStorage = new();

        public AuthController(ApplicationDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            {
                return BadRequest("Email already registered");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            otpStorage[dto.Email] = new TempUser
            {
                Name = dto.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Otp = otp
            };

            await _emailService.SendEmailAsync(
                dto.Email,
                "Bus Booking OTP",
                $"Your OTP is {otp}."
            );

            return Ok("OTP sent to email.");
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            if (!otpStorage.ContainsKey(dto.Email))
            {
                return BadRequest("No OTP request found");
            }

            var tempUser = otpStorage[dto.Email];

            if (tempUser.Otp != dto.OtpCode)
            {
                return BadRequest("Invalid OTP");
            }

            var user = new User
            {
                Name = tempUser.Name,
                Email = dto.Email,
                PasswordHash = tempUser.PasswordHash,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            otpStorage.Remove(dto.Email);

            return Ok("User registered successfully!");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.Trim().ToLower());

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateToken(user);

            return Ok(new
            {
                token,
                email = user.Email,
                role = user.Role
            });
        }


        private string GenerateToken(User user)
        {
            var key = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("JWT Key is missing in configuration");
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
              new Claim(ClaimTypes.Name, user.Email),
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
