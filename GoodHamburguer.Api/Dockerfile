FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5010
ENV ASPNETCORE_URLS=http://+:5010

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GoodHamburguer.Api/GoodHamburguer.Api.csproj", "GoodHamburguer.Api/"]
COPY ["GoodHamburguer.Domain/GoodHamburguer.Domain.csproj", "GoodHamburguer.Domain/"]
RUN dotnet restore "GoodHamburguer.Api/GoodHamburguer.Api.csproj"
COPY . .
WORKDIR "/src/GoodHamburguer.Api"
RUN dotnet build "GoodHamburguer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoodHamburguer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburguer.Api.dll"]

