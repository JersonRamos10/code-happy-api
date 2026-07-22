using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace codeHappy.Business.Services
{
    public class BlockService(CodeHappyContext context) : IBlocksService
    {
        public async Task<BlocksResponse> CreateBlockAsync(Guid userId, Guid snippetId, CreateBlockRequest request)
        {
            var snippet = await context.Snippets
                    .FindAsync(snippetId)
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

            await context.AddAsync(block);
            await context.SaveChangesAsync();

            return MapToBlocksResponse(block);
        }

        public async Task DeleteBlock(Guid UserId, Guid snippetId, Guid blockId)
        {
            var snippet = await context.Snippets
                              .Include(s => s.Blocks)
                              .FirstOrDefaultAsync(s => s.Id == snippetId)
                              ?? throw new NotFoundException("Snippet", snippetId);

            if(snippet.OwnerId != UserId)
                throw new ForbiddenException();
            
            var block = await context.Blocks
                  .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId == snippetId) 
                        ?? throw new NotFoundException("Block", blockId); 
           
            context.Remove(block);
            
            await context.SaveChangesAsync();

        }

        public async Task ReorderBlocks(Guid UserId, Guid snippetId, List<ReorderBlockRequest> reorderBlocks)
        {
            var snippet = await context.Snippets
                              .Include(s => s.Blocks)
                              .FirstOrDefaultAsync(s => s.Id == snippetId) 
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
            
            await context.SaveChangesAsync();  
        }

        public async Task UpdateBlockAnnotations(Guid UserId, Guid snippetId, Guid blockId, List<CreateAnnotationRequest>? request)
        {
            var snippet = await context.Snippets.FindAsync(snippetId)       
                        ?? throw new NotFoundException("Snippet", snippetId);        

            if (snippet.OwnerId != UserId)                                   
                throw new ForbiddenException();                              

            var block = await context.Blocks                                
                            .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId ==  
                                snippetId) ?? throw new NotFoundException("Block", blockId);
            
            block.Annotations = request is null ? [] : request.Select(a => MapToCodeAnnotation(a)).ToList();

            await context.SaveChangesAsync();
        }

        public async Task UpdateBlockContentAsync(Guid userId, Guid snippetId, Guid blockId, UpdateBlockRequest request)
        {
        
            var snippet = await context.Snippets.FindAsync(snippetId)
                ?? throw new NotFoundException("Snippet", snippetId);

            if (snippet.OwnerId != userId)
                throw new ForbiddenException();

            var block = await context.Blocks
                .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId == snippetId)
                ?? throw new NotFoundException("Block", blockId);

            block.Title = request.Title ?? block.Title;
            block.Content = request.Content ?? block.Content;
            block.Language = request.Language ?? block.Language;

            await context.SaveChangesAsync();
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
                UpdateAt: block.UpdatedAt,
                ImageMetadata: block.ImageMetadata is null ? null : MapToImageMetadataResponse(block.ImageMetadata)
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

        private static ImageMetadataResponse MapToImageMetadataResponse(ImageMetadata imageMetadata)
        {
            return new ImageMetadataResponse(
                PublicId: imageMetadata.PublicId,
                SecureUrl: imageMetadata.SecureUrl,
                Width: imageMetadata.Width,
                Height: imageMetadata.Height,
                Format: imageMetadata.Format,
                Bytes: imageMetadata.Bytes,
                Alt: imageMetadata.Alt,
                BucketPath: imageMetadata.BucketPath
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
