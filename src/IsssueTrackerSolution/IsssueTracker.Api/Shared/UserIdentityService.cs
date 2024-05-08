using System.Security.Claims;

namespace IssueTracker.Api.Shared;

public class UserIdentityService(IHttpContextAccessor httpContextAccessor)
{
    public Task<string> GetUserSubAsync()
    {
        //Do not cache httpContextAccessor.HttpContext?.User because of threading issues
        var user = httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new ChaosException("Should not run outside of an http request");
        var userId = user.Value;
        return Task.FromResult(userId);
    }
}

public class ChaosException(string message) : Exception(message)
{
}