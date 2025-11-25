namespace CinephoriaServer.API.Configurations
{
    public class HandlingMiddlewareError
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandlingMiddlewareError> _logger;

        public HandlingMiddlewareError(RequestDelegate next, ILogger<HandlingMiddlewareError> logger)
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
            var message = "Une erreur inattendue s'est produite.";

            if (ex is ApiException apiException)
            {
                statusCode = apiException.StatusCode;
                message = apiException.Message;
            }

            _logger.LogError(ex, "Erreur : {Message}", ex.Message);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { Message = message, StatusCode = statusCode });
        }
    }
}
