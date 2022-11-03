using System.Net;
using System.Text;
using System.Text.Json;
using EntityFrameworkRelationships.Web.DTOs;

namespace EntityFrameworkRelationships.Tests.IntegrationTests;

public class AuthorIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly JsonSerializerOptions _options;
    private readonly HttpClient _httpClient;
    private const string _basePath = "/api/authors";

    public AuthorIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();

        _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region GetAll

    [Fact]
    public async Task GeAll_ShouldReturnAllItems()
    {
        var response = await _httpClient.GetAsync($"{_basePath}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<List<AuthorDto>>(payloadString, _options);
        var item = payloadObject?.SingleOrDefault(x => x.Name == "Sheldon Cooper");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Sheldon.Cooper@email.com", item?.Email);
    }

    #endregion

    #region GeById

    [Fact]
    public async Task GeById_ShouldReturnOnlyOneItem()
    {
        var itemsResponse = await _httpClient.GetAsync($"{_basePath}");
        var itemsPayloadString = await itemsResponse.Content.ReadAsStringAsync();
        var itemsPayloadObject = JsonSerializer.Deserialize<List<AuthorDto>>(itemsPayloadString, _options);
        var item = itemsPayloadObject?.SingleOrDefault(x => x.Name == "Sheldon Cooper");

        var response = await _httpClient.GetAsync($"{_basePath}/{item?.Id}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<AuthorDto>(payloadString, _options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Sheldon Cooper", payloadObject?.Name);
        Assert.Equal("Sheldon.Cooper@email.com", payloadObject?.Email);
    }

    [Fact]
    public async Task GeById_ShouldReturnNotFoundWhenIdNotExists()
    {
        var id = new Guid();
        var response = await _httpClient.GetAsync($"{_basePath}/{id.ToString()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GeById_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string id = "not-valid-id";
        var response = await _httpClient.GetAsync($"{_basePath}/{id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ShouldCreateAnItem()
    {
        var newItem = new
        {
            Name = "Brian O'Conner",
            Email = "Brian.OConner@email.com"
        };

        var payload = JsonSerializer.Serialize(newItem, _options);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_basePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<AuthorDto>(payloadString, _options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, payloadObject?.Id);
        Assert.Equal(newItem.Name, payloadObject?.Name);
        Assert.Equal(newItem.Email, payloadObject?.Email);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsData))]
    public async Task Create_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _options);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_basePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(FieldsAreInvalidData))]
    public async Task Create_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _options);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_basePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion
    
    #region Update

    [Fact]
    public async Task Update_ShouldUpdateAnItem()
    {
        // Create a new item
        var newItem = new
        {
            Name = "Author Update",
            Email = "Author.Update@email.com"
        };

        var newItemPayload = JsonSerializer.Serialize(newItem, _options);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{_basePath}", newItemHttpContent);
        var newItemPayloadString = await newItemResponse.Content.ReadAsStringAsync();
        var newItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(newItemPayloadString, _options);
        var newItemId = newItemPayloadObject?.Id.ToString();

        // Update the created item
        var updatedItem = new
        {
            Name = "Author Updated",
            Email = "Author.Updated@email.com"
        };

        var payload = JsonSerializer.Serialize(updatedItem, _options);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{_basePath}/{newItemId}", httpContent);

        // Ensure the item has been changed getting the item from the DB
        var updatedItemResponse = await _httpClient.GetAsync($"{_basePath}/{newItemId}");
        var updatedItemPayloadString = await updatedItemResponse.Content.ReadAsStringAsync();
        var updatedItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(updatedItemPayloadString, _options);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(updatedItem.Name, updatedItemPayloadObject?.Name);
        Assert.Equal(updatedItem.Email, updatedItemPayloadObject?.Email);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFoundWhenIdNotExists()
    {
        var id = new Guid();
        var itemToUpdate = new
        {
            Name = "Author Second",
            Email = "Author.Second@email.com"
        };

        var payload = JsonSerializer.Serialize(itemToUpdate, _options);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_basePath}/{id.ToString()}", httpContent);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsData))]
    public async Task Update_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _options);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var id = new Guid();
        var response = await _httpClient.PutAsync($"{_basePath}/{id.ToString()}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(FieldsAreInvalidData))]
    public async Task Update_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _options);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var id = new Guid();
        var response = await _httpClient.PutAsync($"{_basePath}/{id.ToString()}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_ShouldRemoveOnlyOneItem()
    {
        // Create a new item
        var newItem = new
        {
            Name = "Item Remove",
            Email="Item.Remove@email.com"
        };

        var newItemPayload = JsonSerializer.Serialize(newItem, _options);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{_basePath}", newItemHttpContent);
        var newItemPayloadString = await newItemResponse.Content.ReadAsStringAsync();
        var newItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(newItemPayloadString, _options);

        // Remove the created item
        var response = await _httpClient.DeleteAsync($"{_basePath}/{newItemPayloadObject?.Id}");

        // Ensure the item has been deleted trying to get the item from the DB
        var deletedItemResponse = await _httpClient.GetAsync($"{_basePath}/{newItemPayloadObject?.Id}");

        Assert.Equal(HttpStatusCode.Created, newItemResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deletedItemResponse.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFoundWhenIdNotExists()
    {
        var id = new Guid();
        var response = await _httpClient.DeleteAsync($"{_basePath}/{id.ToString()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string id = "not-valid-id";
        var response = await _httpClient.DeleteAsync($"{_basePath}/{id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    public static TheoryData<string[], object> MissingRequiredFieldsData => new()
    {
        {new[] {"The Name field is required.", "The Email field is required."}, new { }},
        {new[] {"The Name field is required."}, new {Email = "user@email.com"}},
        {new[] {"The Email field is required."}, new {Name = "Author New"}},
        {new[] {"The Name field is required."}, new {Name = "", Email = "user@email.com"}},
        {new[] {"The Name field is required."}, new {Name = " ", Email = "user@email.com"}},
    };

    public static TheoryData<string[], object> FieldsAreInvalidData => new()
    {
        {new[] {"The Name field is invalid."}, new {Name = "N@me", Email = "user@email.com"}},
        {new[] {"The Name field is invalid.", "The Email field is invalid."}, new {Name = "Author 1", Email = "user @email.com"}},
        {new[] {"The Email field is invalid."}, new {Name = "Author New", Email = "useremail.com"}},
        {new[] {"The Email field is invalid."}, new {Name = "Author New", Email = "user@email"}}
    };
}