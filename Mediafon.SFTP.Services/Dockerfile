#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mediafon.SFTP.Services/Mediafon.SFTP.Services.csproj", "Mediafon.SFTP.Services/"]
RUN dotnet restore "Mediafon.SFTP.Services/Mediafon.SFTP.Services.csproj"
COPY . .
WORKDIR "/src/Mediafon.SFTP.Services"
RUN dotnet build "Mediafon.SFTP.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mediafon.SFTP.Services.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mediafon.SFTP.Services.dll"]