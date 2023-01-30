using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Expenses.Commands.AddUserToExpense;
using SplitExpense.Application.Expenses.Commands.CreateExpense;
using SplitExpense.Application.Expenses.Queries.GetExpensesByGroupId;
using SplitExpense.Contracts.Expense;

namespace SplitExpense.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly IMediator _mediator;
    public ExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetExpensesByGroupId(Guid groupId)
    {
        var query = new GetExpensesByGroupIdQuery(groupId);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("create/{userGroupId}")]
    public async Task<IActionResult> Create(Guid userGroupId, CreateExpenseRequest createExpenseRequest)
    {
        var command = new CreateExpenseCommand(
            createExpenseRequest.TotalExpense,
            createExpenseRequest.Paid,
            userGroupId,
            createExpenseRequest.UserId,
            createExpenseRequest.GroupId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }

    [HttpPost("addToExpense/{expenseId}")]
    public async Task<IActionResult> AddUsersToExpense(Guid expenseId, AddUsersToExpenseRequest addUsersToExpenseRequest)
    {
        var command = new AddUserToExpenseCommand(
            addUsersToExpenseRequest.GroupId,
            addUsersToExpenseRequest.UserId,
            addUsersToExpenseRequest.UserIds, 
            expenseId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }
}
