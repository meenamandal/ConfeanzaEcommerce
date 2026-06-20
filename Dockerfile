# ── Stage 1: Build ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore dependencies first (better layer caching)
COPY ConfeanzaEcommerce/ConfeanzaEcommerce.csproj ConfeanzaEcommerce/
RUN dotnet restore ConfeanzaEcommerce/ConfeanzaEcommerce.csproj

# Copy source and publish
COPY ConfeanzaEcommerce/ ConfeanzaEcommerce/
WORKDIR /src/ConfeanzaEcommerce
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ConfeanzaEcommerce.dll"]
