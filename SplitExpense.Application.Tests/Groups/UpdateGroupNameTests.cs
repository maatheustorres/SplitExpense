using FluentAssertions;
using Moq;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Application.Groups.Commands.UpdateGroupName;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Tests.Groups;

public class UpdateGroupNameTests
{
    private readonly Mock<IGroupRepository> _groupRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateGroupNameCommandHandler _updateGroupNameCommandHandler;

    public UpdateGroupNameTests()
    {
        _updateGroupNameCommandHandler = new UpdateGroupNameCommandHandler(_groupRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task UpdateGroupNameWithValidParameters()
    {
        // Arrange
        var command = new UpdateGroupNameCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), "Viagem RJ");

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);

        // Act
        var result = await _updateGroupNameCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGroupNameWithGroupNotFound()
    {
        // Arrange
        var command = new UpdateGroupNameCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), "Viagem RJ");

        // Act
        var result = await _updateGroupNameCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The group with the specified identifier was not found.");
    }

    [Fact]
    public async Task UpdateGroupNameWithNameRequired()
    {
        // Arrange
        var command = new UpdateGroupNameCommand(Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), "");

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);

        // Act
        var result = await _updateGroupNameCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The name is required");
    }

    [Fact]
    public async Task UpdateGroupNameWithNameLongerThanAllowed()
    {
        // Arrange
        var command = new UpdateGroupNameCommand(
            Guid.Parse("A1DB0611-79D5-4F19-B5ED-AF70ABFE12A7"), 
            "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");

        Category category = Category.FromValue(2);
        ResultT<Name> nameResult = Name.Create("Viagem RJ");

        var group = Group.Create(nameResult.Value, category);

        _groupRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(group);

        // Act
        var result = await _updateGroupNameCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("The name is longer than allowed.");
    }
}
