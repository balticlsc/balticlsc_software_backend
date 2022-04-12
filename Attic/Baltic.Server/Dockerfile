FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 8050

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Baltic.Server/Baltic.Server.csproj", "Baltic.Server/"]
COPY ["Baltic.ProtocolBuffers/Baltic.ProtocolBuffers.csproj", "Baltic.ProtocolBuffers/"]
COPY ["Baltic.Node.Services/Baltic.Node.Services.csproj", "Baltic.Node.Services/"]
COPY ["Baltic.Core/Baltic.Core.csproj", "Baltic.Core/"]
COPY ["Baltic.Database/Baltic.Server.Database.csproj", "Baltic.Database/"]
RUN dotnet restore "Baltic.Server/Baltic.Server.csproj"
COPY . .
WORKDIR "/src/Baltic.Server"
RUN dotnet build "Baltic.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Baltic.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["Baltic.Server/Baltic.Server.xml", "."]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Baltic.Server.dll"]
