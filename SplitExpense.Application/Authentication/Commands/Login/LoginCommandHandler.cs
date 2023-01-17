using MediatR;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Contracts.Authentication;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.Services;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ResultT<AuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashChecker _passwordHashChecker;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHashChecker passwordHashChecker, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHashChecker = passwordHashChecker;
        _jwtProvider = jwtProvider;
    }

    public async Task<ResultT<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ResultT<Email> emailResult = Email.Create(request.Email);

        if(emailResult.IsFailure)
        {
            return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
        }

        ResultT<User> userResult = await _userRepository.GetByEmailAsync(emailResult.Value); 

        if (userResult.Value is null)
        {
            return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
        }

        User user = userResult.Value;

        bool passwordValid = user.VerifyPasswordHash(request.Password, _passwordHashChecker);

        if (!passwordValid)
        {
            return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidEmailOrPassword);
        }

        string token = _jwtProvider.Create(user);

        return Result.Success(new AuthenticationResponse(
            user.Id,
            user.FirstName.Value,
            user.LastName.Value,
            user.FullName,
            user.Email.Value,
            token));
    }
}
