FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RoomsBooking.API/RoomsBooking.API.csproj", "RoomsBooking.API/"]
COPY ["RoomsBooking.Infrastructure/RoomsBooking.Infrastructure.csproj", "RoomsBooking.Infrastructure/"]
COPY ["RoomsBooking.Application/RoomsBooking.Application.csproj", "RoomsBooking.Application/"]
COPY ["RoomsBooking.Domain/RoomsBooking.Domain.csproj", "RoomsBooking.Domain/"]
RUN dotnet restore "RoomsBooking.API/RoomsBooking.API.csproj"
COPY . .
WORKDIR "/src/RoomsBooking.API"
RUN dotnet build "./RoomsBooking.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RoomsBooking.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomsBooking.API.dll"]
