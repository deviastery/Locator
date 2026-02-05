using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Users.Contracts.Responses;

namespace Users.Tests.IntegrationTests;

public class DbTest : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private DockerWebApplicationFactoryFixture _factory;
    private HttpClient _client;
    
    public DbTest(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task Should_not_have_error_when_User_is_valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestBody = new
        {
            EmployeeId = 123, 
            FirstName = "Peter",
            Email = "example@mail.com",
        };
        string jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        // Act
        var createRequest = new HttpRequestMessage(
            HttpMethod.Post, 
            $"https://localhost:5000/api/users/{userId}")
        {
            Content = content,
        };
        createRequest.Headers.Add("User-Agent", "Locator/1.0");
        createRequest.Headers.Add("Api-Gateway", "Signed");

        var createResponse = await _client.SendAsync(createRequest, CancellationToken.None);
        
        var getRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/{userId}");
        getRequest.Headers.Add("User-Agent", "Locator/1.0");
        getRequest.Headers.Add("Api-Gateway", "Signed");

        var getResponse = await _client.SendAsync(getRequest, CancellationToken.None);

        string json = await getResponse.Content.ReadAsStringAsync(CancellationToken.None);
        var result = JsonSerializer.Deserialize<UserResponse>(json);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.User?.FirstName.Should().Be("Peter");
        result.User?.Email.Should().Be("example@mail.com");
        result.User?.EmployeeId.Should().Be("123");
    }
}