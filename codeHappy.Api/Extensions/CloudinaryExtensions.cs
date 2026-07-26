using CloudinaryDotNet;

namespace codeHappy.Api.Extensions;

public static class CloudinaryExtensions 
{
    public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration config)
    {
        var apikey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];
        var cloudName = config["Cloudinary:CloudName"];
        
        Account account = new Account(
            cloudName,
            apikey,
            apiSecret
            );

        Cloudinary cloudinary = new Cloudinary(account);
        cloudinary.Api.Secure = true;
        
        services.AddSingleton(cloudinary);
        
        return services;
    }
}