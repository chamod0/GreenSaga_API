using Azure.Core;
using GreenSagaAPI.Context;
using GreenSagaAPI.Helpers;
using GreenSagaAPI.Models;
using GreenSagaAPI.Models.Dto;
using GreenSagaAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GreenSagaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        private projectService _projectService;

        public UserController()
        {
            _projectService = new projectService();

        }
        public UserController(AppDbContext appDbContext) {

            _authContext = appDbContext;
        }

        [HttpPost("authenticate")] 
        public async Task<IActionResult> Authenticate([FromBody]User userObj)
        {
            if (userObj == null)
                return BadRequest();

            var user = await _authContext.Users.FirstOrDefaultAsync(x=>x.UserName == userObj.UserName);

            if(user == null)
                return NotFound(new { Message = userObj.UserName +": User Not Found !"});

            if(!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message= "password is Incorrect"});
            }
 
            user.Token = CreateJWt(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken= newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            //check user name
            if (await CheckUserNameExistAsync(userObj.UserName))
                return BadRequest(new { Message = "Username Already Exist! " });
            //Check Email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist! " });

            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            userObj.Password = PasswordHasher.HashPasseord(userObj.Password);
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registerd!"
            });

        }
        private Task<bool> CheckUserNameExistAsync(string username)
          => _authContext.Users.AnyAsync(x => x.UserName == username);
        private Task<bool> CheckEmailExistAsync(string email)
         => _authContext.Users.AnyAsync(x => x.Email == email);

       
         private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if(!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[!, @,#,$,%,^,&,*,(,),_,+,=,{,[,},\\],|,\\,:,;,\",<,>,.,?,/]"))
                sb.Append("Password should be contain specaail chars" + Environment.NewLine);
            return sb.ToString();
        }
        private string CreateJWt(User user)
        {
            try { 
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.UserName}")

            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
            }catch(Exception ex)
            {
                throw new Exception();
            }

        }
        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var tokInUser = _authContext.Users
                .Any(x => x.UserName == refreshToken);
            if (tokInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }
        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var tokenValidationParameters = new TokenValidationParameters
            {

                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
            };
            var TokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = TokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
                throw new SecurityTokenException("This is Invalid Token");
            return principal;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUser()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refesh(TokenApiDto tokenApiDto)
        {
            if(tokenApiDto is null)
                return BadRequest("Invalide Client Request");
            string accessToken =tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal =GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user=await _authContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJWt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken= newRefreshToken,
                        
            });
        }


        [HttpGet("project/{id?}")]
        public IActionResult project(int? id)
        {
            var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (id is null)
            {
                return BadRequest("can't pass null value");
            }
                
            else if(id == 0){
                return Ok(_projectService.GetCultivationProjects());
            }
            else {
                var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
                return Ok(project);
            }    
              
          
        }

        

    }
}
