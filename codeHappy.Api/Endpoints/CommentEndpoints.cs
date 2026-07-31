using codeHappy.Business.Dtos.Comments;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Validators.Comments;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace codeHappy.Api.Endpoints;

public static class CommentEndpoints 
{
    public static void MapCommentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/snippets/{snippetId}/comments").RequireAuthorization();

        group.MapPost("/", async (
            ICurrentUserService userService,
            ICommentService service,
            IValidator<CreateCommentRequest> validator,
            [FromQuery] Guid? shareId,
            [FromRoute]Guid snippetId,
            [FromBody]  CreateCommentRequest request,
            CancellationToken ct
            ) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = userService.GetUserId();
            
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Results.Unauthorized();
            
            
            var createComment = await service.CreateCommentAsync(
                parsedUserId,
                request,
                shareId,
                snippetId,
                ct
                );
            
            return Results.Created($"/snippets/{snippetId}/comments/{createComment.Id}", createComment);
        });

        group.MapGet("/", async (
            ICurrentUserService userService, 
            ICommentService service,
            [FromRoute] Guid snippetId,
            [FromQuery] Guid? shareId,
            CancellationToken ct) =>
            {
                var userId = userService.GetUserId();

                if (!Guid.TryParse(userId, out var parsedUserId))
                    return Results.Unauthorized();

                var comments = await service.GetAllCommentsAsync(
                    parsedUserId,
                    snippetId,
                    shareId,
                    ct
                );

                return Results.Ok(comments);

            });


        group.MapPut("/{commentId}", async (
            ICurrentUserService userService,
            IValidator<UpdateCommentRequest> validator,
            UpdateCommentRequest request,
            ICommentService service,
            [FromRoute] Guid commentId,
            [FromRoute] Guid snippetId,
            [FromQuery] Guid? shareId,
            CancellationToken ct
            ) =>
        {
            var result = await validator.ValidateAsync(request, ct);
            
            if(!result.IsValid)           
                return Results.ValidationProblem(result.ToDictionary());
            
            var userId = userService.GetUserId();
            
            if(!Guid.TryParse(userId, out var parsedUserId ))
                return Results.Unauthorized();

            await service.UpdateCommentAsync (parsedUserId, snippetId, commentId, shareId, request, ct);
                
            return Results.NoContent();
            
        });

        group.MapDelete("/{commentId:guid}", async
        (
            ICurrentUserService userService,
            ICommentService service,
            [FromRoute] Guid commentId,
            CancellationToken ct
            ) =>
        {   
            var userId = userService.GetUserId();
            
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Results.Unauthorized();
            
            await service.DeleteCommentAsync(parsedUserId,commentId,ct);
                
                return Results.NoContent();
        });
    }
}