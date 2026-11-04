using Microsoft.EntityFrameworkCore;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Infrastructure.Repositories
{
    public class RuleRepository(ApplicationDbContext _dbcontext) : IRuleRepository
    {
        public Task<List<Ruleset>> GetActiveRulesetsAsync(CancellationToken cancellationToken)
         => _dbcontext.Rulesets.AsNoTracking()
            .Where(x => x.IsEnabled)
            .Include(x => x.Conditions)
            .Include(x => x.Rules)
                .ThenInclude(x => x.Conditions)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }
}

