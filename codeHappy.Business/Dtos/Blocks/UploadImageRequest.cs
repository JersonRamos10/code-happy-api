namespace codeHappy.Business.Dtos;

public record UploadImageRequest(
    string FileName,
    string ContentType,
    long FileLength
    );