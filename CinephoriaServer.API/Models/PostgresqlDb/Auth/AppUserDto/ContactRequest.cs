namespace CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto
{
    public class ContactRequest
    {
        public string? Username { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
