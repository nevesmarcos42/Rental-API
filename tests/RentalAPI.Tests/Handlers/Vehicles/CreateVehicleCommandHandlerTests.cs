using FluentAssertions;
using Moq;
using RentalAPI.Application.Commands.Vehicles;
using RentalAPI.Application.Handlers.Vehicles;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;
using Xunit;

namespace RentalAPI.Tests.Handlers.Vehicles;

public class CreateVehicleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateVehicleCommandHandler _handler;

    public CreateVehicleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateVehicleCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsVehicleDto()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            LicensePlate = "ABC1234",
            Type = VehicleType.Sedan,
            DailyRate = 100.00m,
            Color = "Black"
        };

        _unitOfWorkMock.Setup(x => x.Vehicles.GetByLicensePlateAsync(command.LicensePlate))
            .ReturnsAsync((Vehicle?)null);

        _unitOfWorkMock.Setup(x => x.Vehicles.AddAsync(It.IsAny<Vehicle>()))
            .ReturnsAsync((Vehicle v) => v);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Brand.Should().Be(command.Brand);
        result.Model.Should().Be(command.Model);
        result.LicensePlate.Should().Be(command.LicensePlate);
        
        _unitOfWorkMock.Verify(x => x.Vehicles.AddAsync(It.IsAny<Vehicle>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateLicensePlate_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            LicensePlate = "ABC1234",
            Type = VehicleType.Sedan,
            DailyRate = 100.00m,
            Color = "Black"
        };

        var existingVehicle = new Vehicle { LicensePlate = command.LicensePlate };
        
        _unitOfWorkMock.Setup(x => x.Vehicles.GetByLicensePlateAsync(command.LicensePlate))
            .ReturnsAsync(existingVehicle);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _unitOfWorkMock.Verify(x => x.Vehicles.AddAsync(It.IsAny<Vehicle>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }
}
