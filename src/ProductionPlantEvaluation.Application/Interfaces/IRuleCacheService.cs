using ProductionPlantEvaluation.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Application.Interfaces
{
    public interface IRuleCacheService
    {
        Task<List<Ruleset>> GetRulesetsAsync(
            CancellationToken cancellationToken);

        void ClearCache();
    }
}
