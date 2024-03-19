using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserApplication.Services;
using UserApplication.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UserApplication.Entities;

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
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public class AuthenticationRequestBody
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserWithTokenDto>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            var user = await ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("given_name", user.Name),
                new Claim("email", user.Email)
            };

            var jwtSecurityToken = new JwtSecurityToken(
               _configuration["Authentication:Issuer"],
               _configuration["Authentication:Audience"],
               claimsForToken,
               DateTime.UtcNow,
               DateTime.UtcNow.AddHours(1),
               signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var userDto = _mapper.Map<UserDto>(user);

            return new UserWithTokenDto { Token = tokenToReturn, User = userDto };
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
