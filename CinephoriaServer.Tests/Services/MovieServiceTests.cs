using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.TMDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Services;
using CinephoriaServer.API.Services.TMDb;
using Microsoft.Extensions.Logging;
using Moq;
using static CinephoriaServer.API.Configurations.EnumConfig;
using Xunit;

namespace CinephoriaServer.API.Tests.Services
{
    public class MovieServiceTests
    {
        private readonly Mock<IUnitOfWorkPostgres> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MovieService>> _mockLogger;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<ITMDbService> _mockTMDbService;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkPostgres>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MovieService>>();
            _mockImageService = new Mock<IImageService>();
            _mockTMDbService = new Mock<ITMDbService>();

            _movieService = new MovieService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockImageService.Object,
                _mockTMDbService.Object
            );
        }

        [Fact]
        public async Task FilterMoviesAsync_WithAllFilters_ShouldReturnFilteredMovies()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto
            {
                CinemaId = 1,
                Genre = MovieGenre.Action,
                Date = new DateTime(2024, 1, 15),
                Year = 2023,
                Director = "Christopher Nolan",
                Actor = "Leonardo DiCaprio",
                MinimumAge = MinimumAge.Twelve
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    Director = new List<string> { "Christopher Nolan" },
                    Actors = new List<string> { "Leonardo DiCaprio", "Tom Hardy" },
                    ReleaseDate = new DateTime(2023, 7, 16),
                    MinimumAge = MinimumAge.Twelve,
                    Genre = MovieGenre.Action,
                    Showtimes = new List<Showtime>
                    {
                        new Showtime { CinemaId = 1, StartTime = new DateTime(2024, 1, 15, 20, 0, 0) }
                    }
                }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                filterDto.CinemaId,
                filterDto.Genre,
                filterDto.Date,
                filterDto.Year,
                filterDto.Director,
                filterDto.Actor,
                filterDto.MinimumAge
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MovieId);
            Assert.Equal("Inception", result[0].Title);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                filterDto.CinemaId,
                filterDto.Genre,
                filterDto.Date,
                filterDto.Year,
                filterDto.Director,
                filterDto.Actor,
                filterDto.MinimumAge
            ), Times.Once);
        }

        [Fact]
        public async Task FilterMoviesAsync_WithYearFilterOnly_ShouldReturnMoviesFromThatYear()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto
            {
                Year = 2023
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    ReleaseDate = new DateTime(2023, 7, 16)
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "Interstellar",
                    ReleaseDate = new DateTime(2023, 11, 5)
                }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                null, null, null, 2023, null, null, null
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" },
                new MovieDto { MovieId = 2, Title = "Interstellar" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Equal(2, result.Count);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                null, null, null, 2023, null, null, null
            ), Times.Once);
        }

        [Fact]
        public async Task FilterMoviesAsync_WithDirectorFilter_ShouldReturnMoviesByDirector()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto
            {
                Director = "Christopher Nolan"
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    Director = new List<string> { "Christopher Nolan" }
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "Interstellar",
                    Director = new List<string> { "Christopher Nolan" }
                }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, "Christopher Nolan", null, null
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" },
                new MovieDto { MovieId = 2, Title = "Interstellar" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Equal(2, result.Count);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, "Christopher Nolan", null, null
            ), Times.Once);
        }

        [Fact]
        public async Task FilterMoviesAsync_WithActorFilter_ShouldReturnMoviesWithActor()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto
            {
                Actor = "Leonardo DiCaprio"
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    Actors = new List<string> { "Leonardo DiCaprio", "Tom Hardy" }
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "The Revenant",
                    Actors = new List<string> { "Leonardo DiCaprio", "Tom Hardy" }
                }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, "Leonardo DiCaprio", null
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" },
                new MovieDto { MovieId = 2, Title = "The Revenant" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Equal(2, result.Count);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, "Leonardo DiCaprio", null
            ), Times.Once);
        }

        [Fact]
        public async Task FilterMoviesAsync_WithMinimumAgeFilter_ShouldReturnAppropriateMovies()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto
            {
                MinimumAge = MinimumAge.Twelve
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    MinimumAge = MinimumAge.Twelve
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "Interstellar",
                    MinimumAge = MinimumAge.Twelve
                }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, null, MinimumAge.Twelve
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" },
                new MovieDto { MovieId = 2, Title = "Interstellar" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Equal(2, result.Count);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, null, MinimumAge.Twelve
            ), Times.Once);
        }

        [Fact]
        public async Task FilterMoviesAsync_WithNoFilters_ShouldReturnAllMovies()
        {
            // Arrange
            var filterDto = new FilterMoviesRequestDto();

            var movies = new List<Movie>
            {
                new Movie { MovieId = 1, Title = "Inception" },
                new Movie { MovieId = 2, Title = "Interstellar" },
                new Movie { MovieId = 3, Title = "The Dark Knight" }
            };

            _mockUnitOfWork.Setup(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, null, null
            )).ReturnsAsync(movies);

            var movieDtos = new List<MovieDto>
            {
                new MovieDto { MovieId = 1, Title = "Inception" },
                new MovieDto { MovieId = 2, Title = "Interstellar" },
                new MovieDto { MovieId = 3, Title = "The Dark Knight" }
            };

            _mockMapper.Setup(m => m.Map<List<MovieDto>>(movies))
                .Returns(movieDtos);

            // Act
            var result = await _movieService.FilterMoviesAsync(filterDto);

            // Assert
            Assert.Equal(3, result.Count);
            _mockUnitOfWork.Verify(u => u.Movies.FilterMoviesAsync(
                null, null, null, null, null, null, null
            ), Times.Once);
        }
    }
}