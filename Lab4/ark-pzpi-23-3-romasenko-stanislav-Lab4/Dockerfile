FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["RoomBook/RoomBook.csproj", "RoomBook/"]
COPY ["RoomBook.Core/RoomBook.Core.csproj", "RoomBook.Core/"]

RUN dotnet restore "RoomBook/RoomBook.csproj"

COPY . .

WORKDIR "/src/RoomBook"
RUN dotnet build "RoomBook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomBook.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomBook.dll"]