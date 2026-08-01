namespace codeHappy.Api.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddClientCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? throw new ArgumentNullException("AllowedOrigins");

            if (allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one allowed origin.");
            }

            services.AddCors(options =>
                options.AddPolicy("FrontendCors", policy =>
                {

                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .WithHeaders("Content-Type", "Authorization");
                })

               

            );
            return services;
        }

        
    }
}
