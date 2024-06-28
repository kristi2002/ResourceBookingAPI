using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int bookingId);
    }
}
