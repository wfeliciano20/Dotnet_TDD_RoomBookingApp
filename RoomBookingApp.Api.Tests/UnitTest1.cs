using Microsoft.Extensions.Logging;
using Moq;
using RoomBookingApp.Api.Controllers;

namespace RoomBookingApp.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Shoud_Return_Weather_Forecast_Results()
        {
            var loggerMock = new Mock<ILogger<WeatherForecastController>>();
            var controller = new WeatherForecastController(loggerMock.Object);

            var results = controller.Get();

            Assert.True(results.Count() >= 1);
            Assert.NotNull(results);
        }
    }
}