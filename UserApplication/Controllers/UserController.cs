using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserApplication.Entities;
using UserApplication.Models;
using UserApplication.Services;

namespace UserApplication.Controllers
{
    [Route("api/user")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <param name="name">Filter user by name</param>
        /// <param name="pageNumber"></param>
        /// <param name="pagesize"></param>
        /// <returns>All the users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(
            [FromQuery(Name = "userName")] string? name,
            int pageNumber = 1,
            int pagesize = 10
            )
        {
            if(pagesize > maxPageSize)
            {
                pagesize = maxPageSize;
            }
            var (users, paginationMetadata) = await _userRepository.GetUsersAsync(name,pageNumber, pagesize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
            // here we are mapping users data with UserDto 
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>A user with provided Id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));


        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var userData = _mapper.Map<Entities.User>(createUserDto);

            await _userRepository.CreateUserAsync(userData);

            await _userRepository.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="updateUserDto"></param>
        /// <returns></returns>
        [HttpPut("{userid}")]
        public async Task<ActionResult> UpdateUser(int userid, UpdateUserDto updateUserDto)
        {
            //if(!await _userRepository.UserExistAsync(userid))
            //{
            //    return NotFound();
            //}

            var user = await _userRepository.GetUserByIdAsync(userid);
            if(user == null)
            {
                return NotFound();
            }
            //_mapper.map(source, destination)
            //automapper will override the values in destination object with those from source object
            _mapper.Map(updateUserDto, user);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Updat user partially
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        [HttpPatch("{userid}")]

        public async Task<ActionResult> PartiallyUpdateUser(
            int userid, 
            JsonPatchDocument<UpdateUserDto> userData
            )
        {
            var user = await _userRepository.GetUserByIdAsync(userid);
            if (user == null)
            {
                return NotFound();
            }

            var  updateUserToPatch = _mapper.Map<UpdateUserDto>(user);

            userData.ApplyTo(updateUserToPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            //if we try to update just with path and operation and without value this code will get executed as this will try to validate the model
            if (!TryValidateModel(updateUserToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(updateUserToPatch, user);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpDelete("{userid}")]

        public async Task<ActionResult> DeleteUser(int userid)
        {
            if(!await _userRepository.UserExistAsync(userid))
            {
                return NotFound();
            }

            var user = await _userRepository.GetUserByIdAsync(userid);

            if(user == null)
            {
                return NotFound(userid);
            }

            _userRepository.DeleteUser(user);

            await _userRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
