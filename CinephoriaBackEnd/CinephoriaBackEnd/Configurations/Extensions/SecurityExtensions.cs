using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CinephoriaBackEnd.Configurations.Extensions
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
                    buildercors
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                });
            });
        }
    }
}
