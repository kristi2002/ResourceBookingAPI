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
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IUserRepository _userRepository;

        public BookingController(IBookingRepository bookingRepository, IResourceRepository resourceRepository, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _resourceRepository = resourceRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingRepository.GetBookingsAsync();
            var bookingsToReturn = bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                ResourceId = b.ResourceId,
                UserId = b.UserId,
                DataInizio = b.StartDate,
                DataFine = b.EndDate
            });
            return Ok(bookingsToReturn);
        }

        [HttpGet("{id}", Name = "GetBooking")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            var bookingToReturn = new BookingDto
            {
                BookingId = booking.BookingId,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                DataInizio = booking.StartDate,
                DataFine = booking.EndDate
            };

            return Ok(bookingToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingForCreationDto bookingForCreation)
        {
            if (!await _resourceRepository.ResourceExistsAsync(bookingForCreation.ResourceId) ||
                !await _userRepository.UserExistsAsync(bookingForCreation.UserId))
            {
                return BadRequest("Invalid ResourceId or UserId");
            }

            var booking = new Booking
            {
                ResourceId = bookingForCreation.ResourceId,
                UserId = bookingForCreation.UserId,
                StartDate = bookingForCreation.DataInizio,
                EndDate = bookingForCreation.DataFine
            };

            await _bookingRepository.AddBookingAsync(booking);
            if (await _bookingRepository.SaveAsync())
                return CreatedAtRoute("GetBooking", new { id = booking.BookingId }, booking);

            return BadRequest("Error creating booking");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, BookingForCreationDto bookingForUpdate)
        {
            var bookingFromRepo = await _bookingRepository.GetBookingByIdAsync(id);
            if (bookingFromRepo == null)
                return NotFound();

            bookingFromRepo.ResourceId = bookingForUpdate.ResourceId;
            bookingFromRepo.UserId = bookingForUpdate.UserId;
            bookingFromRepo.StartDate = bookingForUpdate.DataInizio;
            bookingFromRepo.EndDate = bookingForUpdate.DataFine;

            await _bookingRepository.UpdateBookingAsync(bookingFromRepo);
            if (await _bookingRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error updating booking");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var bookingFromRepo = await _bookingRepository.GetBookingByIdAsync(id);
            if (bookingFromRepo == null)
                return NotFound();

            await _bookingRepository.DeleteBookingAsync(bookingFromRepo);
            if (await _bookingRepository.SaveAsync())
                return NoContent();

            return BadRequest("Error deleting booking");
        }
    }
}
