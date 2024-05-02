using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
        public class CityInfoUser
        {
            public CityInfoUser(int userId, string userName, string firstName, string laseName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LaseName = laseName;
                City = city;
            }
            public int  UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LaseName { get; set; }
            public string City { get; set; }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            var user = ValidateCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user is null)
                return Unauthorized();

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new List<Claim>()
            {
                new Claim("sub", user.UserId.ToString()) ,
                new Claim("given_name", user.FirstName) ,
                new Claim("family_name", user.LaseName) ,
                new Claim("city", user.City)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audiance"],
                userClaims,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                signingCredentials
            );

            var tokenToRetrun = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToRetrun);
        }

        private CityInfoUser  ValidateCredentials(string? userName, string? password)
        {
            // i assume the credentials are valid

            return new CityInfoUser(
                1,
                userName ?? "",
                "Kevind",
                "Dockx",
                "Antwerp");
        }
    }
}
