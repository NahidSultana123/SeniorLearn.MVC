using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SeniorLearnV3.Data.Identity;


namespace SeniorLearnV3.Controllers;

[Route("api/v1/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserManager _userManager; //need to verify user's credentials


    public TokenController(IConfiguration config, UserManager userManager)
    {
        _config = config;
        _userManager = userManager;
        
    }


    // Handles user login, validates credentials, generates a JWT token with claims (including user roles)
    // returns the token as a response if successful.
    // If the credentials are invalid, it returns a "BadRequest" error.

    [HttpPost]
    public async Task<IActionResult> Login(LoginApiModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);

            if (user!= null && await _userManager.CheckPasswordAsync(user, model.Password)) // for valid email & password
            {
                //Generating list of claims
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),//need to install Microsoft.AspNetCore.Authentication.JwtBearer package for this
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                         DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // DateTimeOffset records time zone info whereas DateTime does not
                
                };

                var roles = (await _userManager.GetRolesAsync(user))
                    .Select(role=> new Claim(ClaimTypes.Role, role));

                claims.AddRange(roles); // add role claims(roles) to the list of claims

                //create key

                SymmetricSecurityKey Key = new SymmetricSecurityKey(Convert.FromBase64String(_config["Jwt:Key"]));
                 
                SigningCredentials sign = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                
                JwtSecurityToken token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: sign);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));  //writing the token to the response
            }


        }
        return BadRequest("Invalid Credentials");
    }

    public class LoginApiModel
    {
        [Required]
        public string? Email { get; set; } 

        [Required]
        public string? Password { get; set; } 
    }
}