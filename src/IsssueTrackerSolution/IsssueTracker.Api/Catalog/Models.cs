using FluentValidation;

namespace IsssueTracker.Api.Catalog;

//"Models" are the things that leave or arrive from outside our service boundary
//they are C# types that weill be desearilize or serialize from json coming ontp our out of api
public record CreateCatalogItemRequest(string Title, string Description);

public class CreateCatalogItemRequestValidator : AbstractValidator<CreateCatalogItemRequest>
{
    public CreateCatalogItemRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().MinimumLength(5).MaximumLength(256);
        RuleFor(r => r.Description).NotEmpty().MaximumLength(2014);
    }
}

public record CatalogItemResponse(Guid Id, string Title, string Description);


