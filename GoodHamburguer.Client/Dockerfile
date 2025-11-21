FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GoodHamburguer.Client/GoodHamburguer.Client.csproj", "GoodHamburguer.Client/"]
RUN dotnet restore "GoodHamburguer.Client/GoodHamburguer.Client.csproj" --verbosity quiet

COPY . .
WORKDIR "/src/GoodHamburguer.Client"
RUN dotnet publish "GoodHamburguer.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=true

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

LABEL org.opencontainers.image.title="GoodHamburguer Client"
LABEL org.opencontainers.image.description="Blazor WebAssembly client for GoodHamburguer application"
LABEL org.opencontainers.image.version="1.0.0"

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/publish/wwwroot .
COPY GoodHamburguer.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5218

HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost:5218/ || exit 1

CMD ["nginx", "-g", "daemon off;"]

