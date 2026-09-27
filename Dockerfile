# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Server/BusReservationService/BusReservationService.csproj", "Server/BusReservationService/"]
COPY ["Infra/Database/BusReservationService.Database.csproj", "Infra/Database/"]
COPY ["BusReservationService.Domain/BusReservationService.Domain.csproj", "BusReservationService.Domain/"]
COPY ["BusReservationService.Share/BusReservationService.Share.csproj", "BusReservationService.Share/"]

RUN dotnet restore "Server/BusReservationService/BusReservationService.csproj"

COPY . .

RUN dotnet publish \
    "Server/BusReservationService/BusReservationService.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "BusReservationService.dll"]