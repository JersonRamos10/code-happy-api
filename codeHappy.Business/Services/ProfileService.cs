using Microsoft.EntityFrameworkCore;
using codeHappy.Business.Dtos.Profile;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using codeHappy.Business.Exceptions;

namespace codeHappy.Business.Services;

public class ProfileService : IProfileService
{
    private readonly CodeHappyContext _context;
    public ProfileService(CodeHappyContext context)
    {
        _context = context;
    }
    // Busca el perfil por ID. Lanza ProfileNotFoundException si no existe.
    public async Task<UserProfileResponse> GetUserbyIdAsync(Guid userId)
    {
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == userId)
                ?? throw new NotFoundException("Profile", userId);

        return new UserProfileResponse(
            profile.Id,
            profile.UserName,
            profile.Email,
            profile.AvatarUrl
        );
    }

    // Crea el perfil si no existe (upsert simplificado). Se ejecuta en cada login del usuario.
    public async Task SyncProfileAsync(Guid userId, string email, string userName, string displayName)
    {
        var profileExist = await _context.Profiles.FindAsync(userId);

        if (profileExist == null)
        {
            var profile = new Profile
            {
                Id = userId,
                UserName = userName,
                Email = email,
                DisplayName = displayName,
            };

            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
    }
}
