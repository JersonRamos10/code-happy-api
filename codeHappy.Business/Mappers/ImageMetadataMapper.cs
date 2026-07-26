using codeHappy.Data.Models;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Mappers;

public static class ImageMetadataMapper
{
    public static ImageMetadataResponse ToResponse(ImageMetadata image)
    {
        return new ImageMetadataResponse(
            PublicId: image.PublicId,
            SecureUrl: image.SecureUrl.ToString(),
            Width: image.Width,
            Height: image.Height,
            Format: image.Format,
            Bytes: image.Bytes,
            Alt: image.Alt,
            BucketPath: image.BucketPath
        );
    }
}