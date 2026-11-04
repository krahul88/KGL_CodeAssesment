using ProductionPlantEvaluation.Domain.DTO;

namespace ProductionPlantEvaluation.UnitTests;

public class UnitTest1
{
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
