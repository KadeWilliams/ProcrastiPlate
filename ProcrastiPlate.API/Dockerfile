FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProcrastiPlate.API/ProcrastiPlate.API.csproj", "ProcrastiPlate.API/"]
RUN dotnet restore "ProcrastiPlate.API/ProcrastiPlate.API.csproj"
COPY . .
WORKDIR "/src/ProcrastiPlate.API"
RUN dotnet build "./ProcrastiPlate.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProcrastiPlate.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcrastiPlate.API.dll"]
