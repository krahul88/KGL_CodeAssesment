using ProductionPlantEvaluation.Application.DTO;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Domain.DTO;
using ProductionPlantEvaluation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Application.Services
{
    public class OrderEvaluationService : IOrderEvaluationService
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IRuleEvaluator _ruleEvaluator;

        public OrderEvaluationService(IRuleRepository ruleRepository,
        IRuleEvaluator ruleEvaluator)
        {
            _ruleRepository = ruleRepository;
            _ruleEvaluator = ruleEvaluator;
        }

        public async Task<EvaluationResponse> EvaluateAsync(OrderRequest request, CancellationToken cancellationToken)
        {
            var context = BuildContext(request);
            var rulesets = await _ruleRepository
            .GetActiveRulesetsAsync(cancellationToken);

            foreach (var ruleset in rulesets.OrderBy(x => x.Priority))
            {
                var rulesetMatched = _ruleEvaluator.EvaluateConditions(
                    ruleset.Conditions, context);

                if (!rulesetMatched)
                    continue;

                foreach (var rule in ruleset.Rules.OrderBy(x => x.Sequence))
                {
                    var ruleMatched = _ruleEvaluator.EvaluateConditions(
                        rule.Conditions, context);

                    if (!ruleMatched)
                        continue;

                    return new EvaluationResponse
                    {
                        Matched = true,
                        ProductionPlant = rule.ProductionPlant,
                        MatchedRuleset = ruleset.Name,
                        MatchedRule = rule.Name,
                        Reason = "All conditions satisfied"
                    };
                }

                // First matching Ruleset wins.
                // Do not continue to another Ruleset.
                break;
            }

            return new EvaluationResponse
            {
                Matched = false,
                Reason = "No matching rule found"
            };
        }

        private OrderEvaluationContext BuildContext(
        OrderRequest request)
        {
            return new OrderEvaluationContext
            {
                PublisherNumber = request.PublisherNumber,
                OrderMethod = request.OrderMethod,

                Countries = request.Shipments
                    .Select(x => x.ShipTo.IsoCountry)
                    .Distinct()
                    .ToList(),

                BindTypeCodes = request.Items
                    .SelectMany(x => x.Components)
                    .Where(x => x.Attributes.ContainsKey("BindTypeCode"))
                    .Select(x => x.Attributes["BindTypeCode"])
                    .Distinct()
                    .ToList(),

                TotalPrintQuantity = request.Items.Sum(
                    x => x.PrintQuantity)
            };
        }
    }
}
