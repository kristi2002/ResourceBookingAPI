using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using ResourceBooking.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public UsersController(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="userForCreationDto">User data transfer object to create a new user</param>
        /// <returns>Created user object</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Add new User")]
        public async Task<ActionResult<User>> CreateUser(UserForCreationDto userForCreationDto)
        {
            // Validate the incoming user model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email already exists
            var existingUser = await _userRepository.GetUsersAsync();
            if (existingUser.Any(u => u.Email == userForCreationDto.Email))
            {
                return Conflict("Email already exists.");
            }

            // Map UserForCreationDto to User model
            var user = new User
            {
                Email = userForCreationDto.Email,
                Name = userForCreationDto.Nome,
                LastName = userForCreationDto.Cognome,
                Password = userForCreationDto.Password
            };

            // Create the new user
            var createdUser = await _userRepository.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, createdUser);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User object</returns>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get User by Id")]
        public async Task<ActionResult<User>> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Authenticate a user and get a JWT token.
        /// </summary>
        /// <param name="loginDto">Login data transfer object</param>
        /// <returns>JWT token</returns>
        [HttpPost("authenticate")]
        [SwaggerOperation(Summary = "Authenticates User")]
        public async Task<ActionResult<string>> Authenticate(LoginDto loginDto)
        {
            var user = await _userRepository.AuthenticateUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(token);
        }

        /// <summary>
        /// Delete a user by ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Delete User by Id")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(user);
            return NoContent();
        }
    }
}
