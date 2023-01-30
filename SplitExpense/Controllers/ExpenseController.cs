using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Expenses.Commands.SplitExpense;
using SplitExpense.Application.Expenses.Commands.CreateExpense;
using SplitExpense.Application.Expenses.Queries.GetExpensesByGroupId;
using SplitExpense.Contracts.Expense;
using SplitExpense.Application.Expenses.Commands.UpdateExpense;

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
    public async Task<IActionResult> AddUsersToExpense(Guid expenseId, SplitExpenseRequest splitExpenseRequest)
    {
        var command = new SplitExpenseCommand(
            splitExpenseRequest.GroupId,
            splitExpenseRequest.UserId,
            splitExpenseRequest.UserIds, 
            expenseId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }

    [HttpPut("{expenseId}")]
    public async Task<IActionResult> UpdateExpense(Guid expenseId, UpdateExpenseRequest updateExpenseRequest)
    {
        var command = new UpdateExpenseCommand(
            expenseId, 
            updateExpenseRequest.Expense, 
            updateExpenseRequest.Paid, 
            updateExpenseRequest.UserId,
            updateExpenseRequest.UserGroupId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
