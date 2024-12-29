using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistance.Repositories;

namespace RoomBookingApp.Persistance.Tests
{
    public class RoomBookingServiceTest : IDisposable
    {
        private readonly RoomBookingAppDbContext _context;

        public RoomBookingServiceTest()
        {
            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "RoomBookingApp")
                .Options;

            _context = new RoomBookingAppDbContext(dbOptions);
        }

        public void Dispose()
        {
            _context.Rooms.RemoveRange(_context.Rooms);
            _context.RoomBookings.RemoveRange(_context.RoomBookings);
            _context.SaveChanges();
            _context.Dispose();
        }

        [Fact]
        public void Should_Return_Available_Rooms()
        {
            // Arrange
            var date = new DateTime(2024, 12, 30);

            _context.Rooms.Add(new Domain.Room { Id = 1, Name = "Room1" });
            _context.Rooms.Add(new Domain.Room { Id = 2, Name = "Room2" });
            _context.Rooms.Add(new Domain.Room { Id = 3, Name = "Room3" });

            _context.RoomBookings.Add(new RoomBooking { RoomId = 1, Date = date, FullName = "John Doe", Email = "john.doe@example.com" });
            _context.RoomBookings.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1), FullName = "Jane Doe", Email = "jane.doe@example.com" });

            _context.SaveChanges();

            var roomBookingService = new RoomBookingService(_context);

            // Act
            var availableRooms = roomBookingService.GetAvailableRooms(date);

            // Assert
            Assert.Equal(2, availableRooms.Count);
            Assert.Contains(availableRooms, r => r.Id == 2);
            Assert.Contains(availableRooms, r => r.Id == 3);
            Assert.DoesNotContain(availableRooms, r => r.Id == 1);
        }

        [Fact]
        public void Should_Save_Room_Booking()
        {
            var roomBooking = new RoomBooking
            {
                RoomId = 1,
                Date = new DateTime(2024, 12, 29),
                FullName = "John Doe",
                Email = "john.doe@example.com"
            };

            var roomBookingService = new RoomBookingService(_context);
            roomBookingService.Save(roomBooking);

            var bookings = _context.RoomBookings.ToList();

            var booking = Assert.Single(bookings);

            Assert.Equal(roomBooking.Date, booking.Date);
            Assert.Equal(roomBooking.RoomId, booking.RoomId);
        }
    }
}
