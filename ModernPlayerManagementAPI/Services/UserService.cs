using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.Repository;

namespace ModernPlayerManagementAPI.Services
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository { get; set; }
        private readonly AppSettings _appSettings;

        public UserService(IRepository<User> userRepository,IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
        }

        public bool IsUniqueUser(string username)
        {
            var user = (from u in this._userRepository.GetAll()
                where u.Username == username
                select u).FirstOrDefault();

            return user == null;
        }

        public User Authenticate(string username, string password)
        {
            var user = (from u in this._userRepository.GetAll()
                where u.Username == username
                select u).FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        public User Register(string username, string email, string password)
        {
            User user = new User()
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email
            };

            this._userRepository.Insert(user);
            
            return user;
        }
    }
}