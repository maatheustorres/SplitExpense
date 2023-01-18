using MediatR;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Users.Queries.GetUserById;

namespace SplitExpense.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(Guid userId)
    {
        var query = new GetUserByIdQuery(userId);

        var result = await _mediator.Send(query);

        if(result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
