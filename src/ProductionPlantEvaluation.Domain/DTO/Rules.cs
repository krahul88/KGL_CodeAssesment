using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Domain.DTO
{
    public enum Rules
    {
        Equals,
        LessThanOrEqual,
        GreaterThanOrEqual
    }
    public class Condition
    {
        public int ConditionId { get; set; }

        public string Field { get; set; } = string.Empty;

        public Rules RulesCon { get; set; }

        public string Value { get; set; } = string.Empty;

        public int Sequence { get; set; }
    }
    public record ConditionEvaluation(
    string Field,
    string Operator,
    string ExpectedValue,
    string ActualValue,
    bool Matched);

    public class Rule
    {
        public int RuleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Sequence { get; set; }

        public string ProductionPlant { get; set; } = string.Empty;

        public List<Condition> Conditions { get; set; } = new();
    }

    public class Ruleset
    {
        public int RSId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Priority { get; set; }

        public bool IsEnabled { get; set; }

        public List<Condition> Conditions { get; set; } = new();

        public List<Rule> Rules { get; set; } = new();
    }
    public class OrderEvaluationContext
    {
        public string PublisherNumber { get; init; } = string.Empty;
        public string OrderMethod { get; init; } = string.Empty;
        public IReadOnlyCollection<string> Countries { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> BindTypeCodes { get; init; } = Array.Empty<string>();
        public int TotalPrintQuantity { get; init; }
    }
    public class EvaluationLog
    {
        public long EvaluationLogId { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string InputJson { get; set; } = string.Empty;
        public bool Matched { get; set; }
        public string? MatchedRuleset { get; set; }
        public string? MatchedRule { get; set; }
        public string? ProductionPlant { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ConditionTraceJson { get; set; } = "[]";
        public DateTime EvaluatedAtUtc { get; set; }
    }
}
