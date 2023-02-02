using FluentAssertions;
using Moq;
using SplitExpense.Application.Authentication.Commands.Login;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.Services;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Authentication;

public class LoginTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHashChecker> _passwordHashChecker = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly LoginCommandHandler _loginCommandHandler;

    public LoginTests()
    {
        _loginCommandHandler = new LoginCommandHandler(
            _userRepository.Object,
            _passwordHashChecker.Object,
            _jwtProvider.Object);
    }

    [Fact]
    public async Task LoginWithValidParameters()
    {
        // Arrange
        var command = new LoginCommand("mmtorresdosreis@gmail.com", "Password@123");

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync(user);
        _passwordHashChecker.Setup(x => x.HashesMatch(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        // Act
        var result = await _loginCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginWithInvalidEmail()
    {
        // Arrange
        var command = new LoginCommand("matheus", "Password@123");

        // Act
        var result = await _loginCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The specified email or password are incorrect.");
    }

    [Fact]
    public async Task LoginWithEmailNotFound()
    {
        // Arrange
        var command = new LoginCommand("mmtorresdosreis@gmail.com", "Password@123");

        // Act
        var result = await _loginCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The specified email or password are incorrect.");
    }

    [Fact]
    public async Task LoginWithInvalidPassword()
    {
        // Arrange
        var command = new LoginCommand("mmtorresdosreis@gmail.com", "Password@123");

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _passwordHashChecker.Setup(x => x.HashesMatch(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        // Act
        var result = await _loginCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The specified email or password are incorrect.");
    }
}
