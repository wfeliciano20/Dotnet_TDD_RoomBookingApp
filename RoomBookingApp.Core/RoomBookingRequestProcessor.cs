
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Core.Processors
{
    public class RoomBookingRequestProcessor : IRoomBookingRequestProcessor
    {
        private IRoomBookingService _RoomBookingService;

        public RoomBookingRequestProcessor(IRoomBookingService @object)
        {
            _RoomBookingService = @object;
        }

        public RoomBookingResult BookRoom(RoomBookingRequest bookingRequest)
        {

            if (bookingRequest == null)
            {
                throw new ArgumentNullException(nameof(bookingRequest), "Value cannot be null.");
            }

            var availableRooms = _RoomBookingService.GetAvailableRooms(bookingRequest.Date);
            RoomBookingResult result = CreateRoomBookingObject<RoomBookingResult>(bookingRequest);

            if (availableRooms.Any())
            {
                RoomBooking roomBooking = CreateRoomBookingObject<RoomBooking>(bookingRequest);
                roomBooking.RoomId = availableRooms.First().Id;
                _RoomBookingService.Save(roomBooking);
                result.Flag = BookingResultFlag.Success;
                result.RoomBookingId = roomBooking.RoomId;
            }
            else
            {
                result.Flag = BookingResultFlag.Failure;
                result.RoomBookingId = null;
            }

            return result;
        }

        private TRoomBooking CreateRoomBookingObject<TRoomBooking>(RoomBookingRequest bookingRequest) where TRoomBooking : RoomBookingBase, new()
        {
            return new TRoomBooking
            {
                FullName = bookingRequest.FullName,
                Email = bookingRequest.Email,
                Date = bookingRequest.Date
            };
        }
    }
}