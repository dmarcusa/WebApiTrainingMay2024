using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsssueTracker.Api.Catalog;

[Authorize]
[Route("/catalog")]
public class Api(IValidator<CreateCatalogItemRequest> validator, IDocumentSession session) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult> GetCatalogItemAsync(CancellationToken token)
    {
        var data = await session.Query<CatalogItem>()
            .Select(c => new CatalogItemResponse(c.Id, c.Title, c.Description))
            .ToListAsync(token);
        return Ok(new { data });
    }

    [HttpPost]
    //[Authorize(Roles = "SoftwareCenter")]
    [Authorize(Policy = "IsSoftwareAdmin")]
    public async Task<ActionResult> AddACatalogItemAsync(
        [FromBody] CreateCatalogItemRequest request,
        CancellationToken token)
    {
        var user = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        var userId = user.Value;
        var validation = await validator.ValidateAsync(request, token);
        if (!validation.IsValid)
        {
            //dictionary<string,string[]>
            return this.CreateProblemDetailsForModelValidation(
                "Cannot Add Catalog Item", validation.ToDictionary());
            //BadRequest(validation.ToDictionary());
        }

        var entityToSave = request.MapToCatalogItem(userId);
        //new CatalogItem(Guid.NewGuid(), request.Title, request.Description, userId, DateTimeOffset.Now);
        //new CatalogItem
        //{
        //    Id = Guid.NewGuid(),
        //    AddedBy = userId,
        //    CreatedAt = DateTime.UtcNow,
        //    Description = request.Description,
        //    Title = request.Title
        //};

        session.Store(entityToSave);
        await session.SaveChangesAsync(); //do the actual work
        //get the json data they sent and look at it. Is it cool?
        //If not, send them an error (400, with some details)
        //if it is cool, maybe save it to a db or something?
        //we have to create an entity to add it to the request, and add it to the db etc
        //save it
        //and what are we going to return
        //return to them, from the entity the thing we are giving the, as receipt

        var response = entityToSave.MapToResponse();
        //new CatalogItemResponse(entityToSave.Id, request.Title, request.Description);
        return Ok(response); //I have stored this thing in a way I can get to it, it is now part of the collection
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetCatalogItemByIdAsync(
        Guid id,
        CancellationToken token)
    {
        var response = await session.Query<CatalogItem>()
            .Where(c => c.Id == id)
            .Select(c => new CatalogItemResponse(c.Id, c.Title, c.Description))
            .SingleOrDefaultAsync(token);

        if (response == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(response);
        }
    }
}
