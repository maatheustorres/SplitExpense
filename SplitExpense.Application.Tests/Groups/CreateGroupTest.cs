using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.Groups.Commands.Create;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Groups;

public class CreateGroupTest
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IGroupRepository> _groupRepository = new();
    private readonly CreateGroupCommandHandler _createGroupoCommandHandler;

    public CreateGroupTest()
    {
        _createGroupoCommandHandler = new CreateGroupCommandHandler(
            _userRepository.Object,
            _unitOfWork.Object,
            _groupRepository.Object);
    }

    [Fact]
    public async Task CreateGroupWithValidParameters()
    {
        // Arrange
        var command = new CreateGroupCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            "Viagem RJ",
            1);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act 
        var result = await _createGroupoCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateGroupWithUserNotFound()
    {
        // Arrange
        var command = new CreateGroupCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            "Viagem RJ",
            1);

        // Act 
        var result = await _createGroupoCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The user with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateGroupWithCategoryNotFound()
    {
        // Arrange
        var command = new CreateGroupCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            "Viagem RJ",
            3);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act 
        var result = await _createGroupoCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The category with the specified identifier was not found.");
    }

    [Fact]
    public async Task CreateGroupWithNameRequired()
    {
        // Arrange
        var command = new CreateGroupCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            "",
            3);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act 
        var result = await _createGroupoCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The name is required");
    }

    [Fact]
    public async Task CreateGroupWithNameLongerThanAllowed()
    {

        // Arrange
        var command = new CreateGroupCommand(
            Guid.Parse("E0F62196-2876-41EE-81E6-A17AEEC13A55"),
            "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm",
            3);

        ResultT<FirstName> firstNameResult = FirstName.Create("Matheus");
        ResultT<LastName> lastNameResult = LastName.Create("Torres");
        ResultT<Email> emailResult = Email.Create("mmtorresdosreis@gmail.com");
        var passwordHash = "oCnrkMwQG7/UeGsNxEzTCHOTHBObMKXSzEpXog9qsk8piVtWB34hfEChPo9bX6qV";

        User user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);

        _userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act 
        var result = await _createGroupoCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Contains("The name is longer than allowed.");
    }
}
