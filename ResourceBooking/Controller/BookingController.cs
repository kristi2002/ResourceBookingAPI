﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceBooking.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsAsync();
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);

                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Booking by ID")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(id);

                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                var bookingDto = _mapper.Map<BookingDto>(booking);

                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
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
                var isBooked = await _bookingRepository.IsResourceBookedAsync(bookingForCreationDto.ResourceId, bookingForCreationDto.DataInizio, bookingForCreationDto.DataFine);

                if (isBooked)
                {
                    return BadRequest("The resource is already booked during the specified time interval.");
                }

                var booking = _mapper.Map<Booking>(bookingForCreationDto);
                var createdBooking = await _bookingRepository.CreateBookingAsync(booking);
                var bookingDto = _mapper.Map<BookingDto>(createdBooking);

                return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.BookingId }, bookingDto);
            }
            catch (Exception ex)
            {
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
                var existingBooking = await _bookingRepository.GetBookingByIdAsync(bookingForUpdateDto.BookingId);

                if (existingBooking == null)
                {
                    return NotFound("Booking not found.");
                }

                // Check if the resource is available during the specified time interval for the update
                var isBooked = await _bookingRepository.IsResourceBookedAsync(bookingForUpdateDto.ResourceId, bookingForUpdateDto.DataInizio, bookingForUpdateDto.DataFine);

                if (isBooked && existingBooking.ResourceId != bookingForUpdateDto.ResourceId)
                {
                    return BadRequest("The resource is already booked during the specified time interval.");
                }

                var booking = _mapper.Map<Booking>(bookingForUpdateDto);
                var updatedBooking = await _bookingRepository.UpdateBookingAsync(booking);
                var bookingDto = _mapper.Map<BookingDto>(updatedBooking);

                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Booking")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var success = await _bookingRepository.DeleteBookingAsync(id);

                if (!success)
                {
                    return NotFound("Booking not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
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

                var availableResources = await _bookingRepository.GetAvailableResourcesAsync(searchDto.DataInizio, searchDto.DataFine, searchDto.CodiceRisorsa, searchDto.Page, searchDto.PageSize);

                var resourceDtos = _mapper.Map<PaginatedResult<ResourceDto>>(availableResources);

                return Ok(resourceDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
