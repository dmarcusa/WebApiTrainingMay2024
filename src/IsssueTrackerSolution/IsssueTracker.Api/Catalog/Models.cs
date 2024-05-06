namespace IsssueTracker.Api.Catalog;

//"Models" are the things that leave or arrive from outside our service boundary
//they are C# types that weill be desearilize or serialize from json coming ontp our out of api
public record CreateCatalogItemRequest(string Title, string Description);

public record CatalogItemResponse(Guid id, string Title, string Description);
