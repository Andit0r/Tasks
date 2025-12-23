using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoesMarketDbLibrary.Contexts;
using ShoesMarketDbLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoesMarketDbLibrary.Services
{
    public class AuthService(ShoesMarketContext context)
    {
        private readonly ShoesMarketContext _context = context;
        public static readonly string SecretKey = "12345678123456781234567812345678";

        // Генерация JWT токена
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Данные пользователя, которые будут добавлены в JWT-токен
            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.Role, user.Role.Name)
            };

            // Создание токена
            var token = new JwtSecurityToken(
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Аутентификация
        public string LoginUser(string login, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user is null)
                return null;

            return GenerateToken(user);
        }
    }
}
