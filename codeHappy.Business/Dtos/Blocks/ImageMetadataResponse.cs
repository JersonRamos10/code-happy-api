namespace codeHappy.Business.Dtos.Blocks;

public record ImageMetadataResponse
(
     int? Width,
     int? Height,
     string? Alt,
     string? BucketPath 
);
    