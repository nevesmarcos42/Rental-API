using FluentAssertions;
using Moq;
using RentalAPI.Application.Queries.Vehicles;
using RentalAPI.Application.Handlers.Vehicles;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;
using Xunit;

namespace RentalAPI.Tests.Handlers.Vehicles;

public class GetAvailableVehiclesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GetAvailableVehiclesQueryHandler _handler;

    public GetAvailableVehiclesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new GetAvailableVehiclesQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAvailableVehicles()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle 
            { 
                Id = Guid.NewGuid(), 
                Brand = "Toyota", 
                Model = "Corolla",
                LicensePlate = "ABC1234",
                Status = VehicleStatus.Available,
                DailyRate = 100.00m
            },
            new Vehicle 
            { 
                Id = Guid.NewGuid(), 
                Brand = "Honda", 
                Model = "Civic",
                LicensePlate = "XYZ5678",
                Status = VehicleStatus.Available,
                DailyRate = 120.00m
            }
        };

        _unitOfWorkMock.Setup(x => x.Vehicles.GetAvailableVehiclesAsync())
            .ReturnsAsync(vehicles);

        // Act
        var result = await _handler.Handle(new GetAvailableVehiclesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(v => v.Status.Should().Be(VehicleStatus.Available));
        
        _unitOfWorkMock.Verify(x => x.Vehicles.GetAvailableVehiclesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoAvailableVehicles_ReturnsEmptyList()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Vehicles.GetAvailableVehiclesAsync())
            .ReturnsAsync(new List<Vehicle>());

        // Act
        var result = await _handler.Handle(new GetAvailableVehiclesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
