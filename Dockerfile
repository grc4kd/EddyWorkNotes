# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /source

# Copy project file and restore as distinct layers
COPY --link eddy-pm-assistant.sln .
COPY --link DataEntities/*.csproj DataEntities/
COPY --link ui/*.csproj ui/
COPY --link app/*.csproj app/
COPY --link test/*.csproj test/
COPY --link PlaywrightTests/*.csproj PlaywrightTests/

RUN dotnet restore eddy-pm-assistant.sln -a $TARGETARCH

# Copy source code and publish app
COPY --link ui/ ui/
COPY --link DataEntities/ DataEntities/
COPY --link app/ app/
COPY --link test/ test/
COPY --link PlaywrightTests/ PlaywrightTests/

# Test stage -- exposes optional entrypoint
# Target entrypoint with: docker build --target test
FROM build AS test

COPY --link test/*.csproj test/
WORKDIR /source/test
RUN dotnet restore

COPY --link test/ .
RUN dotnet build --no-restore

ENTRYPOINT ["dotnet", "test", "--logger:trx", "--no-build"]

FROM build AS publish
WORKDIR /source
RUN dotnet publish ui/ui.csproj -a $TARGETARCH --no-restore -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 5086
COPY --link --from=publish /app .
USER $APP_UID
ENTRYPOINT [ "./ui" ]