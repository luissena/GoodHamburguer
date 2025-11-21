FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GoodHamburguer.Api/GoodHamburguer.Api.csproj", "GoodHamburguer.Api/"]
COPY ["GoodHamburguer.Domain/GoodHamburguer.Domain.csproj", "GoodHamburguer.Domain/"]
RUN dotnet restore "GoodHamburguer.Api/GoodHamburguer.Api.csproj" --verbosity quiet

COPY . .
WORKDIR "/src/GoodHamburguer.Api"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GoodHamburguer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    -r linux-x64 \
    --self-contained false \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

RUN groupadd -r appuser && useradd -r -g appuser appuser

LABEL org.opencontainers.image.title="GoodHamburguer API"
LABEL org.opencontainers.image.description="REST API for GoodHamburguer application"
LABEL org.opencontainers.image.version="1.0.0"

EXPOSE 5010
ENV ASPNETCORE_URLS=http://+:5010 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=publish /app/publish .

RUN chown -R appuser:appuser /app

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5010/health || exit 1

USER appuser

ENTRYPOINT ["dotnet", "GoodHamburguer.Api.dll"]

