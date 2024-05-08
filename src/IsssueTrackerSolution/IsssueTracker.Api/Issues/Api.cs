using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Issues;

public class Api : ControllerBase
{
    [HttpPost("/catalog/{catalogItemId:guid}/issues")]
    [Authorize]
    public async Task<ActionResult> AddAnIssueAsync(Guid catalogItemId)
    {
        return Ok();
    }
}
