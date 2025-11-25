using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMovieService _movieService;
        private readonly IIncidentService _incidentService;
        //private readonly IUserService _userService;

        public ImageController(
            IImageService imageService,
            IMovieService movieService,
            IIncidentService incidentService
            //IUserService userService
            )
        {
            _imageService = imageService;
            _movieService = movieService;
            _incidentService = incidentService;
            //_userService = userService;
        }







    }
}
