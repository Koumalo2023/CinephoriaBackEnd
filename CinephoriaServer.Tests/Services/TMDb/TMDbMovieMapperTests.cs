using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.TMDb;
using CinephoriaServer.API.Services.TMDb;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.Tests.Services.TMDb
{
    public class TMDbMovieMapperTests
    {
        private readonly TMDbMovieMapper _mapper;

        public TMDbMovieMapperTests()
        {
            _mapper = new TMDbMovieMapper();
        }

        [Fact]
        public void MapToMovie_WithValidData_ReturnsCorrectMovie()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 550,
                Title = "Fight Club",
                Overview = "Un homme déprimé rencontre un vendeur de savon...",
                ReleaseDate = "1999-10-15",
                Runtime = 139,
                PosterPath = "/bptfVGEQuv6vDTIMVCHjJ9Dz8PX.jpg",
                VoteAverage = 8.4,
                Genres = new List<TMDbGenre>
                {
                    new() { Id = 18, Name = "Drama" }
                },
                Credits = new TMDbCredits
                {
                    Cast = new List<TMDbCastMember>
                    {
                        new() { Name = "Brad Pitt", Character = "Tyler Durden", Order = 0 },
                        new() { Name = "Edward Norton", Character = "Narrator", Order = 1 },
                        new() { Name = "Helena Bonham Carter", Character = "Marla Singer", Order = 2 }
                    },
                    Crew = new List<TMDbCrewMember>
                    {
                        new() { Name = "David Fincher", Job = "Director", Department = "Directing" },
                        new() { Name = "Art Linson", Job = "Producer", Department = "Production" }
                    }
                }
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(550, result.TmdbId);
            Assert.Equal("Fight Club", result.Title);
            Assert.Equal("Un homme déprimé rencontre un vendeur de savon...", result.Description);
            Assert.Equal(new DateTime(1999, 10, 15), result.ReleaseDate);
            Assert.Equal("139", result.Duration);
            Assert.Contains("David Fincher", result.Director);
            Assert.Equal(3, result.Actors.Count);
            Assert.Contains("Brad Pitt", result.Actors);
            Assert.Contains("Edward Norton", result.Actors);
            Assert.Contains("Helena Bonham Carter", result.Actors);
            Assert.Equal(MovieGenre.Thriller, result.Genre);
            Assert.Equal("https://image.tmdb.org/t/p/original/bptfVGEQuv6vDTIMVCHjJ9Dz8PX.jpg", result.PosterUrls);
            Assert.Equal(4.2, result.AverageRating); // 8.4 / 2 = 4.2
            Assert.Equal(MinimumAge.Public, result.MinimumAge);
            Assert.False(result.IsFavorite);
        }

        [Fact]
        public void MapToMovie_WithNullTmdbMovie_ThrowsArgumentNullException()
        {
            // Arrange
            TMDbMovieDetails tmdbMovie = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mapper.MapToMovie(tmdbMovie));
        }

        [Fact]
        public void MapToMovie_WithMissingReleaseDate_UsesCurrentDate()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "invalid-date",
                Runtime = 120,
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.InRange(result.ReleaseDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
        }

        [Fact]
        public void MapToMovie_WithNullRuntime_UsesDefaultDuration()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = null,
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal("120", result.Duration);
        }

        [Fact]
        public void MapToMovie_WithMultipleDirectors_ReturnsLimitedList()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits
                {
                    Crew = new List<TMDbCrewMember>
                    {
                        new() { Name = "Director 1", Job = "Director", Department = "Directing" },
                        new() { Name = "Director 2", Job = "Director", Department = "Directing" },
                        new() { Name = "Director 3", Job = "Director", Department = "Directing" },
                        new() { Name = "Director 4", Job = "Director", Department = "Directing" },
                        new() { Name = "Producer 1", Job = "Producer", Department = "Production" }
                    }
                }
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(3, result.Director.Count);
            Assert.Contains("Director 1", result.Director);
            Assert.Contains("Director 2", result.Director);
            Assert.Contains("Director 3", result.Director);
            Assert.DoesNotContain("Director 4", result.Director);
            Assert.DoesNotContain("Producer 1", result.Director);
        }

        [Fact]
        public void MapToMovie_WithMultipleActors_ReturnsLimitedOrderedList()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits
                {
                    Cast = new List<TMDbCastMember>
                    {
                        new() { Name = "Actor 2", Character = "Role 2", Order = 1 },
                        new() { Name = "Actor 1", Character = "Role 1", Order = 0 },
                        new() { Name = "Actor 3", Character = "Role 3", Order = 2 },
                        new() { Name = "Actor 4", Character = "Role 4", Order = 3 },
                        new() { Name = "Actor 5", Character = "Role 5", Order = 4 },
                        new() { Name = "Actor 6", Character = "Role 6", Order = 5 }
                    }
                }
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(5, result.Actors.Count);
            Assert.Equal("Actor 1", result.Actors[0]); // Most important first
            Assert.Equal("Actor 2", result.Actors[1]);
            Assert.Equal("Actor 3", result.Actors[2]);
            Assert.Equal("Actor 4", result.Actors[3]);
            Assert.Equal("Actor 5", result.Actors[4]);
            Assert.DoesNotContain("Actor 6", result.Actors);
        }

        [Fact]
        public void MapToMovie_WithEmptyPosterPath_ReturnsNullPosterUrl()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                PosterPath = "",
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Null(result.PosterUrls);
        }

        [Theory]
        [InlineData("Action", MovieGenre.Action)]
        [InlineData("Adventure", MovieGenre.Action)]
        [InlineData("Comedy", MovieGenre.Comedie)]
        [InlineData("Drama", MovieGenre.Thriller)]
        [InlineData("Horror", MovieGenre.Horreur)]
        [InlineData("Romance", MovieGenre.Romance)]
        [InlineData("Science Fiction", MovieGenre.Fantastique)]
        [InlineData("Fantasy", MovieGenre.Fantastique)]
        [InlineData("Thriller", MovieGenre.Thriller)]
        [InlineData("Mystery", MovieGenre.Thriller)]
        [InlineData("Animation", MovieGenre.Animation)]
        [InlineData("Documentary", MovieGenre.Documentaire)]
        [InlineData("Crime", MovieGenre.Crime)]
        [InlineData("War", MovieGenre.Guerre)]
        [InlineData("Western", MovieGenre.Western)]
        [InlineData("Family", MovieGenre.Familiale)]
        [InlineData("Unknown Genre", MovieGenre.Action)]
        public void MapGenres_WithVariousGenres_ReturnsCorrectMovieGenre(string genreName, MovieGenre expectedGenre)
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                Genres = new List<TMDbGenre> { new() { Id = 1, Name = genreName } },
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(expectedGenre, result.Genre);
        }

        [Fact]
        public void MapToMovie_WithEmptyGenres_ReturnsDefaultGenre()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(MovieGenre.Action, result.Genre);
        }

        [Fact]
        public void MapToMovie_WithNullCredits_ReturnsEmptyLists()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                Genres = new List<TMDbGenre>(),
                Credits = null
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Empty(result.Director);
            Assert.Empty(result.Actors);
        }

        [Fact]
        public void NormalizeRating_ConvertsTMDbRatingToLocalScale()
        {
            // Arrange
            var tmdbMovie = new TMDbMovieDetails
            {
                Id = 1,
                Title = "Test Movie",
                Overview = "Test overview",
                ReleaseDate = "2020-01-01",
                Runtime = 120,
                VoteAverage = 8.0, // TMDb scale: 0-10
                Genres = new List<TMDbGenre>(),
                Credits = new TMDbCredits()
            };

            // Act
            var result = _mapper.MapToMovie(tmdbMovie);

            // Assert
            Assert.Equal(4.0, result.AverageRating); // Local scale: 0-5
        }
    }
}