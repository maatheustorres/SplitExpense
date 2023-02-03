using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.Groups.Commands.AddUser;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Groups;

public class AddUserToGroupTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IGroupRepository> _groupRepository = new();
    private readonly Mock<IUserGroupRepository> _userGroupRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AddUserCommandHandler _addUserCommandHandler;

    public AddUserToGroupTests()
    {
        _addUserCommandHandler = new AddUserCommandHandler(
            _userRepository.Object,
            _groupRepository.Object,
            _userGroupRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task AddUserToGroupWithValidParameters()
    {
        // Arrange
        List<string> emails = new()
        {
            "usario1@outlook.com"
        };

        var command = new AddUserCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), emails);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        List<User> users = new()
        {
            user
        };

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _userRepository.Setup(x => x.GetUsersByEmailsAsync(It.IsAny<IReadOnlyCollection<Email>>())).ReturnsAsync(users);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(false);

        // Act
        var result = await _addUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task AddUserToGroupWithEmailRequired()
    {
        // Arrange
        List<string> emails = new();

        var command = new AddUserCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), emails);

        // Act
        var result = await _addUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The email is required.");
    }
    
    [Fact]
    public async Task AddUserToGroupWithInvalidFormatEmail()
    {
        // Arrange
        List<string> emails = new()
        {
            "usuario@"
        };

        var command = new AddUserCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), emails);

        // Act
        var result = await _addUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The email format is invalid.");
    }

    [Fact]
    public async Task AddUserToGroupWithUserNotFound()
    {
        // Arrange
        List<string> emails = new()
        {
            "usuario1@outlook.com"
        };

        var command = new AddUserCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), emails);

        // Act
        var result = await _addUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The user with the specified identifier was not found.");
    }
    [Fact]
    public async Task AddUserToGroupWithUsersAlreadyAdded()
    {
        // Arrange
        List<string> emails = new()
        {
            "usario1@outlook.com"
        };

        var command = new AddUserCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), emails);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        List<User> users = new()
        {
            user
        };

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _userRepository.Setup(x => x.GetUsersByEmailsAsync(It.IsAny<IReadOnlyCollection<Email>>())).ReturnsAsync(users);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(true);

        // Act
        var result = await _addUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The user(s) has/have already been added.");
    }
}
