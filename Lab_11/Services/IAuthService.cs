using Lab_11.Models.Domain;

namespace Lab_11.Services;

public interface IAuthService
{
    public Task UpdateEmployeeRefreshToken(AppUser user, DateTime addDays);
    public Task<AppUser?> GetEmployeeByRefreshToken(string refreshTokenRefreshToken);
    public Task AddUser(AppUser user);
    public Task<AppUser?> GetUserByLogin(string modelLogin);
    
}