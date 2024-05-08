using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Status;

public class Api(ILogger<Api> logger) : ControllerBase
{
    [HttpGet("/status")]
    public async Task<ActionResult> GetTheStatus(CancellationToken token)
    {
        //some real work to await
        logger.LogInformation("Start the call");
        await Task.Delay(3000, token); //api call, db lookup
        var response = new StatusResponseModel("Looks Good", DateTimeOffset.Now);
        logger.LogInformation("Finish the call");
        return Ok(response);
    }

    public record StatusResponseModel(string Message, DateTimeOffset CheckedAt);
}
