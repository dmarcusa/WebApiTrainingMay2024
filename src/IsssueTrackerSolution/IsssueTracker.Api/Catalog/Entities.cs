namespace IsssueTracker.Api.Catalog;

//public record CatalogItem(Guid Id, string Title, string Description, string AddedBy, DateTimeOffset CreatedAt);

public class CatalogItem
{
    //public CatalogItem(Guid id, string title, string description, string addedBy, DateTimeOffset createdAt)
    //{
    //    Id = id;
    //    Title = title;
    //    Description = description;
    //    AddedBy = addedBy;
    //    CreatedAt = createdAt;
    //}

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string AddedBy { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? RemovedAt { get; set; } = null;
}
