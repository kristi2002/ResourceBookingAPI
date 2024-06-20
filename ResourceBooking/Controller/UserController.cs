using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var usersToReturn = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Email = u.Email,
                Nome = u.Name,
                Cognome = u.LastName
            });
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var userToReturn = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Nome = user.Name,
                Cognome = user.LastName
            };

            return Ok(userToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserForCreationDto userForCreation)
        {
            var user = new User
            {
                Email = userForCreation.Email,
                Name = userForCreation.Nome,
                LastName = userForCreation.Cognome,
                Password = userForCreation.Password
            };

            await _userRepository.AddUserAsync(user);
            if (await _userRepository.SaveAsync())
                return CreatedAtRoute("GetUser", new { id = user.UserId }, user);

            return BadRequest("Error creating user");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForCreationDto userForUpdate)
        {
            var userFromRepo = await _userRepository.GetUserByIdAsync(id);
            if (userFromRepo == null)
                return NotFound();

            userFromRepo.Email = userForUpdate.Email;
            userFromRepo.Name = userForUpdate.Nome;
            userFromRepo.LastName = userForUpdate.Cognome;
            userFromRepo.Password = userForUpdate.Password;

            await _userRepository.UpdateUserAsync(userFromRepo);
            if (await _userRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error updating user");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userFromRepo = await _userRepository.GetUserByIdAsync(id);
            if (userFromRepo == null)
                return NotFound();

            await _userRepository.DeleteUserAsync(userFromRepo);
            if (await _userRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error deleting user");
        }
    }
}
