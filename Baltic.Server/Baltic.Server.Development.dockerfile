FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Install ASP.NET Core
RUN aspnetcore_version=3.1.4 \
    && wget -O aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='f60e9226a5b399470479fd6fdebd03442b0440128be1090adcbe473dba46a3e7a57a9e59b4abff96214e0dd0b1123c67fe764b74c61de1cb35c8b8ac45767eb9' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# Env variable
ENV ConnectionStrings:DefaultConnection="User ID=baltic;Password=somepass;Host=postgresql;Port=5432;Database=baltic;Pooling=true;"
ENV ASPNETCORE_ENVIRONMENT="Development"

# Add default cert
RUN dotnet dev-certs https
# RUN dotnet dev-certs https --trust

EXPOSE 5000
EXPOSE 5001

# Copy project files
WORKDIR /app
COPY ./bin/Debug/netcoreapp3.1 .
ENTRYPOINT ["dotnet", "Baltic.Server.dll"]
