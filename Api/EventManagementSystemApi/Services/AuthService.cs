using EventManagementSystemApi.Models;
using EventManagementSystemApi.Models.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EventManagementSystemApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly DateTime Exparation = DateTime.UtcNow.AddDays(10);
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isUserExist = await _userManager.FindByEmailAsync(registerDto.Email);
            if (isUserExist != null)
            {
                throw new ValidationException("User with this email already exists");
            }
            var user = new User()
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(error => error.Description)));
            }

            return await GenerateTokenAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new ValidationException("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                throw new ValidationException("Invalid email or password");
            }
            return await GenerateTokenAsync(user);
        }

        public async Task<AuthResponseDto> GenerateTokenAsync(User user)
        {

            var jwtSecret = _configuration["JWT_SECRET"];

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FullName", user.FullName)
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = Exparation,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return await Task.FromResult(new AuthResponseDto()
            {
                Token = token,
                Expiration = Exparation
            });
        }

    }
}
