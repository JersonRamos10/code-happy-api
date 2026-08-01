namespace codeHappy.Api.Endpoints
{
    public static class HealthEndpoints

    {
        public static void MapHealthEndpoint(this WebApplication app)
        {
            app.MapGet("/api/health", () =>
            {
                var healthStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow
                };
                return Results.Ok(healthStatus);
            }); 
        }
    }
}
