using Microsoft.AspNetCore.Mvc;

namespace IsssueTracker.Api.Catalog;

public class Api : ControllerBase
{
    [HttpPost("/catalog")]
    public async Task<ActionResult> AddACatalogItemAsync(
        [FromBody] CreateCatalogItemRequest request,
        CancellationToken token)
    {
        //get the json data they sent and look at it. Is it cool?
        //If not, send them an error (400, with some details)
        //if it is cool, maybe save it to a db or something?
        //and what are we going to return
        var response = new CatalogItemResponse(Guid.NewGuid(), request.Title, request.Description);
        return Ok();
    }
}
