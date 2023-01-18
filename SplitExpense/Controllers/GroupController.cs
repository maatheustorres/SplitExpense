using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Groups.Commands.AddUser;
using SplitExpense.Application.Groups.Commands.Create;
using SplitExpense.Application.Groups.Commands.UpdateGroupName;
using SplitExpense.Application.Groups.Queries.GetGroupsByUserId;
using SplitExpense.Contracts.Group;

namespace SplitExpense.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("userId/{userId}")]
    public async Task<IActionResult> GetGroupsByUserId(Guid userId, int page, int pageSize)
    {
        var command = new GetGroupsByUserIdQuery(userId, page, pageSize);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateGroupRequest createGroupRequest)
    {
        var command = new CreateGroupCommand(createGroupRequest.UserId, createGroupRequest.Name, createGroupRequest.CategoryId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("add")] 
    public async Task<IActionResult> AddUser(AddUserRequest addUserRequest)
    {
        var command = new AddUserCommand(addUserRequest.GroupId, addUserRequest.Emails);

        var result = await _mediator.Send(command);

        if(result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPut("update/{groupId}")]
    public async Task<IActionResult> Update(Guid groupId, UpdateGroupRequest updateGroupRequest)
    {
        var command = new UpdateGroupNameCommand(groupId, updateGroupRequest.Name);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }
}
