using Lab_11.Context;
using Lab_11.Helper;
using Lab_11.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lab_11.Services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;

    public AuthService(DatabaseContext context)
    {
        _context = context;
    }


    public async Task<AppUser?> GetUserByLogin(string modelLogin)
    {
        return await _context.AppUsers.FirstOrDefaultAsync(e => e.Login == modelLogin);
    }


    public async Task AddUser(AppUser user)
    {
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<AppUser?> GetEmployeeByRefreshToken(string refreshTokenRefreshToken)
    {
        return await _context.AppUsers.FirstOrDefaultAsync(e => e.RefreshToken == refreshTokenRefreshToken);
    }

    public async Task UpdateEmployeeRefreshToken(AppUser user, DateTime addDays)
    {
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = addDays;
        await _context.SaveChangesAsync();
    }
}