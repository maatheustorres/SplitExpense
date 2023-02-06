using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.SplitExpense.Commands.CreateSplitExpense;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.SplitExpense;

public class CreateSplitExpenseTests
{
    private readonly Mock<IExpenseRepository> _expenseRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IGroupRepository> _groupRepository = new();
    private readonly Mock<IUserGroupRepository> _userGroupRepository = new();
    private readonly Mock<IExpenseUserRepository> _expenseUserRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateSplitExpenseCommandHandler _createSplitExpenseCommandHandler;

    public CreateSplitExpenseTests()
    {
        _createSplitExpenseCommandHandler = new CreateSplitExpenseCommandHandler(
            _expenseRepository.Object,
            _userRepository.Object,
            _groupRepository.Object,
            _userGroupRepository.Object,
            _expenseUserRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateSplitExpenseWithValidParameters()
    {
        // Arrange
        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            user.Id,
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(true);
        _expenseUserRepository.Setup(x => x.CheckIfAddedToExpense(It.IsAny<ExpenseUsers>())).ReturnsAsync(false);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Arrange
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSplitExpenseWithExpenseNotFound()
    {
        // Arrange
        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The expense with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateSplitExpenseWithExpenseAlreadyPaid()
    {
        // Arrange
        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, true, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The expense has already been paid.");
    }
    
    [Fact]
    public async Task CreateSplitExpenseWithUserGroupNullValue()
    {
        // Arrange
        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The specified result value is null");
    }

    [Fact]
    public async Task CreateSplitExpenseWithInvalidPermissions()
    {
        // Arrange
        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The current user does not have the permissions to perform that operation.");
    }

    [Fact]
    public async Task CreateSplitExpenseWithGroupNotFound()
    {
        // Arrange
        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            user.Id,
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Arrange
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The group with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateSplitExpenseWithUserListNullOrEmpty()
    {
        // Arrange
        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        var users = new List<Guid>();

        var command = new CreateSplitExpenseCommand(
            user.Id,
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Arrange
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The user is required");
    }

    [Fact]
    public async Task CreateSplitExpenseWithValidUserAlreadyAddedToGroup()
    {
        // Arrange
        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            user.Id,
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(false);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Arrange
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The user(s) has/have already been added.");
    }


    [Fact]
    public async Task CreateSplitExpenseWithValidUserAlreadyAddedToExpense()
    {
        // Arrange
        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(550);
        var expense = Expense.Create(totalExpenseResult.Value, false, Guid.Parse("5B8A2FF3-4D0A-4994-B912-3843FC65C59A"));

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("usario1@outlook.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        var userGroup = Group.AddUserToGroup(user, group);

        var users = new List<Guid>()
        {
            Guid.Parse("4878BC72-4F33-4222-96A0-4F928554C312"),
            Guid.Parse("9CBF1555-2DCA-42E7-8FE4-8BB47A3A6DCF")
        };

        var command = new CreateSplitExpenseCommand(
            user.Id,
            users,
            Guid.Parse("BDE4F833-5DCD-4619-8467-840FF1DD07B7"));

        _expenseRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expense);
        _userGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(userGroup);
        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);
        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _userGroupRepository.Setup(x => x.CheckIfAddedToGroup(It.IsAny<UserGroup>())).ReturnsAsync(true);
        _expenseUserRepository.Setup(x => x.CheckIfAddedToExpense(It.IsAny<ExpenseUsers>())).ReturnsAsync(true);

        // Act
        var result = await _createSplitExpenseCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Arrange
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The user(s) has/have already been added to the expense.");
    }
}
