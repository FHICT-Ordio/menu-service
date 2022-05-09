#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=https://+:1001
EXPOSE 1001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["menu-service/menu-service.csproj", "menu-service/"]
RUN dotnet restore "menu-service/menu-service.csproj"
COPY . .
WORKDIR "/src/menu-service"
RUN dotnet build "menu-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "menu-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "menu-service.dll"]