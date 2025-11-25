namespace CinephoriaServer.API.Configurations
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        // Constructeur pour les réponses réussies
        public ApiResponse(T data, string message = null, int statusCode = StatusCodes.Status200OK)
        {
            Success = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
            Errors = null;
        }

        // Constructeur pour les réponses d'erreur
        public ApiResponse(string message, int statusCode, IEnumerable<string> errors = null)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
            Data = default;
        }

        // Méthodes statiques pour des réponses standardisées
        public static ApiResponse<T> SuccessResponse(T data, string message = null)
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<T> ErrorResponse(string message, int statusCode, IEnumerable<string> errors = null)
        {
            return new ApiResponse<T>(message, statusCode, errors);
        }
    }
}
