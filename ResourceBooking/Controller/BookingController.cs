using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Dto;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        [SwaggerOperation(Summary = "List of Users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        Nome = u.Name,
                        Cognome = u.LastName
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get User by ID")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Nome = user.Name,
                    Cognome = user.LastName
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/users
        [HttpPost]
        [SwaggerOperation(Summary = "Add User")]
        public async Task<ActionResult<UserDto>> AddUser(UserForCreationDto userForCreationDto)
        {
            try
            {
                var user = new User
                {
                    Email = userForCreationDto.Email,
                    Name = userForCreationDto.Nome,
                    LastName = userForCreationDto.Cognome,
                    Password = userForCreationDto.Password // Note: Ensure to hash the password before saving in a real application
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Nome = user.Name,
                    Cognome = user.LastName
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/users
        [HttpPut]
        [SwaggerOperation(Summary = "Update User")]
        public async Task<ActionResult<UserDto>> UpdateUser(UserForUpdateDto userForUpdateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userForUpdateDto.UserId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.Email = userForUpdateDto.Email;
                user.Name = userForUpdateDto.Name;
                user.LastName = userForUpdateDto.LastName;
                user.Password = userForUpdateDto.Password; // Note: Ensure to hash the password before saving in a real application

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Nome = user.Name,
                    Cognome = user.LastName
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete User")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
