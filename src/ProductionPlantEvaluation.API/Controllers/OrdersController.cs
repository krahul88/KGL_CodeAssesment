using Microsoft.AspNetCore.Mvc;
using ProductionPlantEvaluation.Application.DTO;
using ProductionPlantEvaluation.Application.Interfaces;

namespace ProductionPlantEvaluation.API.Controllers
{
    [Route("api")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderEvaluationService _service;
        public OrdersController(IOrderEvaluationService service)
        { _service = service; }

        [HttpPost("Evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] OrderRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OrderId))
                return BadRequest("Invalid request for the submitted order");

            var output = await _service.EvaluateAsync(request, cancellationToken);

            return Ok(output);
        }
    }
}
