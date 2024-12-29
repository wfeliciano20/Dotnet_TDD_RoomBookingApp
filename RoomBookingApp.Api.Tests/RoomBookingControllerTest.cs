﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomBookingApp.Api.Controllers;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;

namespace RoomBookingApp.Api.Tests
{
    public class RoomBookingControllerTest
    {
        private Mock<IRoomBookingRequestProcessor> _roomBookingProcessor;
        private RoomBookingController _controller;
        private RoomBookingRequest _request;
        private RoomBookingResult _result;

        public RoomBookingControllerTest()
        {
            _roomBookingProcessor = new Mock<IRoomBookingRequestProcessor>();
            _controller = new RoomBookingController(_roomBookingProcessor.Object);
            _request = new RoomBookingRequest();
            _result = new RoomBookingResult();


            _roomBookingProcessor.Setup(x => x.BookRoom(_request)).Returns(_result);
        }

        [Theory]
        [InlineData(1, true, typeof(OkObjectResult), BookingResultFlag.Success)]
        [InlineData(0, false, typeof(BadRequestObjectResult), BookingResultFlag.Failure)]
        public async Task Should_Call_Booking_Method_When_Valid(int expectedMEthodCalls, bool isModelValid, Type expectedActionResultType, BookingResultFlag bookingResultFlag)
        {
            if (!isModelValid)
            {
                _controller.ModelState.AddModelError("Key", "Error Message");
            }

            _result.Flag = bookingResultFlag;

            // ACT
            var result = await _controller.BookRoom(_request);

            //ASSERT
            Assert.IsType(expectedActionResultType, result);
            _roomBookingProcessor.Verify(x => x.BookRoom(_request), Times.Exactly(expectedMEthodCalls));
        }
    }
}
