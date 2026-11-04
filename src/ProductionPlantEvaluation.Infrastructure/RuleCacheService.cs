using Microsoft.Extensions.Caching.Memory;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Infrastructure
{
    public class RuleCacheService : IRuleCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IRuleRepository _repository;

        private const string CacheKey = "ActiveRulesets";

        public RuleCacheService(
            IMemoryCache cache,
            IRuleRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<List<Ruleset>> GetRulesetsAsync(
            CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(CacheKey, out List<Ruleset>? rulesets)
                && rulesets != null)
            {
                return rulesets;
            }

            rulesets = await _repository.GetActiveRulesetsAsync(
                cancellationToken);

            _cache.Set(
                CacheKey,
                rulesets,
                TimeSpan.FromMinutes(10));

            return rulesets;
        }

        public void ClearCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}
