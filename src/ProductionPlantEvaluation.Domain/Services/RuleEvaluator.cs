using System;
using System.Collections.Generic;
using System.Text;
using ProductionPlantEvaluation.Domain.DTO;
using ProductionPlantEvaluation.Domain.Interfaces;


namespace ProductionPlantEvaluation.Domain.DTO;

public class RuleEvaluator : IRuleEvaluator
{
    public bool EvaluateConditions(
        IEnumerable<Condition> conditions,
        OrderEvaluationContext context, ICollection<ConditionEvaluation>? trace = null)
    {
        foreach (var condition in conditions.OrderBy(x => x.Sequence))
        {
            var actualValue = GetFieldValue(
                condition.Field, context);

            if (actualValue == null)
                return false;

            var isMatched = Compare(
                actualValue,
                condition.Value,
                condition.RulesCon);

            if (!isMatched)
                return false;
        }

        return true;
    }

    private object? GetFieldValue(
        string field,
        OrderEvaluationContext context)
    {
        return field.ToLowerInvariant() switch
        {
            "publishernumber" => context.PublisherNumber,
            "ordermethod" => context.OrderMethod,
            "isocountry" => context.Countries,
            "bindtypecode" => context.BindTypeCodes,
            "printquantity" => context.TotalPrintQuantity,
            _ => null
        };
    }

    private bool Compare(
        object actualValue,
        string expectedValue,
        Rules operation)
    {
        switch (actualValue)
        {
            case string actual:
                return operation == Rules.Equals
                    && string.Equals(
                        actual,
                        expectedValue,
                        StringComparison.OrdinalIgnoreCase);

            case IEnumerable<string> actualValues:
                return operation == Rules.Equals
                    && actualValues.Any(x =>
                        string.Equals(
                            x,
                            expectedValue,
                            StringComparison.OrdinalIgnoreCase));

            case int actual:
                if (!int.TryParse(expectedValue, out var expected))
                    return false;

                return operation switch
                {
                    Rules.Equals =>
                        actual == expected,

                    Rules.LessThanOrEqual =>
                        actual <= expected,

                    Rules.GreaterThanOrEqual =>
                        actual >= expected,

                    _ => false
                };

            default:
                return false;
        }
    }
}
