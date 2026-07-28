# ── Stage 1: Build ──────────────────────────────────────────────────────────

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# rarely changes so higher so that caches each layer

COPY ["SGPA-CALCULATOR.csproj", "./"]
RUN dotnet restore "SGPA-CALCULATOR.csproj"


# Now lets copy all source code.

COPY . .

# Publish command prepare for dotnet applciation for deployment
# we use release mode not debug mode and not again running dotnet restore
# saved in the -o /app/publish (Output Directory)

RUN dotnet publish "SGPA-CALCULATOR.csproj" \
  -c Release \
  -o /app/publish \
  --no-restore \
  /p:UseAppHost=false

# ── Stage 2: Runtime ────────────────────────────────────────────────────────

# aspnet:10.0 = only the ASP.NET runtime. No SDK, no compiler, no dotnet CLI.
# make directory /app and move to it

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/*

# containers run as root by default
RUN groupadd -r appuser && useradd -r -g appuser appuser

#we copy pass compile output only not sdk
COPY --from=build /app/publish .

# give non-root user permissons

RUN chown -R appuser:appuser /app
USER appuser
ENV ASPNETCORE_URLS=http://+:8080

# appsettings.Production.json loaded (via env vars in compose, not file).
ENV ASPNETCORE_ENVIRONMENT=Production

# Docker daemon runs this check inside the container every 30 seconds.
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

#Exec Form: Docker skips the shell entirely. It runs dotnet directly.--> this application becomes PID 1
ENTRYPOINT ["dotnet", "SGPA-CALCULATOR.dll"]

