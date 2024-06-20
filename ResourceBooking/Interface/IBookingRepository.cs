using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync(int resourceId);
        Task<bool> SaveAsync();
    }
}
