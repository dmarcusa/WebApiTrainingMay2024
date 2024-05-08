using FluentValidation;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IssueTracker.Api.Catalog;

[Authorize(Policy = "IsSoftwareAdmin")]
[Route("/catalog")]
[Produces("application/json")]
public class ApiCommands(IValidator<CreateCatalogItemRequest> validator, IDocumentSession session) : ControllerBase
{
    /// <summary>
    /// Add an item to the software catalog
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userIdentityService"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="201"> The new software item</response>
    /// <response code="400"> An app problem respoinse</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CatalogItemResponse>> AddACatalogItemAsync(
       [FromBody] CreateCatalogItemRequest request,
       [FromServices] UserIdentityService userIdentityService,
       CancellationToken token)
    {
        var userId = await userIdentityService.GetUserSubAsync();
        var validation = await validator.ValidateAsync(request, token);
        if (!validation.IsValid)
        {
            return this.CreateProblemDetailsForModelValidation("Cannot Add Catalog Item", validation.ToDictionary());
        }

        var entityToSave = request.MapToCatalogItem(userId);

        session.Store(entityToSave);
        await session.SaveChangesAsync(token); // Do the actual work!

        var response = entityToSave.MapToResponse();
        return CreatedAtRoute("catalog#get-by-id", new { id = response.Id }, response);
        // part of this collection. 
    }
    [HttpDelete("{id:guid}")]

    public async Task<ActionResult> RemoveCatalogItemAsync(Guid id, CancellationToken token)
    {

        // see if the thing exists.
        var storedItem = await session.LoadAsync<CatalogItem>(id, token);
        if (storedItem != null)
        {
            var user = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            //if (storedItem.AddedBy != user.Value)
            //{
            //    return StatusCode(403);
            //}
            // if it does, do a "soft delete"
            storedItem.RemovedAt = DateTimeOffset.Now;
            session.Store(storedItem); // "Upsert"
            await session.SaveChangesAsync(token); // save it.
        }
        return NoContent();
    }

    // PUT //catalog/387...
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> ReplaceCatalogItemAsync(
        Guid id, [FromBody] ReplaceCatalogItemRequest request, CancellationToken token)
    {
        var item = await session.LoadAsync<CatalogItem>(id, token);

        if (item is null)
        {
            return NotFound();
        }
        if (id != request.Id)
        {
            return BadRequest("Ids don't match");
        }
        item.Title = request.Title;
        item.Description = request.Description;
        session.Store(item);
        await session.SaveChangesAsync(token);
        return Ok();
    }
}
