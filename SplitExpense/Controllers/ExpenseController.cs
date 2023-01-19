using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Expenses.Commands.CreateExpense;
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

        if(result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }
}
