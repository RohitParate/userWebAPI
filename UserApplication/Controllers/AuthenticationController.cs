using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApplication.Entities;
using UserApplication.Models;
using UserApplication.Services;

namespace UserApplication.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;



        public AuthenticationController(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ??
               throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        public class AuthenticationRequestBody
        {
            public string UserName { get; set; }

            public string Password { get; set; }
        }

        //private class UserInfo
        //{
        //    public int Id { get; set; }

        //    public string Name { get; set; }    
        //    public string Email { get; set; }

        //    public UserInfo(int id, string name, string email)
        //    {
        //        Id = id;
        //        Name = name;
        //        Email = email;
        //    }
        //} 

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            //1 : validate the username and password
            var user = await ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(
              Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.Id.ToString()));
            claimsForToken.Add(new Claim("given_name", user.Name));
            claimsForToken.Add(new Claim("email", user.Email));
           // claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
               _configuration["Authentication:Issuer"],
               _configuration["Authentication:Audience"],
               claimsForToken,
               DateTime.UtcNow,
               DateTime.UtcNow.AddHours(1),
               signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private async Task<User> ValidateUserCredentials(string userName, string password)
        {
            var user = await _userRepository.ValidateUserAsync(userName, password);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return user;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var userData = _mapper.Map<Entities.User>(createUserDto);

            await _userRepository.CreateUserAsync(userData);

            await _userRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
