using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductionPlantEvaluation.Infrastructure.Repositories;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ProductionPlantEvaluation.IntegrationTests;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _factory;
    public UnitTest1(WebApplicationFactory<Program> test)
    {
        _factory = test;
        _httpClient = test.CreateClient();
    }

    [Fact]
    public async Task Evaluate_InvalidRequest_ReturnsBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logCountBeforeRequest = await dbContext.EvaluationLogs.CountAsync();

        var response = await _httpClient.PostAsJsonAsync("/api/evaluate",
            new
            {
                orderId = ""
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(logCountBeforeRequest + 1, await dbContext.EvaluationLogs.CountAsync());
    }

    [Fact]
    public async Task Evaluate_MalformedJson_ReturnsBadRequest()
    {
        using var content = new StringContent("{ invalid", Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/evaluate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Evaluate_MissingBody_ReturnsBadRequest()
    {
        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/evaluate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
