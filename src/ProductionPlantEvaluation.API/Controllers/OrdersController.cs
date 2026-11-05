using Microsoft.AspNetCore.Mvc;
using ProductionPlantEvaluation.Application.DTO;
using ProductionPlantEvaluation.Application.Interfaces;

namespace ProductionPlantEvaluation.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderEvaluationService _service;
        public OrdersController(IOrderEvaluationService service)
        { _service = service; }

        [HttpPost("Evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] OrderRequest request, CancellationToken cancellationToken)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.OrderId)
                || request.Shipments == null
                || request.Items == null
                || request.Shipments.Any(shipment => shipment?.ShipTo == null)
                || request.Items.Any(item => item == null
                    || item.Components == null
                    || item.Components.Any(component => component == null
                        || component.Attributes == null)))
            {
                await _service.LogInvalidRequestAsync(request, cancellationToken);
                return BadRequest("Invalid request for the submitted order");
            }

            var output = await _service.EvaluateAsync(request, cancellationToken);

            return Ok(output);
        }
    }
}
