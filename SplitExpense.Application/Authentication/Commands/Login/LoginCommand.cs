using MediatR;
using SplitExpense.Contracts.Authentication;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<ResultT<AuthenticationResponse>>;
