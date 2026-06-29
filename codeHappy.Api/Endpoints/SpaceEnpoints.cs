using codeHappy.Business.Dtos.Spaces;
using codeHappy.Business.Interfaces;
using FluentValidation;

namespace codeHappy.Api.Endpoints;

public static class SpaceEndpoints
{
    public static void MapSpaceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/spaces").RequireAuthorization();


        // POST /spaces — creates a new space. Returns 201 with the created space.
        group.MapPost("/", async (
            CreateSpaceRequest request,
            IValidator<CreateSpaceRequest> validator,
            ISpaceService service,
            ICurrentUserService current) =>
        {
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var space = await service.CreateSpaceAsync(Guid.Parse(userId), request);

            return Results.Created($"/spaces/{space.Id}", space);
        });


        // GET /spaces — returns all spaces owned by the authenticated user.
        group.MapGet("/", async (ISpaceService service, ICurrentUserService current) =>
        {

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var spaces = await service.GetAllSpacesAsync(Guid.Parse(userId));

            return Results.Ok(spaces);
        });

        // DELETE /spaces/{id} — deletes the space. Returns 403 if the user is not the owner.
        group.MapDelete("/{id}", async (Guid id, ISpaceService service, ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.DeleteSpaceAsync(id, Guid.Parse(userId));

            return Results.NoContent();
        });

        // PUT /spaces/{id} — updates name and icon. Returns 403 if the user is not the owner.
        group.MapPut("/{id}", async (Guid id, UpdateSpaceRequest request, IValidator<UpdateSpaceRequest> validator, ISpaceService service, ICurrentUserService current) =>
        {
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.UpdateSpaceAsync(id, Guid.Parse(userId), request);

            return Results.NoContent();
        });

        // PATCH /spaces/{id}/touch — updates LastAccessedAt to track recently opened spaces.
        group.MapPatch("/{id}/touch", async (Guid id, ISpaceService service, ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.TouchSpaceAsync(id, Guid.Parse(userId));

            return Results.NoContent();
        });
    }
}