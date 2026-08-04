using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Mappers;
using codeHappy.Data.Context;
using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace codeHappy.Business.Services
{
    public class BlockService(CodeHappyContext context, IImagesService imagesService) : IBlocksService
    {
        public async Task<BlocksResponse> CreateBlockAsync(Guid userId, Guid snippetId, CreateBlockRequest request, CancellationToken ct)
        {
            var snippet = await context.Snippets
                    .FindAsync([snippetId], ct)
                    ?? throw new NotFoundException("Snippet", snippetId);

            if( snippet.OwnerId != userId )
                throw new ForbiddenException();

            var block = new Block
            {
                SnippetId =  snippetId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                Position = request.Position,
                Language = request.Language,
                Annotations = request.Annotations is null ? []
                    : request.Annotations.Select(a => MapToCodeAnnotation(a)).ToList(),
                ImageMetadata = request.Type != BlockType.Image ? null : new ImageMetadata
                {
                    PublicId = request.PublicId!,
                    SecureUrl = request.Content,
                    Width = request.Width,
                    Height = request.Height,
                    Format = request.Format,
                    Bytes = request.Bytes
                }
            };

            await context.AddAsync(block, ct);
            await context.SaveChangesAsync(ct);

            return MapToBlocksResponse(block);
        }

        public async Task DeleteBlock(Guid UserId, Guid snippetId, Guid blockId, CancellationToken ct)
        {
            var snippet = await context.Snippets
                              .Include(s => s.Blocks)
                              .FirstOrDefaultAsync(s => s.Id == snippetId, ct)
                              ?? throw new NotFoundException("Snippet", snippetId);

            if(snippet.OwnerId != UserId)
                throw new ForbiddenException();

            var block = await context.Blocks
                  .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId == snippetId, ct)
                        ?? throw new NotFoundException("Block", blockId);

            context.Remove(block);

            await context.SaveChangesAsync(ct);

            if (block.ImageMetadata is not null)
                await imagesService.DestroyAssetBestEffortAsync(block.ImageMetadata.PublicId, ct);
        }

        public async Task ReorderBlocks(Guid UserId, Guid snippetId, List<ReorderBlockRequest> reorderBlocks, CancellationToken ct)
        {
            var snippet = await context.Snippets
                              .Include(s => s.Blocks)
                              .FirstOrDefaultAsync(s => s.Id == snippetId, ct)
                              ?? throw new NotFoundException("Snippet", snippetId);

            if(snippet.OwnerId != UserId)
                throw new ForbiddenException();


            foreach (var item in reorderBlocks)
            {
                var b = snippet.Blocks
                            .FirstOrDefault(b => b.Id == item.Id)
                             ?? throw new NotFoundException("Block", item.Id);

                b.Position = item.Position;
            }

            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateBlockAnnotations(Guid UserId, Guid snippetId, Guid blockId, List<CreateAnnotationRequest>? request, CancellationToken ct)
        {
            var snippet = await context.Snippets.FindAsync([snippetId], ct)
                        ?? throw new NotFoundException("Snippet", snippetId);

            if (snippet.OwnerId != UserId)
                throw new ForbiddenException();

            var block = await context.Blocks
                            .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId ==
                                snippetId, ct) ?? throw new NotFoundException("Block", blockId);

            block.Annotations = request is null ? [] : request.Select(a => MapToCodeAnnotation(a)).ToList();

            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateBlockContentAsync(Guid userId, Guid snippetId, Guid blockId, UpdateBlockRequest request, CancellationToken ct)
        {

            var snippet = await context.Snippets.FindAsync([snippetId], ct)
                ?? throw new NotFoundException("Snippet", snippetId);

            if (snippet.OwnerId != userId)
                throw new ForbiddenException();

            var block = await context.Blocks
                .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId == snippetId, ct)
                ?? throw new NotFoundException("Block", blockId);

            block.Title = request.Title ?? block.Title;
            block.Content = request.Content ?? block.Content;
            block.Language = request.Language ?? block.Language;

            await context.SaveChangesAsync(ct);
        }

        private static BlocksResponse MapToBlocksResponse(Block block)
        {
            return new BlocksResponse(
                Id: block.Id,
                Title: block.Title,
                Content: block.Content,
                Language: block.Language,
                Type: block.Type,
                Annotations: block.Annotations.Select(a => MapToAnnotationResponse(a)).ToList(),
                Position: block.Position,
                CreatedAt: block.CreatedAt,
                UpdatedAt: block.UpdatedAt,
                ImageMetadata: block.ImageMetadata is null ? null : ImageMetadataMapper.ToResponse(block.ImageMetadata)
            );
        }

        private static AnnotationResponse MapToAnnotationResponse(CodeAnnotation annotation)
        {
            return new AnnotationResponse(
                Id: annotation.Id,
                LineNumber: annotation.LineNumber,
                Text: annotation.Text
                );
        }


        private static CodeAnnotation MapToCodeAnnotation (CreateAnnotationRequest request)
        {
            return new CodeAnnotation
            {
                LineNumber = request.LineNumber,
                Text = request.Text
            };

        }


    }
}
