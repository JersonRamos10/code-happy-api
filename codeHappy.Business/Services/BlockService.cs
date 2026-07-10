using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using CodeHappy.Business.Dtos.Blocks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace codeHappy.Business.Services
{
    public class BlockService : IBlocksService
    {

        private readonly CodeHappyContext _context;

        public BlockService(CodeHappyContext context)
        {
            _context = context;
        }

        public async Task<BlocksResponse> CreateBlockAsync(Guid userId, Guid snippetId, CreateBlockRequest request)
        {
            var snippet = await _context.Snippets
                    .FindAsync(snippetId)
                    ?? throw new Exception("Snippet not found");

            if( snippet.OwnerId != userId )
                throw new ForbiddenException();

            var block = new Block
            {
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                Language = request.Language,
                Annotations = request.Annotations ?? new List<CodeAnnotation>(),
                ImageMetadata = request.Width.HasValue && request.Height.HasValue ? new ImageMetadata
                {
                    Width = request.Width.Value,
                    Height = request.Height.Value,
                    Alt = request.Alt,
                    BucketPath = request.BucketPath
                } : null,

            };

            await _context.AddAsync(block);
            await _context.SaveChangesAsync();

            return MapToBlocksResponse(block);
        }

        public Task DeleteBlock(Guid UserId, Guid snippetId, Guid blockId)
        {
            throw new NotImplementedException();
        }

        public Task reorderBlocks(Guid UserId, Guid snippetId, List<Guid> blockIds)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBlockAnnotations(Guid UserId, Guid snippetId, Guid blockId, BlockAnnotationsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBlockContentAsync(Guid userId, Guid snippetId, Guid blockId, UpdateBlockRequest request)
        {
          
            var snippet = await _context.Snippets.FindAsync(snippetId)
                ?? throw new NotFoundException("Snippet", snippetId);

            if (snippet.OwnerId != userId)
                throw new ForbiddenException();

            var block = await _context.Blocks
                .FirstOrDefaultAsync(b => b.Id == blockId && b.SnippetId == snippetId)
                 ?? throw new NotFoundException("Block", blockId);

            block.Title = request.Title ?? block.Title;
            block.Content = request.Content ?? block.Content;
            block.Language = request.Language ?? block.Language;

            await _context.SaveChangesAsync();
        }

        private static BlocksResponse MapToBlocksResponse(Block block)
        {
            return new BlocksResponse(
                Id: block.Id,
                Title: block.Title,
                Content: block.Content,
                Lenguaje: block.Language,
                Type: block.Type,
                CreatedAt: block.CreatedAt,
                UpdateAt: block.UpdatedAt,
                Annotations: block.Annotations?.ToList(),
                Position: block.Position,
                Width: block.ImageMetadata?.Width,
                Height: block.ImageMetadata?.Height,
                Alt: block.ImageMetadata?.Alt,
                BucketPath: block.ImageMetadata?.BucketPath
            );
        }
    }
}
