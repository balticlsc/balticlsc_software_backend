using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Baltic.Security.Auth;
using Baltic.Security.Controllers.Models;
using Baltic.Security.Entities;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Organisation = Baltic.DataModel.Accounts.Organisation;

namespace Baltic.Security.Controllers
{
    public class UserExtensionModel
    {    
        public string FullName { get; set; }
        public string RoleInOrganisation { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
    }
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class LoginController : BalticController    
    {
        private readonly UserRegistryRepository _userRepository = new UserRegistryRepository();
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config, IUserSessionRoutine userSession) : base(userSession)
        {    
            _config = config;    
        }

        [Authorize] 
        [HttpGet("CheckAuthentication")]
        public ActionResult CheckAuthentication()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;
            var userName = User.Identity.Name;
            var roles = User.GetRoles();
            var sid = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sid)
                .Select(c => c.Value).SingleOrDefault();
            return new JsonResult(new { Success = true, isAuthenticated, userName, roles, sid });
        }

        [HttpPost("CreateUser")]
        [Authorize]
        public IActionResult CreateUser([FromBody] UserModel user)
        {
            var usr = _userRepository.CreateUser(user.Username, user.Password, user.EmailAddress??"", user.Status);

            return usr != null ? Ok(usr) : Error($"Can't create user: {user.Username}");
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel passwordModel)
        {
            if (User.GetRoles().Contains(PoliciesLookup.RequireAdmin))
            {
                if (_userRepository.ChangePassword(passwordModel.UserName, passwordModel.OldPassword, passwordModel.NewPassword))
                {
                    return Ok();                    
                }
            }
            else if (User.GetRoles().Contains(PoliciesLookup.RequireOrganisationAdmin)) // && user is from my organisation
            {
                if (_userRepository.ChangePassword(passwordModel.UserName, passwordModel.OldPassword, passwordModel.NewPassword))
                {
                    return Ok();                    
                }
            }
            else
            {
                if (_userRepository.ChangePassword(User.Identity.Name, passwordModel.OldPassword, passwordModel.NewPassword))
                {
                    return Ok();                    
                }
            }
            return Error($"You can't change password for user: {passwordModel.UserName}");
        }

        [HttpGet("AssignUserToOrganisation")]
        [Authorize]
        public IActionResult AssignUserToOrganisation(string userName, int organisationId)
        {
            var id = _userRepository.GetUserId(userName);
            if (id > 0)
            {
                _userRepository.AssignUserToOrganisation(id, organisationId);
                return Ok();
            }

            return Error("Can't assign user to organisation");
        }
        
        [HttpGet("AddUserRole")]
        [Authorize]
        public IActionResult AddUserRole(string userName, string role)
        {
            return Ok();            
        }

        [HttpGet("GetUsers")]
        [Authorize]
        public IActionResult GetUsers(bool activeOrAll = false)
        {
            var users = _userRepository.GetUserList(activeOrAll).ToList().Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Status
            }).ToList();
            
            return Ok(users);
        } 
        
        [HttpGet("GetOrganisations")]
        [Authorize]
        public IActionResult GetOrganisations()
        {
            var organisations = _userRepository.GetOrganisationList().ToList().Select(o => new
            {
                o.Id,
                o.Name,
                o.Details
            }).ToList();
            
            return Ok(organisations);
        } 
        
        
        [AllowAnonymous]    
        [HttpPost]    
        public IActionResult Login([FromBody]UserLoginModel loginUser)    
        {
            var user = AuthenticateUser(loginUser);

            if (user != null)
            {
                var sid = Guid.NewGuid().ToString("N");
                var jti = Guid.NewGuid().ToString("N");

                _userRepository.SaveSession(loginUser.UserName, sid, jti);

                var tokenString = GenerateJsonWebToken(user.UserName, sid, jti);
                return Ok(new {token = tokenString});
            }

            return Unauthorized(); 
        }

        [HttpGet("UpdateSession")]
        [Authorize]
        public IActionResult UpdateSession()
        {
            var jti = Guid.NewGuid().ToString("N");

            if (_userRepository.SaveSession(User.Identity.Name, Sid, jti))
            {
                var tokenString = GenerateJsonWebToken(User.Identity.Name, Sid, jti);
                return Ok(new {token = tokenString});                
            }

            return StatusCode(409);  // 409 Conflict - Indicates that the request could not be processed because of conflict in the current state of the resource, such as an edit conflict between multiple simultaneous updates.
        }
        
        [HttpGet("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            _userRepository.DeleteSession(Sid);
            
            return Ok();            
        }

        [HttpGet("SaveParam")]
        [Authorize]
        public IActionResult SaveParam(string paramName, string paramValue)
        {
            if (_userRepository.SaveSessionParam(Sid, paramName, paramValue))
            {
                return Ok();
            }
            return Error($"Can't save param: {paramName}");
        }
        
        [HttpGet("GetParam")]
        [Authorize]
        public IActionResult GetParam(string paramName)
        {
            var paramValue = _userRepository.GetSessionParam(Sid, paramName);

            if (paramValue != string.Empty)
            {
                return Ok(new {paramName = paramName, paramValue = paramValue});
            }
            return Error($"Can't read param: {paramName}");
        }
        
        [HttpGet("SavePreference")]
        [Authorize]
        public IActionResult SavePreference(string paramName, string paramValue)
        {
            if (_userRepository.SaveUserPref(User.Identity.Name, paramName, paramValue))
            {
                return Ok();
            }
            return Error($"Can't save preference: {paramName}");
        }
        
        [HttpGet("GetPreference")]
        [Authorize]
        public IActionResult GetPreference(string paramName)
        {
            var paramValue = _userRepository.GetUserPref(User.Identity.Name, paramName);

            if (paramValue != string.Empty)
            {
                return Ok(new {paramName = paramName, paramValue = paramValue});
            }
            return Error($"Can't read preference: {paramName}");
        }
        
        private string GenerateJsonWebToken(string userName, string sid, string jti)    
        {    
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Bearer:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);            

            var token = new JwtSecurityToken(_config["Bearer:Issuer"], _config["Bearer:Audience"], 
                claims: new []
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName,userName),
                    new Claim(JwtRegisteredClaimNames.Sub, userName),    
                    new Claim(JwtRegisteredClaimNames.Jti, jti), 
                    new Claim(JwtRegisteredClaimNames.Sid, sid)                      
                },
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);    
        }    
    
        private SystemUser AuthenticateUser(UserLoginModel loginUser)    
        {    
            return _userRepository.AuthUser(loginUser.UserName, loginUser.Password);
        }    
    }    
}