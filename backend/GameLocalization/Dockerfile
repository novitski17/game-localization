# ---------- Runtime layer ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ---------- Build layer ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only csproj files first (better cache for restore)
COPY GameLocalization.Api/GameLocalization.Api.csproj GameLocalization.Api/
COPY GameLocalization.Core/GameLocalization.Core.csproj GameLocalization.Core/
COPY GameLocalization.Infrastructure/GameLocalization.Infrastructure.csproj GameLocalization.Infrastructure/
RUN dotnet restore GameLocalization.Api/GameLocalization.Api.csproj

# Copy the rest of the source code
COPY . .
WORKDIR /src/GameLocalization.Api

# Publish the API project
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Final image ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "GameLocalization.Api.dll"]
