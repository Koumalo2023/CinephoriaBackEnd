using AutoMapper;
using CinephoriaServer.API.Data;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Services;
using Microsoft.Extensions.Logging;
using Moq;  

namespace CinephoriaServer.API.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IUnitOfWorkPostgres> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ReservationService>> _mockLogger;
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkPostgres>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ReservationService>>();

            // Create a simple mock context
            var mockContext = new Mock<CinephoriaDbContext>();

            _reservationService = new ReservationService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                mockContext.Object
            );
        }

        [Fact]
        public async Task CreateReservationAsync_WithValidData_ShouldCreateReservation()
        {
            // Arrange
            var createReservationDto = new CreateReservationDto
            {
                AppUserId = "user123",
                ShowtimeId = 1,
                SeatNumbers = new List<string> { "A1", "A2" }
            };

            var showtime = new Showtime { ShowtimeId = 1, MovieId = 1 };
            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = "A1" },
                new Seat { SeatId = 2, SeatNumber = "A2" }
            };

            // Setup mock methods - use exact parameter matching to avoid optional parameter issues
            _mockUnitOfWork.Setup(u => u.Showtimes.GetByIdAsync(It.Is<int>(id => id == 1), It.IsAny<bool>()))
                .ReturnsAsync(showtime);
            _mockUnitOfWork.Setup(u => u.Seats.GetSeatsByNumbersAsync(It.Is<int>(id => id == 1), It.IsAny<List<string>>()))
                .ReturnsAsync(seats);
            _mockUnitOfWork.Setup(u => u.Reservations.CalculateReservationPriceAsync(It.IsAny<Showtime>(), It.IsAny<List<Seat>>()))
                .ReturnsAsync(20.0m);
            _mockUnitOfWork.Setup(u => u.Reservations.HoldSeatsAsync(It.Is<int>(id => id == 1), It.IsAny<List<Seat>>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.Reservations.CreateReservationAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<Reservation>(It.IsAny<CreateReservationDto>()))
                .Returns(new Reservation());

            // Act
            var result = await _reservationService.CreateReservationAsync(createReservationDto);

            // Assert
            Assert.Equal("Réservation créée et confirmée avec succès.", result);
            _mockUnitOfWork.Verify(u => u.Reservations.HoldSeatsAsync(1, seats), Times.Once);
            _mockUnitOfWork.Verify(u => u.Reservations.CreateReservationAsync(It.IsAny<Reservation>()), Times.Once);
        }

        [Fact]
        public async Task CreateReservationAsync_WithNullData_ShouldThrowApiException()
        {
            // Arrange
            CreateReservationDto createReservationDto = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _reservationService.CreateReservationAsync(createReservationDto));

            Assert.Equal("Les données de la réservation sont invalides.", exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact]
        public async Task CreateReservationAsync_WithNonExistentShowtime_ShouldThrowApiException()
        {
            // Arrange
            var createReservationDto = new CreateReservationDto
            {
                AppUserId = "user123",
                ShowtimeId = 999,
                SeatNumbers = new List<string> { "A1" }
            };

            _mockUnitOfWork.Setup(u => u.Showtimes.GetByIdAsync(It.Is<int>(id => id == 999), It.IsAny<bool>()))
                .ReturnsAsync((Showtime)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _reservationService.CreateReservationAsync(createReservationDto));

            Assert.Equal("Séance non trouvée.", exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact]
        public async Task CreateReservationAsync_WithNonExistentSeats_ShouldThrowApiException()
        {
            // Arrange
            var createReservationDto = new CreateReservationDto
            {
                AppUserId = "user123",
                ShowtimeId = 1,
                SeatNumbers = new List<string> { "Z99" }
            };

            var showtime = new Showtime { ShowtimeId = 1 };
            _mockUnitOfWork.Setup(u => u.Showtimes.GetByIdAsync(It.Is<int>(id => id == 1), It.IsAny<bool>()))
                .ReturnsAsync(showtime);
            _mockUnitOfWork.Setup(u => u.Seats.GetSeatsByNumbersAsync(It.Is<int>(id => id == 1), It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Seat>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _reservationService.CreateReservationAsync(createReservationDto));

            Assert.Equal("Aucun siège trouvé avec les numéros fournis.", exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact]
        public async Task CalculateReservationPriceAsync_WithValidData_ShouldReturnCorrectPrice()
        {
            // Arrange
            int showtimeId = 1;
            var seatNumbers = new List<string> { "A1", "A2" };

            var showtime = new Showtime { ShowtimeId = 1 };
            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = "A1" },
                new Seat { SeatId = 2, SeatNumber = "A2" }
            };

            _mockUnitOfWork.Setup(u => u.Showtimes.GetByIdAsync(It.Is<int>(id => id == 1), It.IsAny<bool>()))
                .ReturnsAsync(showtime);
            _mockUnitOfWork.Setup(u => u.Seats.GetSeatsByNumbersAsync(It.Is<int>(id => id == 1), It.Is<List<string>>(numbers => numbers.SequenceEqual(seatNumbers))))
                .ReturnsAsync(seats);
            _mockUnitOfWork.Setup(u => u.Reservations.CalculateReservationPriceAsync(It.IsAny<Showtime>(), It.IsAny<List<Seat>>()))
                .ReturnsAsync(30.0m);

            // Act
            var result = await _reservationService.CalculateReservationPriceAsync(showtimeId, seatNumbers);

            // Assert
            Assert.Equal(30.0m, result);
        }

        [Fact]
        public async Task CancelReservationAsync_WithInvalidReservationId_ShouldThrowApiException()
        {
            // Arrange
            int reservationId = 0;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _reservationService.CancelReservationAsync(reservationId));

            Assert.Equal("L'identifiant de la réservation doit être un nombre positif.", exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact]
        public async Task GetUserReservationsAsync_WithValidUserId_ShouldReturnReservations()
        {
            // Arrange
            string userId = "user123";
            var reservations = new List<Reservation>
            {
                new Reservation { ReservationId = 1, AppUserId = "user123" }
            };

            _mockUnitOfWork.Setup(u => u.Reservations.GetUserReservationsAsync(userId)).ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetUserReservationsAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].ReservationId);
        }

        [Fact]
        public async Task GetUserReservationsAsync_WithEmptyUserId_ShouldThrowApiException()
        {
            // Arrange
            string userId = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _reservationService.GetUserReservationsAsync(userId));

            Assert.Equal("L'identifiant de l'utilisateur est invalide.", exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact]
        public async Task HoldSeatsAsync_WithValidData_ShouldHoldSeats()
        {
            // Arrange
            int showtimeId = 1;
            var seatNumbers = new List<string> { "A1", "A2" };
            var showtime = new Showtime { ShowtimeId = 1 };
            var seats = new List<Seat>
            {
                new Seat { SeatId = 1, SeatNumber = "A1" },
                new Seat { SeatId = 2, SeatNumber = "A2" }
            };

            _mockUnitOfWork.Setup(u => u.Showtimes.GetByIdAsync(It.Is<int>(id => id == 1), It.IsAny<bool>()))
                .ReturnsAsync(showtime);
            _mockUnitOfWork.Setup(u => u.Seats.GetSeatsByNumbersAsync(It.Is<int>(id => id == 1), It.Is<List<string>>(numbers => numbers.SequenceEqual(seatNumbers))))
                .ReturnsAsync(seats);
            _mockUnitOfWork.Setup(u => u.Reservations.HoldSeatsAsync(It.Is<int>(id => id == 1), It.IsAny<List<Seat>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _reservationService.HoldSeatsAsync(showtimeId, seatNumbers);

            // Assert
            _mockUnitOfWork.Verify(u => u.Reservations.HoldSeatsAsync(1, seats), Times.Once);
        }
    }
}