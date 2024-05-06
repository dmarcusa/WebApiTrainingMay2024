using Alba;
using Alba.Security;
using IsssueTracker.Api.Catalog;
using System.Security.Claims;

namespace IssueTracker.ContractTests.Catalog;
public class CatalogTests
{
    [Fact]
    public async Task CanAddAnItemToTheCatalog()
    {
        var stubbedToken = new AuthenticationStub()
            .With(ClaimTypes.NameIdentifier, "carl@aol.com") // Sub claim
            .With(ClaimTypes.Role, "SotwareCenter");  // this adds this role.

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A text editor for windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBeOk();
        });

        var actualResponse = await response.ReadAsJsonAsync<CatalogItemResponse>();

        Assert.NotNull(actualResponse);
        Assert.Equal("Notepad", actualResponse.Title);
        Assert.Equal("A text editor for windows", actualResponse.Description);
    }

    [Fact]
    public async Task OnlySoftwareCenterPeopleCanAddThings()
    {
        var stubbedToken = new AuthenticationStub()
           .With(ClaimTypes.NameIdentifier, "carl@aol.com") // Sub claim
           .With(ClaimTypes.Role, "TacoNose");  // this adds this role.

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBe(403); // Unauthorized
        });


    }

}
