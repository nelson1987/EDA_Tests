using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDA.Api.Controllers;
[ApiController]
[Route("api/[controller]s")]
public class ContaController : ControllerBase
{
    private readonly ILogger<ContaController> _logger;
    private readonly IMediator _mediator;

    public ContaController(ILogger<ContaController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        string mensagem = string.Empty;
        await Task.Run(() =>
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));
            _logger.LogInformation("INI");
            _logger.LogInformation("END");
            mensagem = "List All";
        }, cancellationToken);
        return Ok(mensagem);
    }
    //[HttpGet("{int:id}")]
    //public IActionResult GetWithFilters(int id, CancellationToken cancellationToken)
    //{
    //    _logger.LogInformation("INI");
    //    _logger.LogInformation("END");
    //    return Ok($"List with filters: {typeof(int)}:{nameof(id)}");
    //}

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AberturaContaCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("INI");
        if (ModelState.IsValid)
        {
            //https://masstransit.io/documentation/configuration/transports/kafka
            //https://www.macoratti.net/21/01/c_cancelasync1.htm
            //https://www.infoworld.com/article/3692811/how-to-use-the-unit-of-work-pattern-in-aspnet-core.html
            //https://dotnettutorials.net/lesson/unit-of-work-csharp-mvc/
            AberturaContaCommandResponse response = await _mediator.Send(command, cancellationToken);
            _logger.LogInformation("END");
            return Ok(response);

        }
        _logger.LogInformation("ERR");
        return Problem();
    }
}
