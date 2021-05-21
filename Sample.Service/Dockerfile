FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Sample.Components/Sample.Components.csproj", "Sample.Components/"]
COPY ["Sample.Contracts/Sample.Contracts.csproj", "Sample.Contracts/"]
COPY ["Sample.Service/Sample.Service.csproj", "Sample.Service/"]

RUN dotnet restore "Sample.Service/Sample.Service.csproj"
COPY . .
WORKDIR "/src/Sample.Service"
RUN dotnet build "Sample.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Service.dll"]