using ProductionPlantEvaluation.Domain.DTO;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace ProductionPlantEvaluation.UnitTests;

public class UnitTest1
{
    [Fact]
    public async Task GetRulesetsAsync_RepeatedCalls_UsesCachedRulesets()
    {
        var repository = new CountingRuleRepository();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var ruleCache = new RuleCacheService(cache, repository);

        var firstResult = await ruleCache.GetRulesetsAsync(CancellationToken.None);
        var secondResult = await ruleCache.GetRulesetsAsync(CancellationToken.None);

        Assert.Same(firstResult, secondResult);
        Assert.Equal(1, repository.CallCount);
    }

    [Fact]
    public void Evaluate_Equals_ShouldReturnTrue()
    {
        var evaluator = new RuleEvaluator();

        var context = new OrderEvaluationContext
        {
            PublisherNumber = "99999"
        };

        var conditions = new List<Condition>
    {
        new Condition
        {
            Field = "PublisherNumber",
            RulesCon = Rules.Equals,
            Value = "99999"
        }
    };

        var result = evaluator.EvaluateConditions(
            conditions, context);

        Assert.True(result);
    }

internal sealed class CountingRuleRepository : IRuleRepository
{
    public int CallCount { get; private set; }

    public Task<List<Ruleset>> GetActiveRulesetsAsync(CancellationToken cancellationToken)
    {
        CallCount++;
        return Task.FromResult(new List<Ruleset>());
    }
}

    [Fact]
    public void Evaluate_LessThanOrEqual_ShouldReturnTrue()
    {
        var evaluator = new RuleEvaluator();

        var context = new OrderEvaluationContext
        {
            TotalPrintQuantity = 10
        };

        var conditions = new List<Condition>
    {
        new Condition
        {
            Field = "PrintQuantity",
            RulesCon = Rules.LessThanOrEqual,
            Value = "20"
        }
    };

        var result = evaluator.EvaluateConditions(
            conditions, context);

        Assert.True(result);
    }

    [Fact]
    public void Evaluate_GreaterThanOrEqual_ShouldReturnTrue()
    {
        var evaluator = new RuleEvaluator();

        var context = new OrderEvaluationContext
        {
            TotalPrintQuantity = 25
        };

        var conditions = new List<Condition>
    {
        new Condition
        {
            Field = "PrintQuantity",
            RulesCon = Rules.GreaterThanOrEqual,
            Value = "20"
        }
    };

        var result = evaluator.EvaluateConditions(
            conditions, context);

        Assert.True(result);
    }

    [Theory]
    [InlineData("US", "US", true)]
    [InlineData("CA", "CA", true)]
    [InlineData("US", "CA", false)]
    public void Evaluate_IsoCountry_ShouldMatchSeededCountryCondition(
        string country,
        string expectedCountry,
        bool expected)
    {
        var evaluator = new RuleEvaluator();
        var context = new OrderEvaluationContext
        {
            Countries = new[] { country }
        };
        var conditions = new List<Condition>
        {
            new()
            {
                Field = "IsoCountry",
                RulesCon = Rules.Equals,
                Value = expectedCountry
            }
        };

        var result = evaluator.EvaluateConditions(conditions, context);

        Assert.Equal(expected, result);
    }
}
