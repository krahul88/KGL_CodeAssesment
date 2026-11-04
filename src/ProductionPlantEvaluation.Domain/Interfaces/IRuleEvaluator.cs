using ProductionPlantEvaluation.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionPlantEvaluation.Domain.Interfaces
{
    public interface IRuleEvaluator
    {
        bool EvaluateConditions(
            IEnumerable<Condition> conditions,
            OrderEvaluationContext context,
            ICollection<ConditionEvaluation>? trace = null);
    }
}
