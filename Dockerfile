# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Offline packages + config
COPY nuget-packages/ /nuget-packages/
COPY NuGet.config ./
COPY apenew.sln ./
COPY apenew.csproj ./

RUN dotnet restore apenew.csproj \
    --configfile NuGet.config \
    --packages /nuget-packages \
    --verbosity minimal

COPY . .

RUN dotnet publish apenew.csproj -c Release -o /app/out --no-restore

# =========================
# Runtime stage (ASP.NET + Postgres)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && \
    apt-get install -y postgresql postgresql-client && \
    rm -rf /var/lib/apt/lists/*

ENV POSTGRES_USER=postgres \
    POSTGRES_PASSWORD=postgres \
    POSTGRES_DB=assessmentdb \
    PGDATA=/var/lib/postgresql/data

RUN mkdir -p "$PGDATA" && chown -R postgres:postgres /var/lib/postgresql

COPY --from=build /app/out .
COPY appsettings.yaml .
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["./entrypoint.sh"]
