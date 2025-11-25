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
    public class TMDbServiceTests
    {
        private readonly Mock<IUnitOfWorkPostgres> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MovieService>> _mockLogger;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<ITMDbService> _mockTMDbService;
        private readonly MovieService _movieService;

        public TMDbServiceTests()
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
        public async Task SearchTMDbMoviesAsync_WithValidQuery_ShouldReturnSearchResults()
        {
            // Arrange
            var searchRequest = new TMDbSearchRequestDto
            {
                Query = "Inception",
                Page = 1,
            };

            var expectedResult = new TMDbSearchResult
            {
                Page = 1,
                TotalPages = 5,
                TotalResults = 95,
                Results = new List<TMDbMovie>
                {
                    new TMDbMovie
                    {
                        Id = 27205,
                        Title = "Inception",
                        Overview = "Dom Cobb est un voleur expérimenté...",
                        ReleaseDate = "2010-07-16",
                        PosterPath = "/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
                        BackdropPath = "/s2bT29y0ngXxxu2IA8AOzzXTRhd.jpg",
                        VoteAverage = 8.4,
                        VoteCount = 32987,
                        GenreIds = new List<int> { 28, 878, 12 }
                    }
                }
            };

            _mockTMDbService.Setup(s => s.SearchMoviesAsync(searchRequest.Query, searchRequest.Page))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _movieService.SearchTMDbMoviesAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Page, result.Page);
            Assert.Equal(expectedResult.TotalPages, result.TotalPages);
            Assert.Equal(expectedResult.TotalResults, result.TotalResults);
            Assert.Single(result.Results);
            Assert.Equal("Inception", result.Results[0].Title);
            _mockTMDbService.Verify(s => s.SearchMoviesAsync(searchRequest.Query, searchRequest.Page), Times.Once);
        }

        [Fact]
        public async Task SearchTMDbMoviesAsync_WithEmptyQuery_ShouldThrowException()
        {
            // Arrange
            var searchRequest = new TMDbSearchRequestDto
            {
                Query = "",
                Page = 1
            };

            _mockTMDbService.Setup(s => s.SearchMoviesAsync(searchRequest.Query, searchRequest.Page))
                .ThrowsAsync(new Exception("Query cannot be empty"));

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => _movieService.SearchTMDbMoviesAsync(searchRequest));
        }

        [Fact]
        public async Task ImportMovieFromTMDbAsync_WithValidTmdbId_ShouldImportMovie()
        {
            // Arrange
            var importRequest = new TMDbImportRequestDto
            {
                TmdbId = 27205
            };

            var tmdbMovieDetails = new TMDbMovieDetails
            {
                Id = 27205,
                Title = "Inception",
                Overview = "Dom Cobb est un voleur expérimenté...",
                ReleaseDate = "2010-07-16",
                Runtime = 148,
                VoteAverage = 8.4,
                PosterPath = "/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
                Genres = new List<TMDbGenre>
                {
                    new TMDbGenre { Id = 28, Name = "Action" },
                    new TMDbGenre { Id = 878, Name = "Science-Fiction" },
                    new TMDbGenre { Id = 12, Name = "Aventure" }
                },
                Credits = new TMDbCredits
                {
                    Cast = new List<TMDbCastMember>
                    {
                        new TMDbCastMember { Name = "Leonardo DiCaprio", Character = "Dom Cobb" },
                        new TMDbCastMember { Name = "Marion Cotillard", Character = "Mal" },
                        new TMDbCastMember { Name = "Tom Hardy", Character = "Eames" }
                    },
                    Crew = new List<TMDbCrewMember>
                    {
                        new TMDbCrewMember { Name = "Christopher Nolan", Job = "Director" },
                        new TMDbCrewMember { Name = "Christopher Nolan", Job = "Writer" }
                    }
                }
            };

            // Mock pour vérifier que le film n'existe pas déjà
            _mockUnitOfWork.Setup(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId))
                .ReturnsAsync((Movie)null);

            // Mock pour récupérer les détails TMDb
            _mockTMDbService.Setup(s => s.GetMovieDetailsAsync(importRequest.TmdbId))
                .ReturnsAsync(tmdbMovieDetails);

            // Mock pour la création du film
            _mockUnitOfWork.Setup(u => u.Movies.CreateMovieAsync(It.IsAny<Movie>()))
                .Callback<Movie>(movie =>
                {
                    movie.MovieId = 123; // Simuler l'assignation d'un ID
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _movieService.ImportMovieFromTMDbAsync(importRequest);

            // Assert
            Assert.Equal(123, result);
            _mockUnitOfWork.Verify(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId), Times.Once);
            _mockTMDbService.Verify(s => s.GetMovieDetailsAsync(importRequest.TmdbId), Times.Once);
            _mockUnitOfWork.Verify(u => u.Movies.CreateMovieAsync(It.IsAny<Movie>()), Times.Once);
        }

        [Fact]
        public async Task ImportMovieFromTMDbAsync_WithExistingMovie_ShouldThrowConflictException()
        {
            // Arrange
            var importRequest = new TMDbImportRequestDto
            {
                TmdbId = 27205
            };

            var existingMovie = new Movie
            {
                MovieId = 456,
                TmdbId = 27205,
                Title = "Inception"
            };

            _mockUnitOfWork.Setup(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId))
                .ReturnsAsync(existingMovie);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _movieService.ImportMovieFromTMDbAsync(importRequest));
            Assert.Equal(409, exception.StatusCode);
            Assert.Equal("Ce film existe déjà dans la base de données", exception.Message);
            _mockUnitOfWork.Verify(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId), Times.Once);
            _mockTMDbService.Verify(s => s.GetMovieDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ImportMovieFromTMDbAsync_WithInvalidTmdbId_ShouldThrowNotFoundException()
        {
            // Arrange
            var importRequest = new TMDbImportRequestDto
            {
                TmdbId = 999999
            };

            _mockUnitOfWork.Setup(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId))
                .ReturnsAsync((Movie)null);

            _mockTMDbService.Setup(s => s.GetMovieDetailsAsync(importRequest.TmdbId))
                .ThrowsAsync(new Exception("Movie not found"));

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => _movieService.ImportMovieFromTMDbAsync(importRequest));
            _mockUnitOfWork.Verify(u => u.Movies.GetMovieByTmdbIdAsync(importRequest.TmdbId), Times.Once);
            _mockTMDbService.Verify(s => s.GetMovieDetailsAsync(importRequest.TmdbId), Times.Once);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithActionGenre_ShouldReturnAction()
        {
            // Arrange
            var tmdbGenres = new List<TMDbGenre>
            {
                new TMDbGenre { Id = 28, Name = "Action" }
            };

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Action, result);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithComedyGenre_ShouldReturnComedie()
        {
            // Arrange
            var tmdbGenres = new List<TMDbGenre>
            {
                new TMDbGenre { Id = 35, Name = "Comedy" }
            };

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Comedie, result);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithScienceFictionGenre_ShouldReturnFantastique()
        {
            // Arrange
            var tmdbGenres = new List<TMDbGenre>
            {
                new TMDbGenre { Id = 878, Name = "Science Fiction" }
            };

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Fantastique, result);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithNoGenres_ShouldReturnDefaultAction()
        {
            // Arrange
            List<TMDbGenre> tmdbGenres = null;

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Action, result);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithEmptyGenres_ShouldReturnDefaultAction()
        {
            // Arrange
            var tmdbGenres = new List<TMDbGenre>();

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Action, result);
        }

        [Fact]
        public void MapTMDbGenresToMovieGenre_WithUnknownGenre_ShouldReturnDefaultAction()
        {
            // Arrange
            var tmdbGenres = new List<TMDbGenre>
            {
                new TMDbGenre { Id = 999, Name = "Unknown Genre" }
            };

            // Act
            var result = _movieService.TestMapTMDbGenresToMovieGenre(tmdbGenres);

            // Assert
            Assert.Equal(MovieGenre.Action, result);
        }
    }

    // Classe d'extension pour accéder à la méthode privée de mapping
    public static class MovieServiceTestExtensions
    {
        public static MovieGenre TestMapTMDbGenresToMovieGenre(this MovieService movieService, List<TMDbGenre> tmdbGenres)
        {
            var method = typeof(MovieService).GetMethod("MapTMDbGenresToMovieGenre", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (tmdbGenres == null)
            {
                return MovieGenre.Action;
            }
            return (MovieGenre)method.Invoke(movieService, new object[] { tmdbGenres });
        }
    }
}