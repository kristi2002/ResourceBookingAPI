﻿using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Dtos;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DataContext _context;

        public BookingRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync()
        {
            try
            {
                return await _context.Bookings.Include(b => b.Resource).Include(b => b.User).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching bookings.", ex);
            }
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                return await _context.Bookings.Include(b => b.Resource).Include(b => b.User).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching booking by ID.", ex);
            }
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error creating booking.", ex);
            }
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error updating booking.", ex);
            }
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error deleting booking.", ex);
            }
        }

        public async Task<bool> IsResourceBookedAsync(int resourceId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ResourceId == resourceId && (b.StartDate < endDate && b.EndDate > startDate));
        }

        public async Task<PaginatedResult<Resource>> GetAvailableResourcesAsync(DateTime startDate, DateTime endDate, int? resourceId, int page, int pageSize)
        {
            var bookedResourceIds = await _context.Bookings
                .Where(b => (b.StartDate < endDate && b.EndDate > startDate) &&
                            (!resourceId.HasValue || b.ResourceId == resourceId.Value))
                .Select(b => b.ResourceId)
                .Distinct()
                .ToListAsync();

            var availableResourcesQuery = _context.Resources
                .Where(r => !bookedResourceIds.Contains(r.ResourceId) &&
                            (!resourceId.HasValue || r.ResourceId == resourceId.Value));

            var totalResources = await availableResourcesQuery.CountAsync();

            var availableResources = await availableResourcesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedResult = new PaginatedResult<Resource>
            {
                TotalResults = totalResources,
                TotalPages = (int)Math.Ceiling(totalResources / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Results = availableResources
            };

            return paginatedResult;
        }
    }
}
