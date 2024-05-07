﻿using FluentValidation;
using Riok.Mapperly.Abstractions;

namespace IsssueTracker.Api.Catalog;

//"Models" are the things that leave or arrive from outside our service boundary
//they are C# types that weill be desearilize or serialize from json coming ontp our out of api
public record CreateCatalogItemRequest(string Title, string Description);

public static class ModelExtensions
{
    //public static CatalogItemResponse MapToResponse(this CatalogItem item)
    //{
    //    return new CatalogItemResponse(item.Id, item.Title, item.Description);
    //}
    public static CatalogItem MapToCatalogItem(this CreateCatalogItemRequest request, string addedBy)
    {
        return new CatalogItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            AddedBy = addedBy,
            CreatedAt = DateTimeOffset.Now

        };
    }
}

[Mapper]
public static partial class CatalogMappers
{
    public static partial CatalogItemResponse MapToResponse(this CatalogItem item);
}

public class CreateCatalogItemRequestValidator : AbstractValidator<CreateCatalogItemRequest>
{
    public CreateCatalogItemRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("We need a title");
        RuleFor(r => r.Title).MinimumLength(5).MaximumLength(256).WithMessage("We need a title");
        RuleFor(r => r.Description).NotEmpty().MaximumLength(2024);
    }
}

public record CatalogItemResponse(Guid Id, string Title, string Description);


