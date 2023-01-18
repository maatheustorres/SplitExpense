using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Users.Queries.GetUserById;
using SplitExpense.Application.Users.Queries.GetUsersByGroupId;

namespace SplitExpense.Controllers;

[Authorize]
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

    [HttpGet("usersByGroup/{groupId}")]
    public async Task<IActionResult> GetUsersByGroup(Guid groupId)
    {
        var query = new GetUsersByGroupIdQuery(groupId);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
