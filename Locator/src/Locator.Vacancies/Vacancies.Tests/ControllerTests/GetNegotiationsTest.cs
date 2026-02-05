using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Vacancies.Web;

namespace Vacancies.Test.ControllerTests;

public class GetNegotiationsTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    
    public GetNegotiationsTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("invalid.jwt.token")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.invalid.signature")]
    public async Task Should_have_unauthorized_error_in_GetNegotiations_with_invalid_JwtToken(
        string invalidToken)
    {
        // Arrange
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            "https://localhost:5002/api/vacancies/negotiations");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");
        
        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    } 
}