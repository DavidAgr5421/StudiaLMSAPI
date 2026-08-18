FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Studia.sln .
COPY Studia.WebApi/Studia.WebApi.csproj Studia.WebApi/
COPY Studia.Application/Studia.Application.csproj Studia.Application/
COPY Studia.Infrastructure/Studia.Infrastructure.csproj Studia.Infrastructure/
COPY Studia.Domain/Studia.Domain.csproj Studia.Domain/
COPY Studia.Console/Studia.Console.csproj Studia.Console/
COPY Studia.Domain.Tests/Studia.Domain.Tests.csproj Studia.Domain.Tests/
COPY Studia.Application.Tests/Studia.Application.Tests.csproj Studia.Application.Tests/

RUN dotnet restore Studia.WebApi/Studia.WebApi.csproj

COPY . .
RUN dotnet publish Studia.WebApi/Studia.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render asume el puerto 10000 salvo que se configure la variable PORT.
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Studia.WebApi.dll"]
