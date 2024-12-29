using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Persistance.Repositories
{
    public class RoomBookingService : IRoomBookingService
    {
        private readonly RoomBookingAppDbContext _dbContext;

        public RoomBookingService(RoomBookingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Room> GetAvailableRooms(DateTime date)
        {
            return _dbContext.Rooms.Where(r => !r.RoomBookings.Any(x => x.Date == date)).ToList();
        }

        public void Save(RoomBooking roomBooking)
        {
            _dbContext.RoomBookings.Add(roomBooking);
            _dbContext.SaveChanges();
        }
    }
}
