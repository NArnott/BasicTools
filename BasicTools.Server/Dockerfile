#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BasicTools.Server/BasicTools.Server.csproj", "BasicTools.Server/"]
COPY ["BasicTools.Shared/BasicTools.Shared.csproj", "BasicTools.Shared/"]
COPY ["BasicTools.Client/BasicTools.Client.csproj", "BasicTools.Client/"]
RUN dotnet restore "BasicTools.Server/BasicTools.Server.csproj"
COPY . .
WORKDIR "/src/BasicTools.Server"
RUN dotnet build "BasicTools.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasicTools.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasicTools.Server.dll"]