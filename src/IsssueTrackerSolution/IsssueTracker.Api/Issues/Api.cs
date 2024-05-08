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
    public async Task<ActionResult<UserIssueResponse>> AddAnIssueAsync(Guid catalogItemId, [FromBody] UserCreateIssueRequestModel request)
    {
        var softwareExists = await session.Query<CatalogItem>().Where(c => c.Id == catalogItemId).AnyAsync();
        if (!softwareExists)
        {
            return NotFound("No Software With That Id In The Catalog.");
        }
        var userInfo = await userIdentityService.GetUserInformationAsync();

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

public record IssueSoftwareEmbeddedResponse(Guid Id, string Title, string Description);

public enum IssueStatusType { Submitted }