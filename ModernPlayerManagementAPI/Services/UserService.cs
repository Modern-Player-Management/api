using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Models.Repository;

namespace ModernPlayerManagementAPI.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository { get; set; }
        private readonly AppSettings _appSettings;
        private readonly IFilesService _filesService;
        private readonly IEmailValidator _emailValidator;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettings, IFilesService filesService, IEmailValidator emailValidator, IMapper mapper, IUserRepository userRepository)
        {
            _appSettings = appSettings?.Value;
            _filesService = filesService;
            _emailValidator = emailValidator;
            _mapper = mapper;
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
            var passwordValidity = ValidatePassword(password);
            var isPasswordValid = passwordValidity == "Valid";
            if (!isPasswordValid)
            {
                throw new ArgumentException(passwordValidity);
            }
            
            User user = new User()
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email
            };

            this._userRepository.Insert(user);
            
            return user;
        }

        public void Update(UpdateUserDTO dto, Guid userId)
        {
            var user = this._userRepository.GetById(userId);
            if (user.Image != dto.Image && dto.Image != null)
            {
                if (user.Image != null)
                {
                    this._filesService.Delete(Guid.Parse(user.Image.Split("/").Last()));
                }
                
                user.Image = dto.Image;
            }

            if (dto.Username != user.Username && dto.Username != null && this.IsUniqueUser(dto.Username))
            {
                user.Username = dto.Username;
            }

            if (dto.Email != user.Email && dto.Email != null && this._emailValidator.IsValidEmail(dto.Email))
            {
                user.Email = dto.Email;
            }

            if (dto.Password != user.Password && dto.Password != null)
            {
                string passwordValidity = this.ValidatePassword(dto.Password);
                if (passwordValidity == "Valid")
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }
                else
                {
                    throw new ArgumentException(passwordValidity);
                }
            }
            this._userRepository.Update(user);
        }

        public ICollection<UserDTO> SearchUser(string search)
        {
            return this._userRepository.findUsersByUsernameContains(search).Select(user => this._mapper.Map<UserDTO>(user)).ToList();
        }

        public UserDTO GetFromUsername(string username)
        {
            return this._mapper.Map<UserDTO>(this._userRepository.GetUserByUsername(username));
        }

        private string ValidatePassword(string password)
        {
            if (!new Regex(@"(?=.*[a-z])").IsMatch(password))
            {
                return "Invalid Password : At least one lowercase letter is required";
            }
            if (!new Regex(@"(?=.*[A-Z])").IsMatch(password))
            {
                return "Invalid Password : At least one uppercase letter is required";
            }
            if (!new Regex(@"(?=.*\d)").IsMatch(password))
            {
                return "Invalid Password : At least one number is required";
            }
            if (!new Regex(@"^.{8,32}$").IsMatch(password))
            {
                return "Invalid Password : Password length : 8-32 characters";
            }

            return "Valid";
        }
    }
}