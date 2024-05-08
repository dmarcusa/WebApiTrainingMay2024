using Marten;
using System.Security.Claims;

namespace IssueTracker.Api.Shared;

public class UserIdentityService(IHttpContextAccessor httpContextAccessor, IDocumentSession session)
{
    public Task<string> GetUserSubAsync()
    {
        //Do not cache httpContextAccessor.HttpContext?.User because of threading issues
        var user = httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new ChaosException("Should not run outside of an http request");
        var userId = user.Value;
        return Task.FromResult(userId);
    }

    public async Task<UserInformation> GetUserInformationAsync()
    {
        //I got their subclaim
        var sub = await GetUserSubAsync();
        var user = await session.Query<UserInformation>().Where(u => u.Sub == sub).SingleOrDefaultAsync();
        if (user is null)
        {
            var newInformation = new UserInformation(Guid.NewGuid(), sub);
            session.Store(newInformation);
            await session.SaveChangesAsync();
            return newInformation;
        }
        else
        {
            return user;
        }
    }
}

public class ChaosException(string message) : Exception(message)
{
}

public record UserInformation(Guid Id, string Sub);