using ProductionPlantEvaluation.Domain.DTO;

namespace ProductionPlantEvaluation.Application.Interfaces
{
    public interface IEvaluationLogRepository
    {
        Task AddAsync(EvaluationLog evaluationLog, CancellationToken cancellationToken);
    }
}
