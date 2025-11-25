
namespace CinephoriaServer.API.Configurations.Extensions
{
    public static class SecurityExtensions
    {
        #region Constants
        public const string DEFAULT_POLICY = "Cinephoria_CORS_policy";
        #endregion

        public static void AddCustomSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            //Cors
            AddCustomSecurityCORS(services, configuration);

        }


        public static void AddCustomSecurityCORS(this IServiceCollection services, IConfiguration configuration)
        {
            CorsOption corsOption = new CorsOption();
            configuration.GetSection("FrontendSettings").Bind(corsOption);

            services.AddCors(options =>
            {
                options.AddPolicy(DEFAULT_POLICY, buildercors =>
                {
                    if (corsOption.AllowedOrigins.Length > 0)
                    {
                        buildercors
                            .WithOrigins("http://localhost:4200", "https://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                    else
                    {
                        buildercors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                });
            });
        }
    }
}
