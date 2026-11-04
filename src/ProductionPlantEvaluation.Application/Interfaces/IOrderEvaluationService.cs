using ProductionPlantEvaluation.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Application.Interfaces
{
    public interface IOrderEvaluationService
    {
        Task<EvaluationResponse> EvaluateAsync(
            OrderRequest context,
            CancellationToken cancellationToken);
    }
}
