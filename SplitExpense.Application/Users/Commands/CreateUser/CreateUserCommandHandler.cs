using MediatR;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Application.Core.Abstractions.Cryptography;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Authentication;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResultT<AuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
    }

    public async Task<ResultT<AuthenticationResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ResultT<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        ResultT<LastName> lastNameResult = LastName.Create(request.LastName);
        ResultT<Email> emailResult = Email.Create(request.Email);
        ResultT<Password> passwordResult = Password.Create(request.Password);

        Result firstFailureOrSuccess = Result.FirstFailureOrSuccess(firstNameResult, lastNameResult, emailResult, passwordResult);

        if (firstFailureOrSuccess.IsFailure)
        {
            return Result.Failure<AuthenticationResponse>(firstFailureOrSuccess.Error);
        }

        if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value))
        {
            return Result.Failure<AuthenticationResponse>(DomainErrors.User.DuplicateEmail);
        }

        string passwordHash = _passwordHasher.HashPassword(passwordResult.Value);

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Insert(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
