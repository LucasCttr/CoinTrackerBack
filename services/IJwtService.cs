using coninTracker.API.Models;

namespace coninTracker.API.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}