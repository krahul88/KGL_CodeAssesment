using System;
using System.Collections.Generic;
using System.Text;
using ProductionPlantEvaluation.Domain.DTO;

namespace ProductionPlantEvaluation.Application.Interfaces
{
    public interface IRuleRepository
    {
        Task<List<Ruleset>> GetActiveRulesetsAsync(
        CancellationToken cancellationToken);
    }
}
