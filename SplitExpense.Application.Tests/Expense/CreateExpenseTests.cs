using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.Expenses.Commands.CreateExpense;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Expense;

public class CreateExpenseTests
{
    private readonly Mock<IUserGroupRepository> _userGroupRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IGroupRepository> _groupRepository = new();
    private readonly Mock<IExpenseRepository> _expenseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateExpenseCommandHandler _createExpenseCommandHandler;

    public CreateExpenseTests()
    {
        _createExpenseCommandHandler = new CreateExpenseCommandHandler(
            _userGroupRepository.Object,
            _unitOfWork.Object,
            _userRepository.Object,
            _groupRepository.Object,
            _expenseRepository.Object);
    }

    [Fact]
    public async Task CreateExpenseWithValidParameters()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            100,
            false,
            Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"),
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(true);

        // Act 
        var result = await _createExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateExpenseWithUserNotFound()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            100,
            false,
            Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"),
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"));

        // Act 
        var result = await _createExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The user with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateExpenseWithGroupNotFound()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            100,
            false,
            Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"),
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act 
        var result = await _createExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The group with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateExpenseWithUserInvalidPermissions()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            100,
            false,
            Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"),
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(false);

        // Act 
        var result = await _createExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The current user does not have the permissions to perform that operation.");
    }

    [Fact]
    public async Task CreateExpenseWithInvalidExpense()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            0,
            false,
            Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"),
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(true);

        // Act 
        var result = await _createExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Expense amount must be greater than zero.");
    }
}
