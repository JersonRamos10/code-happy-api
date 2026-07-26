using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Mappers;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace codeHappy.Business.Services;

public class ImagesService(CodeHappyContext context, Cloudinary cloudinary, ILogger<ImagesService> logger) : IImagesService 
{
    private readonly CodeHappyContext _context = context;
    private readonly Cloudinary _cloudinary = cloudinary;
    private readonly ILogger<ImagesService> _logger = logger;
    
    public async Task<ImageMetadataResponse> UploadFileAsync(Guid userId, Guid snippetId, string fileName, Stream file, CancellationToken ct)
    {
        var snippet = 
            await _context.Snippets.FirstOrDefaultAsync(s => s.Id == snippetId, ct)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, file),
            UseFilename = true,
            UniqueFilename = true,
        };
        
        var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);
        
        if (uploadResult.StatusCode != HttpStatusCode.OK)                      
            throw new ExternalServiceException(uploadResult.Error?.Message ??  
                                               "Cloudinary upload failed.");

        var image = new ImageMetadata
        {
            PublicId = uploadResult.PublicId,
            SecureUrl = uploadResult.SecureUrl.ToString(),
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            Format =  uploadResult.Format,
            Bytes =  uploadResult.Bytes,
            Alt = null,
            BucketPath = null
            
        };
        
        return ImageMetadataMapper.ToResponse(image);
    }
    public async Task DeleteFileAsync(Guid userId, string publicId, CancellationToken ct)
    {
        var block = await _context.Blocks.Include(b => b.Snippet)
            .Where(b => b.ImageMetadata!= null && b.ImageMetadata.PublicId == publicId)
            .FirstOrDefaultAsync(ct) 
                          ?? throw new NotFoundException($"Block with public_id '{publicId}' Not found.");
        
        if(block.Snippet.OwnerId != userId)
            throw new ForbiddenException();

        await DestroyAssetBestEffortAsync(publicId, ct);
    }

    public async Task DestroyAssetBestEffortAsync(string publicId, CancellationToken ct)
    {
        try
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            await _cloudinary.DestroyAsync(deletionParams);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cloudinary delete failed for PublicId {PublicId}", publicId);
        }
    }
}