using MediatR;
using SplitExpense.Contracts.Authentication;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Cpf,
    string Rg) : IRequest<ResultT<AuthenticationResponse>>;
