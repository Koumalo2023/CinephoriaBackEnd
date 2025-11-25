using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Configurations
{
    public class ResendConfirmationRequest
    {
        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }
    }
}
