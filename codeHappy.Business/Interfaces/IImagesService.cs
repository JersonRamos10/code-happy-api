using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Interfaces;

public interface IImagesService
{
    Task<ImageMetadataResponse> UploadFileAsync(Guid userId, Guid snippetId, string fileName, Stream file, CancellationToken ct);

    Task DeleteFileAsync(Guid userId, string publicId, CancellationToken ct);

    // Best-effort Cloudinary destroy sin ownership check — para llamar desde flujos que ya validaron ownership (cleanup de Block/Snippet).
    Task DestroyAssetBestEffortAsync(string publicId, CancellationToken ct);
}
