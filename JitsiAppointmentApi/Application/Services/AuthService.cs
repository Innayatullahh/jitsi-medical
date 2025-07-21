using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JitsiAppointmentApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Simple hardcoded check (for demo only!)
            // In a real application, this would validate against a user database
            if (request.Username == "demo" && request.Password == "password")
            {
                var token = GenerateJwtToken(request.Username);
                return Task.FromResult(new LoginResponse
                {
                    Status = "success",
                    Message = "Login successful",
                    Token = token
                });
            }

            return Task.FromResult(new LoginResponse
            {
                Status = "error",
                Message = "Invalid credentials"
            });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyValue = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyValue));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 