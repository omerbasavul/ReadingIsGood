FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ReadingIsGood.sln ./

COPY BuildingBlocks/ReadingIsGood.BuildingBlocks.ApplicationContext/ReadingIsGood.BuildingBlocks.ApplicationContext.csproj BuildingBlocks/ReadingIsGood.BuildingBlocks.ApplicationContext/
COPY BuildingBlocks/ReadingIsGood.BuildingBlocks.Common/ReadingIsGood.BuildingBlocks.Common.csproj BuildingBlocks/ReadingIsGood.BuildingBlocks.Common/
COPY BuildingBlocks/ReadingIsGood.BuildingBlocks.EntityFrameworkCore/ReadingIsGood.BuildingBlocks.EntityFrameworkCore.csproj BuildingBlocks/ReadingIsGood.BuildingBlocks.EntityFrameworkCore/
COPY BuildingBlocks/ReadingIsGood.BuildingBlocks.Redis/ReadingIsGood.BuildingBlocks.Redis.csproj BuildingBlocks/ReadingIsGood.BuildingBlocks.Redis/


COPY Core/ReadingIsGood.Domain/ReadingIsGood.Domain.csproj Core/ReadingIsGood.Domain/
COPY Core/ReadingIsGood.Application/ReadingIsGood.Application.csproj Core/ReadingIsGood.Application/
COPY Infrastructure/ReadingIsGood.Infrastructure/ReadingIsGood.Infrastructure.csproj Infrastructure/ReadingIsGood.Infrastructure/
COPY Presentation/ReadingIsGood.Api/ReadingIsGood.Api.csproj Presentation/ReadingIsGood.Api/

RUN dotnet restore Presentation/ReadingIsGood.Api/ReadingIsGood.Api.csproj

COPY . .

RUN dotnet publish Presentation/ReadingIsGood.Api/ReadingIsGood.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "ReadingIsGood.Api.dll"]