
using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Core
{
    public class RoomBookingRequestProcessorTest
    {
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _request;
        private Mock<IRoomBookingService>_RoomBookingServiceMock;
        private List<Room> _availableRooms;

        public RoomBookingRequestProcessorTest()
        {
            _request = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@test.com",
                Date = new DateTime(2024, 12, 27),
            };

            _RoomBookingServiceMock = new Mock<IRoomBookingService>();
           
            _processor = new RoomBookingRequestProcessor(_RoomBookingServiceMock.Object);
            _availableRooms = new List<Room> { new Room() { Id = 1 } };

            _RoomBookingServiceMock.Setup(rb => rb.GetAvailableRooms(_request.Date)).Returns(_availableRooms);
        }


        [Fact]
        public void Should_Return_Room_Booking_Response_With_Request_values()
        {

            RoomBookingResult results = _processor.BookRoom(_request);

            // ASSERT
            Assert.NotNull(results);

            Assert.Equal(_request.FullName, results.FullName);
            Assert.Equal(_request.Email, results.Email);
            Assert.Equal(_request.Date, results.Date);
        }


        [Fact]
        public void Should_Throw_Null_Exception_When_Booking_Request_Is_Null()
        {
            // ACT
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookRoom(null));

            // ASSERT
            Assert.Equal("Value cannot be null. (Parameter 'bookingRequest')", exception.Message);
        }


        [Fact]
        public void Should_Save_Room_Booking_Request()
        {
            RoomBooking SavedBooking = null;

            _RoomBookingServiceMock.Setup(rb => rb.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking => SavedBooking = booking);

            _processor.BookRoom(_request);

            _RoomBookingServiceMock.Verify(rb => rb.Save(It.IsAny<RoomBooking>()), Times.Once);

            Assert.NotNull(SavedBooking);
            Assert.Equal(_request.FullName, SavedBooking.FullName);
            Assert.Equal(_request.Email, SavedBooking.Email);
            Assert.Equal(_request.Date, SavedBooking.Date);
            Assert.Equal(_availableRooms.First().Id, SavedBooking.RoomId);
        }

        [Fact]
        public void Should_Not_Save_Room_Booking_Request_If_None_Available() 
        { 
            _availableRooms.Clear();

            _processor.BookRoom(_request);
            _RoomBookingServiceMock.Verify(rb => rb.Save(It.IsAny<RoomBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(BookingResultFlag.Failure, false)]
        [InlineData(BookingResultFlag.Success, true)]
        public void Should_Return_Success_Or_Failure_Flag_In_Return(BookingResultFlag bookingResultFlag,bool isAvailable)
        {
            if(!isAvailable)
            {
                _availableRooms.Clear();
            }

            var result = _processor.BookRoom(_request);

            Assert.Equal(bookingResultFlag, result.Flag);
        }


        [Theory]
        [InlineData(1, true)]
        [InlineData(null, false)]

        public void Should_Return_RoomBookingId_If_Room_Is_Available(int? roomBookingId, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }
            else 
            {
                _RoomBookingServiceMock.Setup(rb => rb.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking => booking.Id = roomBookingId);
            }

            var result = _processor.BookRoom(_request);

            Assert.Equal(roomBookingId, result.RoomBookingId);
        }
    }
}