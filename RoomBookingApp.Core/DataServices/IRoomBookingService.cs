using RoomBookingApp.Domain;


namespace RoomBookingApp.Core.DataServices
{
    public interface IRoomBookingService
    {
        List<Room> GetAvailableRooms(DateTime date);
        void Save(RoomBooking roomBooking);
    }
}
