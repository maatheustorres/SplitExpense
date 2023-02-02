using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Application.Core.Abstractions.Cryptography;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.Users.Commands.CreateUser;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Users;

public class CreateUserTest
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly CreateUserCommandHandler _createUserCommandHandler;

    public CreateUserTest()
    {
        _createUserCommandHandler = new CreateUserCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _unitOfWork.Object,
            _jwtProvider.Object);
    }

    [Fact]
    public async Task CreateUserWithValidParameters()
    {
        // arrange 
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "Matheus@123");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        _userRepository.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(true);
        _passwordHasher.Setup(x => x.HashPassword(It.IsAny<Password>())).Returns(passwordHash);

        // act
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUserWithFirstNameNullOrEmpty()
    {
        // arrange
        var command = new CreateUserCommand("", "Torres", "matheus@outlook.com", "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The first name is required");
    }

    [Fact]
    public async Task CreateUserWithFirstNameLongerThanAllowed()
    {
        // arrange
        var command = new CreateUserCommand("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm", 
            "Torres", 
            "matheus@outlook.com", 
            "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The first name is longer than allowed.");
    }

    [Fact]
    public async Task CreateUserWithLastNameNullOrEmpty()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "", "matheus@outlook.com", "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The last name is required");
    }

    [Fact]
    public async Task CreateUserWithLastNameLongerThanAllowed()
    {
        // arrange
        var command = new CreateUserCommand("Matheus",
            "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm",
            "matheus@outlook.com",
            "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The last name is longer than allowed.");
    }

    [Fact]
    public async Task CreateUserWithDuplicateEmail()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "Matheus@123");

        _userRepository.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(false);

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The specified email is already in use.");
    }

    [Fact]
    public async Task CreateUserWithEmailNullOrEmpty()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "", "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The email is required");
    }

    [Fact]
    public async Task CreateUserWithInvalidEmailFormat()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus", "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The email format is invalid.");
    }

    [Fact]
    public async Task CreateUserWithEmailLongerThanAllowed()
    {
        // arrange
        var command = new CreateUserCommand("Matheus",
            "Torres",
            "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm@outlook.com",
            "Matheus@123");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The email is longer than allowed.");
    }

    [Fact]
    public async Task CreateUserWithPasswordNullOrEmpty()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password is required.");
    }

    [Fact]
    public async Task CreateUserWithPasswordTooShort()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "Math@1");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password is too short.");
    }
    
    [Fact]
    public async Task CreateUserWithPasswordMissingLowercaseLetter()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "MATHEUS@1");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password requires at least one lowercase letter.");
    }   
    
    [Fact]
    public async Task CreateUserWithPasswordMissingUppercaseLetter()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "matheus@1");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password requires at least one uppercase letter.");
    }

    [Fact]
    public async Task CreateUserWithPasswordMissingDigit()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "Matheus@");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password requires at least one digit.");
    }

    [Fact]
    public async Task CreateUserWithPasswordMissingNonAlphaNumeric()
    {
        // arrange
        var command = new CreateUserCommand("Matheus", "Torres", "matheus@outlook.com", "Matheus1");

        // act 
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The password requires at least one non-alphanumeric.");
    }
}
