using FluentValidation;
using codeHappy.Business.Dtos;
using codeHappy.Business.Interfaces;

namespace codeHappy.Api.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/snippets/{snippetId}/images").RequireAuthorization();
        
        //Post 

        group.MapPost("/upload", async (
            Guid snippetId,
            IFormFile file,
            ICurrentUserService user,
            IImagesService service,
            IValidator<UploadImageRequest> validator,
            CancellationToken ct
        ) =>
        {
            var upload = new UploadImageRequest(
                FileName: file.FileName,
                ContentType: file.ContentType,
                FileLength: file.Length
            );
            var result = await validator.ValidateAsync(upload, ct);
            
            
            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());
            
            var userId = user.GetUserId();
            
            if(string.IsNullOrEmpty(userId))
                return Results.Unauthorized();
            
            var image = await service.UploadFileAsync(Guid.Parse(userId), snippetId,file.FileName,file.OpenReadStream(), ct);
            
            return Results.Created($"api/snippets/{snippetId}/images/{image.PublicId}", image);
        });
        
        //Delete
        var images = app.MapGroup("api/images").RequireAuthorization();      
        
        images.MapDelete("/{**publicId}", async (
            string publicId,
            ICurrentUserService user, 
            IImagesService service,
            CancellationToken ct) =>
        {
            var userId =  user.GetUserId();
            
            if(string.IsNullOrEmpty(userId))
                return Results.Unauthorized();
            
            await service.DeleteFileAsync(Guid.Parse(userId),publicId, ct);
           
            return Results.NoContent();

        });


    }
    
}