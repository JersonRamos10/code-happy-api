namespace codeHappy.Business.Dtos.Blocks;

public record ImageMetadataResponse
(
     string PublicId,
     string SecureUrl,
     int? Width,
     int? Height,
     string? Format,
     long? Bytes,
     string? Alt,
     string? BucketPath
);
    