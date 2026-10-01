using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PBDASHBOARD.Configuration;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;
using PBDASHBOARD.Services;
using PBDASHBOARD.User;
using PBDASHBOARD.User.Requests;
using PBDASHBOARD.User.Responses;

namespace PBDASHBOARD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthMangementController : ControllerBase

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<CurrentReGroupMember> _userManager;
        private readonly JwtConfig _jwtConfig;


        public AuthMangementController(UserManager<CurrentReGroupMember> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor, ApplicationDbContext dbContext)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Authorize(Roles = "SuperUser")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationForm user)
        {
            var exisngUser = await _userManager.FindByEmailAsync(user.Email);
            if (!ModelState.IsValid) 
                return Ok();
            if (exisngUser != null)
            {
                return BadRequest(new AuthResult.AuthResult()
                {
                    Error = new List<string>() {"Bad Request "},
                    Success = false
                });
            }

            var newUser = new CurrentReGroupMember()
            {
                Role = UserRole.User, UserName = user.Email, Email = user.Email,
            };
            var isCreated = await _userManager.CreateAsync(newUser, user.Password);
            if (isCreated.Succeeded)
            {
                var jwtToken = GenerateJwtToken(newUser);
                var gr = new ResearchGroup()
                {
                    GroupName = user.GroupName, GroupUrl = "",
                };
                await _dbContext.ResearchGroups.AddRangeAsync(gr);
                await _dbContext.SaveChangesAsync();
                var rg = new ResearchGroupCurrentMembers()
                {
                    _member = newUser, ResearchGroup = gr, RoleStatus = true, MemberRole = user.Role
                };
                await _dbContext.CurrentGroupMembers.AddAsync(rg);
                await _dbContext.SaveChangesAsync();
                return Ok(new RegistrationResponse()
                {
                    Success = true,
                    Token = jwtToken
                });
            }
            else
            {
                return BadRequest(new AuthResult.AuthResult()
                {
                    Error = isCreated.Errors.Select(x => x.Description).ToList(),

                    Success = false
                });
            }
        }

        private string GenerateJwtToken(CurrentReGroupMember user)
        {
            var id = "";
            var exisngUser = _userManager.FindByEmailAsync(user.Email);
            if (exisngUser != null)
            {
                var userId = _userManager.GetUserIdAsync(exisngUser.Result);
                id = userId.Result;
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    
                }),
                
                Expires = DateTime.UtcNow.AddHours(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            tokenDescription.Subject.AddClaim(new Claim(ClaimTypes.Role,user.Role));

            var token = jwtHandler.CreateToken(tokenDescription);
            var jwtToken = jwtHandler.WriteToken(token);
            return jwtToken;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginForm user)
        {
            if (ModelState.IsValid)
            {
                var Id = "";
                var exisngUser = await _userManager.FindByEmailAsync(user.Email);

                if (exisngUser != null)
                {
                    var userId = await _userManager.GetUserIdAsync(exisngUser);
                    Id = userId;
                }


                if (exisngUser == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Error = new List<string>()
                        {
                            "Invalid Login"
                        },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(exisngUser, user.Password);
                if (!isCorrect)
                { return BadRequest(new RegistrationResponse() {
                        Error = new List<string>()
                        {"Invalid Login"},
                        Success = false});
                }

                var jwtHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", Id),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var roles = exisngUser.Role;

                tokenDescription.Subject.AddClaim(new Claim(ClaimTypes.Role, roles));

                var token = jwtHandler.CreateToken(tokenDescription);
                var jwtToken = jwtHandler.WriteToken(token);
                return Ok(new RegistrationResponse()
                {
                    Success = true,
                    Token = jwtToken
                });
                

            }


            return BadRequest(new RegistrationResponse()
            {
                Error = new List<string>()
                {
                    "Invalid payLoad"
                },
                Success = false
            });
        }

        [Authorize(Roles = UserRole.SuperUser)]
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromBody] EditForm user)
        { var exisngUser = _userManager.FindByEmailAsync(user.Email);
            var role = exisngUser.Result.Role;
            var password = exisngUser.Result.PasswordHash;
            var delet = Delete(user);
            var newUser = new CurrentReGroupMember()
            {
                Role = UserRole.User, UserName = user.Email, Email = user.Email,
            };
            var isCreated = await _userManager.CreateAsync(newUser, password);
            if (isCreated.Succeeded)
            {
                var jwtToken = GenerateJwtToken(newUser);
                var gr = new ResearchGroup()
                {
                    GroupName = user.ToGroup, GroupUrl = "",
                };
                await _dbContext.ResearchGroups.AddRangeAsync(gr);
                await _dbContext.SaveChangesAsync();
                var rg = new ResearchGroupCurrentMembers()
                {
                    _member = newUser, ResearchGroup = gr, RoleStatus = true, MemberRole = role
                };
                await _dbContext.CurrentGroupMembers.AddAsync(rg);
                await _dbContext.SaveChangesAsync();
                return Ok(new RegistrationResponse()
                {
                    Success = true,
                    Token = jwtToken
                });
            }

            return BadRequest(new AuthResult.AuthResult()
            {
                Error = isCreated.Errors.Select(x => x.Description).ToList(),

                Success = false
            });
        }

        [Authorize(Roles = UserRole.SuperUser)]
        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] EditForm user)
        {
            int id = 0;
            var exisngUser = _userManager.FindByEmailAsync(user.Email);
            if (exisngUser.Result != null)
            {
                var userId = _userManager.GetUserIdAsync(exisngUser.Result);
                id = exisngUser.Id;
                if (exisngUser.Result.Role == UserRole.SuperUser)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Error = new List<string>()
                        {
                            "You can delete Admin"
                        },
                        Success = false
                    });
                }
                var deltUser = _dbContext.Users.Find(id);
                try
                {
                    _dbContext.Users.Remove(exisngUser.Result);
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            


            return BadRequest(new RegistrationResponse()
            {
                Error = new List<string>()
                {
                    "The user does not exist"
                },
                Success = false
            });
        }

        [AllowAnonymous]
        [HttpGet("NameOfGroups")]
        public List<string> Names()
        {
            var groupNames = (from lists in _dbContext.ResearchGroups select lists.GroupName).ToList();
            Console.WriteLine("");

            return groupNames;
        }
    }
}