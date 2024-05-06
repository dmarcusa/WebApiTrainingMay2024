using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace IsssueTracker.Api.Catalog;

public class Api(IValidator<CreateCatalogItemRequest> validator, IDocumentSession session) : ControllerBase
{
    [HttpPost("/catalog")]
    public async Task<ActionResult> AddACatalogItemAsync(
        [FromBody] CreateCatalogItemRequest request,
        CancellationToken token)
    {
        var validation = await validator.ValidateAsync(request, token);
        if (!validation.IsValid)
        {
            //dictionary<string,string[]>
            return this.CreateProblemDetailsForModelValidation(
                "Cannot Add Catalog Item", validation.ToDictionary());
            //BadRequest(validation.ToDictionary());
        }

        var entityToSave = new CatalogItem(Guid.NewGuid(), request.Title, request.Description, "todo", DateTimeOffset.Now);
        session.Store(entityToSave);
        await session.SaveChangesAsync(); //do the actual work
        //get the json data they sent and look at it. Is it cool?
        //If not, send them an error (400, with some details)
        //if it is cool, maybe save it to a db or something?
        //we have to create an entity to add it to the request, and add it to the db etc
        //save it
        //and what are we going to return
        //return to them, from the entity the thing we are giving the, as receipt

        var response = new CatalogItemResponse(entityToSave.Id, request.Title, request.Description);
        return Ok(response); //I have stored this thing in a way I can get to it, it is now part of the collection
    }

    [HttpGet("/catalog")]
    public async Task<ActionResult> GetCatalogItemAsync(
    [FromBody] CreateCatalogItemRequest request,
    CancellationToken token)
    {
        //get the json data they sent and look at it. Is it cool?
        //If not, send them an error (400, with some details)
        //if it is cool, maybe save it to a db or something?
        //and what are we going to return
        var response = new CatalogItemResponse(Guid.NewGuid(), request.Title, request.Description);
        return Ok(response);
    }
}
