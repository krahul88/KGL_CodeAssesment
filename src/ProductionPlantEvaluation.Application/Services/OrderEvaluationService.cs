using ProductionPlantEvaluation.Application.DTO;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Domain.DTO;
using ProductionPlantEvaluation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ProductionPlantEvaluation.Application.Services
{
    public class OrderEvaluationService : IOrderEvaluationService
    {
        private readonly IRuleCacheService _ruleCacheService;
        private readonly IRuleEvaluator _ruleEvaluator;
        private readonly IEvaluationLogRepository _evaluationLogRepository;

        public OrderEvaluationService(IRuleCacheService ruleCacheService,
        IRuleEvaluator ruleEvaluator,
        IEvaluationLogRepository evaluationLogRepository)
        {
            _ruleCacheService = ruleCacheService;
            _ruleEvaluator = ruleEvaluator;
            _evaluationLogRepository = evaluationLogRepository;
        }

        public async Task<EvaluationResponse> EvaluateAsync(OrderRequest request, CancellationToken cancellationToken)
        {
            var context = BuildContext(request);
            var rulesets = await _ruleCacheService
                .GetRulesetsAsync(cancellationToken);

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

                    var response = new EvaluationResponse
                    {
                        Matched = true,
                        ProductionPlant = rule.ProductionPlant,
                        MatchedRuleset = ruleset.Name,
                        MatchedRule = rule.Name,
                        Reason = "All conditions satisfied"
                    };

                    await SaveEvaluationLogAsync(request, response, cancellationToken);
                    return response;
                }

                // First matching Ruleset wins.
                // Do not continue to another Ruleset.
                break;
            }

            var unmatchedResponse = new EvaluationResponse
            {
                Matched = false,
                Reason = "No matching rule found"
            };

            await SaveEvaluationLogAsync(request, unmatchedResponse, cancellationToken);
            return unmatchedResponse;
        }

        public Task LogInvalidRequestAsync(
            OrderRequest? request,
            CancellationToken cancellationToken)
        {
            return SaveEvaluationLogAsync(
                request,
                new EvaluationResponse
                {
                    Matched = false,
                    Reason = "Invalid request for the submitted order"
                },
                cancellationToken);
        }

        private Task SaveEvaluationLogAsync(
            OrderRequest? request,
            EvaluationResponse response,
            CancellationToken cancellationToken)
        {
            return _evaluationLogRepository.AddAsync(new EvaluationLog
            {
                OrderId = request?.OrderId ?? string.Empty,
                InputJson = JsonSerializer.Serialize(request),
                Matched = response.Matched,
                MatchedRuleset = response.MatchedRuleset,
                MatchedRule = response.MatchedRule,
                ProductionPlant = response.ProductionPlant,
                Reason = response.Reason,
                EvaluatedAtUtc = DateTime.UtcNow
            }, cancellationToken);
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
