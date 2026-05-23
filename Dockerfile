# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Lulu_Portfolio.sln", "."]
COPY ["Lulu_Portfolio.API/Lulu_Portfolio.API.csproj", "Lulu_Portfolio.API/"]
COPY ["Lulu_Portfolio.Application/Lulu_Portfolio.Application.csproj", "Lulu_Portfolio.Application/"]
COPY ["Lulu_Portfolio.Infrastructure/Lulu_Portfolio.Infrastructure.csproj", "Lulu_Portfolio.Infrastructure/"]
COPY ["Lulu_Portfolio.Domain/Lulu_Portfolio.Domain.csproj", "Lulu_Portfolio.Domain/"]
COPY ["Lulu_Portfolio.Shared/Lulu_Portfolio.Shared.csproj", "Lulu_Portfolio.Shared/"]

# Restore dependencies
RUN dotnet restore "Lulu_Portfolio.sln"

# Copy all source code
COPY . .

# Build
RUN dotnet build "Lulu_Portfolio.API/Lulu_Portfolio.API.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "Lulu_Portfolio.API/Lulu_Portfolio.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run app
ENTRYPOINT ["dotnet", "Lulu_Portfolio.API.dll"]