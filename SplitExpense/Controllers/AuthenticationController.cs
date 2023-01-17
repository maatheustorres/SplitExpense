using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Application.Authentication.Commands.Login;
using SplitExpense.Application.Users.Commands.CreateUser;
using SplitExpense.Contracts.Authentication;

namespace SplitExpense.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var command = new LoginCommand(loginRequest.Email, loginRequest.Password);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(RegisterRequest registerRequest)
    {
        var command = new CreateUserCommand(registerRequest.FirstName, registerRequest.LastName, registerRequest.Email, registerRequest.Password);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
