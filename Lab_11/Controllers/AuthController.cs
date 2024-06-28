using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using Lab_11.Helper;
using Lab_11.Models.AuthRequests;
using Lab_11.Models.Domain;
using Lab_11.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = Lab_11.Models.AuthRequests.LoginRequest;
using RegisterRequest = Lab_11.Models.AuthRequests.RegisterRequest;

namespace Lab_11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration, IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent(RegisterRequest model)
        {
            var user = await authService.GetUserByLogin(model.Login);
            if (user != null)
            {
                return Conflict("User with this login already exists");
            }
            
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);

            var newUser = new AppUser()
            {
                Login = model.Login,
                Password = hashedPasswordAndSalt.Item1,
                Salt = hashedPasswordAndSalt.Item2,
                RefreshToken = SecurityHelpers.GenerateRefreshToken(),
                RefreshTokenExp = DateTime.Now.AddDays(1)
            };

            await authService.AddUser(newUser);
            
            transaction.Complete();

            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await authService.GetUserByLogin(loginRequest.Login);
            
            if (user == null)
            {
                return Unauthorized();
            }

            string passwordHashFromDb = user.Password;
            string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(loginRequest.Password, user.Salt);

            if (passwordHashFromDb != curHashedPassword)
            {
                return Unauthorized();
            }
            
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Issuer"],
                claims: Array.Empty<Claim>(),
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            await authService.UpdateEmployeeRefreshToken(user, DateTime.Now.AddDays(1));
            
            transaction.Complete();

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = user.RefreshToken
            });
        }
        
        [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest refreshToken)
        {
            var user  = await authService.GetEmployeeByRefreshToken(refreshToken.RefreshToken);
            if (user == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            if (user.RefreshTokenExp < DateTime.Now)
            {
                throw new SecurityTokenException("Refresh token expired");
            }
            
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Issuer"],
                claims: Array.Empty<Claim>(),
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
            
            await authService.UpdateEmployeeRefreshToken(user, DateTime.Now.AddDays(1));
            
            transaction.Complete();

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                refreshToken = user.RefreshToken
            });
        }
    }
}
