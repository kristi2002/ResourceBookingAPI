using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DataContext _context;

        public BookingController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings.Select(b => new BookingDto
                {
                    BookingId = b.BookingId,
                    ResourceId = b.ResourceId,
                    UserId = b.UserId,
                    DataInizio = b.StartDate,
                    DataFine = b.EndDate
                }).ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Booking by ID")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                var bookingDto = new BookingDto
                {
                    BookingId = booking.BookingId,
                    ResourceId = booking.ResourceId,
                    UserId = booking.UserId,
                    DataInizio = booking.StartDate,
                    DataFine = booking.EndDate
                };

                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add Booking")]
        public async Task<ActionResult<BookingDto>> AddBooking(BookingForCreationDto bookingForCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if the resource is available during the specified time interval
                var overlappingBooking = await _context.Bookings
                    .AnyAsync(b => b.ResourceId == bookingForCreationDto.ResourceId &&
                                   ((b.StartDate < bookingForCreationDto.DataFine && b.EndDate > bookingForCreationDto.DataInizio)));

                if (overlappingBooking)
                {
                    return BadRequest("The resource is already booked during the specified time interval.");
                }

                var booking = new Booking
                {
                    ResourceId = bookingForCreationDto.ResourceId,
                    UserId = bookingForCreationDto.UserId,
                    StartDate = bookingForCreationDto.DataInizio,
                    EndDate = bookingForCreationDto.DataFine
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var bookingDto = new BookingDto
                {
                    BookingId = booking.BookingId,
                    ResourceId = booking.ResourceId,
                    UserId = booking.UserId,
                    DataInizio = booking.StartDate,
                    DataFine = booking.EndDate
                };

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, bookingDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update Booking")]
        public async Task<ActionResult<BookingDto>> UpdateBooking(BookingDto bookingForUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var booking = await _context.Bookings.FindAsync(bookingForUpdateDto.BookingId);

                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                // Check if the resource is available during the specified time interval for the update
                var overlappingBooking = await _context.Bookings
                    .AnyAsync(b => b.ResourceId == bookingForUpdateDto.ResourceId &&
                                   b.BookingId != bookingForUpdateDto.BookingId &&
                                   ((b.StartDate < bookingForUpdateDto.DataFine && b.EndDate > bookingForUpdateDto.DataInizio)));

                if (overlappingBooking)
                {
                    return BadRequest("The resource is already booked during the specified time interval.");
                }

                booking.ResourceId = bookingForUpdateDto.ResourceId;
                booking.UserId = bookingForUpdateDto.UserId;
                booking.StartDate = bookingForUpdateDto.DataInizio;
                booking.EndDate = bookingForUpdateDto.DataFine;

                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                var bookingDto = new BookingDto
                {
                    BookingId = booking.BookingId,
                    ResourceId = booking.ResourceId,
                    UserId = booking.UserId,
                    DataInizio = booking.StartDate,
                    DataFine = booking.EndDate
                };

                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Booking")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("availability")]
        [SwaggerOperation(Summary = "Search Availability with Pagination")]
        public async Task<ActionResult<PaginatedResult<ResourceDto>>> SearchAvailability([FromQuery] AvailabilitySearchDto searchDto)
        {
            try
            {
                // Validate date range
                if (searchDto.DataInizio >= searchDto.DataFine)
                {
                    return BadRequest("DataInizio must be earlier than DataFine.");
                }

                // Get booked resource IDs within the specified time interval
                var bookedResourceIds = await _context.Bookings
                    .Where(b => (b.StartDate < searchDto.DataFine && b.EndDate > searchDto.DataInizio) &&
                                (!searchDto.CodiceRisorsa.HasValue || b.ResourceId == searchDto.CodiceRisorsa.Value))
                    .Select(b => b.ResourceId)
                    .Distinct()
                    .ToListAsync();

                // Get available resources that are not in the bookedResourceIds list
                var availableResources = await _context.Resources
                    .Where(r => !bookedResourceIds.Contains(r.ResourceId) &&
                                (!searchDto.CodiceRisorsa.HasValue || r.ResourceId == searchDto.CodiceRisorsa.Value))
                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .Select(r => new ResourceDto
                    {
                        ResourceId = r.ResourceId,
                        Name = r.Name,
                        ResourceTypeName = r.ResourceType.TypeName
                    })
                    .ToListAsync();

                // Get total count of available resources
                var totalResources = await _context.Resources
                    .Where(r => !bookedResourceIds.Contains(r.ResourceId) &&
                                (!searchDto.CodiceRisorsa.HasValue || r.ResourceId == searchDto.CodiceRisorsa.Value))
                    .CountAsync();

                var paginatedResult = new PaginatedResult<ResourceDto>
                {
                    TotalResults = totalResources,
                    TotalPages = (int)Math.Ceiling(totalResources / (double)searchDto.PageSize),
                    CurrentPage = searchDto.Page,
                    PageSize = searchDto.PageSize,
                    Results = availableResources
                };

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
