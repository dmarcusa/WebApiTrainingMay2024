using IssueTracker.Api.Catalog;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Issues;

public class Api(UserIdentityService userIdentityService, IDocumentSession session) : ControllerBase
{
    [HttpPost("/catalog/{catalogItemId:guid}/issues")]
    [Authorize]
    public async Task<ActionResult<UserIssueResponse>> AddAnIssueAsync(
        Guid catalogItemId,
        [FromBody] UserCreateIssueRequestModel request,
        CancellationToken token)
    {
        var software = await session.Query<CatalogItem>()
            .Where(c => c.Id == catalogItemId)
            .Select(c => new IssueSoftwareEmbeddedResponse(c.Id, c.Title, c.Description))
            .SingleOrDefaultAsync(token);
        if (software is null)
        {
            return NotFound("No Software With That Id In The Catalog.");
        }
        var userInfo = await userIdentityService.GetUserInformationAsync();
        var userUrl = Url.RouteUrl("users#get-by-id", new { id = userInfo.Id }) ?? throw new ChaosException("Need a good id");

        var entity = new UserIssue
        {
            Id = userInfo.Id,
            Status = IssueStatusType.Submitted,
            User = userUrl,
        };

        session.Store(entity);
        await session.SaveChangesAsync(token);

        var fakeResponse = new UserIssueResponse
        {
            Id = Guid.NewGuid(),
            Status = IssueStatusType.Submitted,
            User = "/users/" + userInfo.Id,
            Software = new IssueSoftwareEmbeddedResponse(catalogItemId, "Fake Title", "Fake Description")
        };
        return Ok(fakeResponse);
    }
}

public record UserCreateIssueRequestModel(string Description);

public record UserIssueResponse
{
    public Guid Id { get; set; }
    public string User { get; set; } = string.Empty;
    public IssueSoftwareEmbeddedResponse Software { get; set; }
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
}

public class UserIssue
{
    public Guid Id { get; set; }
    public string User { get; set; } = string.Empty;
    public IssueSoftwareEmbeddedResponse Software { get; set; }
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
    public DateTimeOffset DateTimeOffset { get; set; }
}

public record IssueSoftwareEmbeddedResponse(Guid Id, string Title, string Description);

public enum IssueStatusType { Submitted }