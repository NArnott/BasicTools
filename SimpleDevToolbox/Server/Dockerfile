#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SimpleDevToolbox/Server/SimpleDevToolbox.Server.csproj", "SimpleDevToolbox/Server/"]
COPY ["SimpleDevToolbox/Shared/SimpleDevToolbox.Shared.csproj", "SimpleDevToolbox/Shared/"]
COPY ["SimpleDevToolbox/Client/SimpleDevToolbox.Client.csproj", "SimpleDevToolbox/Client/"]
RUN dotnet restore "SimpleDevToolbox/Server/SimpleDevToolbox.Server.csproj"
COPY . .
WORKDIR "/src/SimpleDevToolbox/Server"
RUN dotnet build "SimpleDevToolbox.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleDevToolbox.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleDevToolbox.Server.dll"]