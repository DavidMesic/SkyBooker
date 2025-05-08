using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _hasher;

        public UserService(AuthDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _hasher = new PasswordHasher<User>();
        }

        public User? Register(string username, string email, string password)
        {
            if (_db.Users.Any(u => u.Username == username)) return null;
            var user = new User { Username = username, Email = email };
            user.PasswordHash = _hasher.HashPassword(user, password);
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public string? Authenticate(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null) return null;
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed) return null;

            // Token erstellen
            var jwt = _config.GetSection("JwtSettings");
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}