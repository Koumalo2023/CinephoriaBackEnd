namespace CinephoriaServer.API.Configurations
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An unexpected error occurred.";
            var logLevel = LogLevel.Error;

            switch (ex)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    logLevel = LogLevel.Warning;
                    break;

                case ValidationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    logLevel = LogLevel.Warning;
                    break;

                case UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    logLevel = LogLevel.Warning;
                    break;

                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    logLevel = LogLevel.Warning;
                    break;

                default:
                    // Pour les erreurs inattendues, on garde le statut 500 et le message par défaut
                    break;
            }

            // Journalisation avec plus de contexte
            _logger.Log(logLevel, ex, "An error occurred: {Message}. Request: {Method} {Url}",
                ex.Message, context.Request.Method, context.Request.Path);

            // Retourner une réponse structurée
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>(message, statusCode));
        }
    }
}
