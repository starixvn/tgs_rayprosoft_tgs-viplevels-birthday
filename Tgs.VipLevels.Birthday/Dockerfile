#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.VipLevels.Birthday/Tgs.VipLevels.Birthday.csproj", "Tgs.VipLevels.Birthday/"]
COPY ["Tgs.VipLevels.Birthday.Core/Tgs.VipLevels.Birthday.Core.csproj", "Tgs.VipLevels.Birthday.Core/"]
COPY ["Tgs.VipLevels.Birthday.Repositories/Tgs.VipLevels.Birthday.Repositories.csproj", "Tgs.VipLevels.Birthday.Repositories/"]
RUN dotnet restore "./Tgs.VipLevels.Birthday/./Tgs.VipLevels.Birthday.csproj"
COPY . .
WORKDIR "/src/Tgs.VipLevels.Birthday"
RUN dotnet build "./Tgs.VipLevels.Birthday.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.VipLevels.Birthday.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.VipLevels.Birthday.dll"]