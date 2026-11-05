using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Domain.DTO;

namespace ProductionPlantEvaluation.Infrastructure.Repositories
{
    public class EvaluationLogRepository(ApplicationDbContext dbContext) : IEvaluationLogRepository
    {
        public async Task AddAsync(EvaluationLog evaluationLog, CancellationToken cancellationToken)
        {
            dbContext.EvaluationLogs.Add(evaluationLog);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
