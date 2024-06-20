using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using System.Collections.Generic;
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
            return await _context.Bookings.Include(b => b.Resource).Include(b => b.User).ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.Include(b => b.Resource).Include(b => b.User).SingleOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
        }

        public async Task DeleteBookingAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync(int resourceId)
        {
            return await _context.Bookings.Where(b => b.ResourceId == resourceId).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
