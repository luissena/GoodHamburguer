FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GoodHamburguer.Client/GoodHamburguer.Client.csproj", "GoodHamburguer.Client/"]
RUN dotnet restore "GoodHamburguer.Client/GoodHamburguer.Client.csproj"
COPY . .
WORKDIR "/src/GoodHamburguer.Client"
RUN dotnet publish "GoodHamburguer.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY GoodHamburguer.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 5218
CMD ["nginx", "-g", "daemon off;"]

