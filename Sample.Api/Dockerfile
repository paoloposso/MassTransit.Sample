FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Sample.Api/Sample.Api.csproj", "Sample.Api/"]
COPY ["Sample.Components/Sample.Components.csproj", "Sample.Components/"]
COPY ["Sample.Contracts/Sample.Contracts.csproj", "Sample.Contracts/"]

RUN dotnet restore "Sample.Api/Sample.Api.csproj"
COPY . .
WORKDIR "/src/Sample.Api"
RUN dotnet build "Sample.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Api.dll"]