# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/RentalAPI.API/RentalAPI.API.csproj", "RentalAPI.API/"]
COPY ["src/RentalAPI.Application/RentalAPI.Application.csproj", "RentalAPI.Application/"]
COPY ["src/RentalAPI.Domain/RentalAPI.Domain.csproj", "RentalAPI.Domain/"]
COPY ["src/RentalAPI.Infrastructure/RentalAPI.Infrastructure.csproj", "RentalAPI.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "RentalAPI.API/RentalAPI.API.csproj"

# Copy all source code
COPY src/ .

# Build application
WORKDIR "/src/RentalAPI.API"
RUN dotnet build "RentalAPI.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "RentalAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RentalAPI.API.dll"]
