using System.Net;
using System.Text;
using System.Text.Json;
using EntityFrameworkRelationships.Tests.Helpers;
using EntityFrameworkRelationships.Tests.IntegrationTests.Fixtures;
using EntityFrameworkRelationships.Web.DTOs;

namespace EntityFrameworkRelationships.Tests.IntegrationTests;

[Collection("SharedContext")]
public class AuthorIntegrationTests
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly HttpClient _httpClient;
    private const string BasePath = "/api/authors";

    public AuthorIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region GetAll

    [Fact]
    public async Task GeAll_ShouldReturnAllItems()
    {
        var response = await _httpClient.GetAsync($"{BasePath}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<List<AuthorDto>>(payloadString, _serializerOptions);

        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(payloadObject?.Count >= DbHelper.Authors.Count);
    }

    #endregion

    #region GeById

    [Fact]
    public async Task GeById_ShouldReturnOnlyOneItem()
    {
        var itemsResponse = await _httpClient.GetAsync($"{BasePath}");
        var itemsPayloadString = await itemsResponse.Content.ReadAsStringAsync();
        var itemsPayloadObject = JsonSerializer.Deserialize<List<AuthorDto>>(itemsPayloadString, _serializerOptions);
        var item = itemsPayloadObject?.SingleOrDefault(x => x.Name == "Sheldon Cooper");

        var response = await _httpClient.GetAsync($"{BasePath}/{item?.Id}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<AuthorDto>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Sheldon Cooper", payloadObject?.Name);
        Assert.Equal("Sheldon.Cooper@email.com", payloadObject?.Email);
    }

    [Fact]
    public async Task GeById_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GeById_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";
        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

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

        var payload = JsonSerializer.Serialize(newItem, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<AuthorDto>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, payloadObject?.Id);
        Assert.Equal(newItem.Name, payloadObject?.Name);
        Assert.Equal(newItem.Email, payloadObject?.Email);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsData))]
    public async Task Create_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(FieldsAreInvalidData))]
    public async Task Create_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
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

        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);
        var newItemPayloadString = await newItemResponse.Content.ReadAsStringAsync();
        var newItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(newItemPayloadString, _serializerOptions);
        var newItemId = newItemPayloadObject?.Id.ToString();

        // Update the created item
        var itemToUpdate = new
        {
            Name = "Author Updated",
            Email = "Author.Updated@email.com"
        };

        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{BasePath}/{newItemId}", httpContent);

        // Ensure the item has been changed getting the item from the DB
        var updatedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemId}");
        var updatedItemPayloadString = await updatedItemResponse.Content.ReadAsStringAsync();
        var updatedItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(updatedItemPayloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(itemToUpdate.Name, updatedItemPayloadObject?.Name);
        Assert.Equal(itemToUpdate.Email, updatedItemPayloadObject?.Email);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var itemToUpdate = new
        {
            Name = "Author Second",
            Email = "Author.Second@email.com"
        };

        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsData))]
    public async Task Update_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(FieldsAreInvalidData))]
    public async Task Update_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection, object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);
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

        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);
        var newItemPayloadString = await newItemResponse.Content.ReadAsStringAsync();
        var newItemPayloadObject = JsonSerializer.Deserialize<AuthorDto>(newItemPayloadString, _serializerOptions);

        // Remove the created item
        var response = await _httpClient.DeleteAsync($"{BasePath}/{newItemPayloadObject?.Id}");

        // Ensure the item has been deleted trying to get the item from the DB
        var deletedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemPayloadObject?.Id}");

        Assert.Equal(HttpStatusCode.Created, newItemResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deletedItemResponse.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

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
        {new[] {"The Email field is invalid."}, new {Name = "Author New", Email = "user.email.com"}},
        {new[] {"The Email field is invalid."}, new {Name = "Author New", Email = "user@email"}}
    };
}
