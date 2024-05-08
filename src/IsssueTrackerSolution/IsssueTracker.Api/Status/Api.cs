using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api.Status;

/// <summary>
/// This is the Api for the status stuff
/// </summary>
/// <param name="logger">use to log some stuff</param>
public class Api(ILogger<Api> logger) : ControllerBase
{
    /// <summary>
    /// Use this to get the status of the api
    /// </summary>
    /// <param name="token">cancelation token</param>
    /// <returns>status stuff</returns>
    /// <response code="200"> The status of the system including...</response>
    [HttpGet("/status")]
    [Produces("application/json")]
    public async Task<ActionResult<StatusResponseModel>> GetTheStatus(CancellationToken token)
    {
        //some real work to await
        logger.LogInformation("Start the call");
        await Task.Delay(3000, token); //api call, db lookup
        //var response = new StatusResponseModel("Looks Good", DateTimeOffset.Now);

        var response = new StatusResponseModel
        {
            Message = "Looks Good",
            CheckedAt = DateTime.UtcNow,
        };
        logger.LogInformation("Finished the call");
        return Ok(response);
    }

    [HttpPost("/status")]
    public async Task<ActionResult> AddANewStatusMessage([FromBody] StatusRequestModel request)
    {
        return Ok();
    }

    public record StatusRequestModel
    {
        [Required, MinLength(5), MaxLength(30)]
        public string Message { get; set; } = string.Empty;
    }

    //public record StatusResponseModel(string Message, DateTimeOffset CheckedAt);

    public record StatusResponseModel
    {
        [Required, MinLength(5), MaxLength(30)]
        public string Message { get; init; } = string.Empty;
        [Required]
        public DateTimeOffset CheckedAt { get; init; }

        public string? CheckedBy { get; init; }
    }
}
