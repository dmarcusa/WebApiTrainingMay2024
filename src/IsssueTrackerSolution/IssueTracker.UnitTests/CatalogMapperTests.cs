using IssueTracker.Api.Catalog;

namespace IssueTracker.UnitTests;
public class CatalogMapperTests
{
    [Fact]
    public void CanMapCatalogItems()
    {
        var entity = new CatalogItem { Id = Guid.NewGuid(), Title = "Stuff", AddedBy = "joe", CreatedAt = DateTimeOffset.Now };

        var mappedResponse = entity.MapToResponse();

        Assert.Equal("Stuff", mappedResponse.Title);
        Assert.Equal("", mappedResponse.Description);
        Assert.Equal(entity.Id, mappedResponse.Id);
    }
}
