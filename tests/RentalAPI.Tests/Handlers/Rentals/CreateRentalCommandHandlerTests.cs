using FluentAssertions;
using Moq;
using RentalAPI.Application.Commands.Rentals;
using RentalAPI.Application.Handlers.Rentals;
using RentalAPI.Application.Interfaces;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Tests.Handlers.Rentals;

public class CreateRentalCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly CreateRentalCommandHandler _handler;

    public CreateRentalCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _rentalRepositoryMock = new Mock<IRentalRepository>();

        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Vehicles).Returns(_vehicleRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Rentals).Returns(_rentalRepositoryMock.Object);

        _handler = new CreateRentalCommandHandler(_unitOfWorkMock.Object, _kafkaProducerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateRental()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var customer = new Customer
        {
            Id = customerId,
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "11999999999",
            DriversLicense = "12345678900",
            IsActive = true
        };

        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            LicensePlate = "ABC-1234",
            DailyRate = 150.00m,
            Status = VehicleStatus.Available
        };

        var command = new CreateRentalCommand
        {
            CustomerId = customerId,
            VehicleId = vehicleId,
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(5),
            InitialMileage = 10000,
            Notes = "Teste"
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        _rentalRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Rental>()))
            .ReturnsAsync((Rental r) => r);

        _vehicleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        _kafkaProducerMock.Setup(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.VehicleId.Should().Be(vehicleId);
        result.Status.Should().Be(RentalStatus.Active);
        result.TotalAmount.Should().Be(750.00m);

        _rentalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
        _vehicleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Vehicle>(v => v.Status == VehicleStatus.Rented)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _kafkaProducerMock.Verify(k => k.ProduceAsync("rental-events", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new CreateRentalCommand
        {
            CustomerId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(5),
            InitialMileage = 10000
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InactiveCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "11999999999",
            DriversLicense = "12345678900",
            IsActive = false
        };

        var command = new CreateRentalCommand
        {
            CustomerId = customerId,
            VehicleId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(5),
            InitialMileage = 10000
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("inativo");
    }

    [Fact]
    public async Task Handle_VehicleNotAvailable_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var customer = new Customer
        {
            Id = customerId,
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "11999999999",
            DriversLicense = "12345678900",
            IsActive = true
        };

        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            LicensePlate = "ABC-1234",
            DailyRate = 150.00m,
            Status = VehicleStatus.Rented
        };

        var command = new CreateRentalCommand
        {
            CustomerId = customerId,
            VehicleId = vehicleId,
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(5),
            InitialMileage = 10000
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("não está disponível");
    }

    [Fact]
    public async Task Handle_InvalidDateRange_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var customer = new Customer
        {
            Id = customerId,
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "11999999999",
            DriversLicense = "12345678900",
            IsActive = true
        };

        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            LicensePlate = "ABC-1234",
            DailyRate = 150.00m,
            Status = VehicleStatus.Available
        };

        var command = new CreateRentalCommand
        {
            CustomerId = customerId,
            VehicleId = vehicleId,
            StartDate = DateTime.UtcNow.Date,
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(-1),
            InitialMileage = 10000
        };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("posterior");
    }
}
