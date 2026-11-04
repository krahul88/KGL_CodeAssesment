using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace ProductionPlantEvaluation.IntegrationTests;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    public UnitTest1(WebApplicationFactory<Program> test)
    { _httpClient = test.CreateClient(); }

    [Fact]
    public async Task Evaluate_InvalidRequest_ReturnsBadRequest()
    {
        var response = await _httpClient.PostAsJsonAsync("/api/evaluate",
            new
            {
                orderId = ""
            });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
