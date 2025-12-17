using RoomBook.Core.Entities;
using RoomBook.Core.DTOs; 


namespace RoomBook.Core.Interfaces
{
    public interface IBookingRepository
    {
       
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime start, DateTime end, int minCapacity);

        Task<Booking> CreateBookingAsync(Booking booking);

        Task<Booking?> GetBookingByIdAsync(int bookingId);

        Task<bool> UpdateBookingAsync(Booking booking);

        Task<bool> DeleteBookingAsync(int bookingId);

        Task<IEnumerable<RoomUsageReportDto>> GetUsageReportAsync(DateTime startDate, DateTime endDate);
    }
}