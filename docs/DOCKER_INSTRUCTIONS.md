# Docker Instructions

## Run Development Container w/ HTTPS and User Secrets
- Similar to production container, but dev certs are not production-ready.
- Free certificates for production can be generated using the Let's Encrypt API and client (ACME).

1. First set the user secrets for the `ui` project. Also make sure to setup the database connection string using `dotnet user-secrets`.

```powershell
# Read dev cert password in to environment variable
$env:KestrelCertificatePassword=(Read-Host -Prompt "Enter your password now" -MaskInput)

# Store user secret for the `ui.csproj` project scope
dotnet user-secrets -p .\ui\ui.csproj set "Kestrel:Certificates:Default:Password" $env:KestrelCertificatesPassword

# Set the database connection string for Eddy, replace connection string with your information
dotnet user-secrets -p .\ui\ui.csproj set "ConnectionStrings:EddyWorkNotes" "<<EF core provider connecting string>>"
```

2. Make sure to build the container and pull the latest ASP.NET base images.

```powershell
cd .\ui
docker build --pull -t eddypmassistant .
```

3. Use Docker volumes to mount the secrets and user profile when running the container.
- set the timezone environment variable `TZ` to your local timezone
- set the `EDDY_POSTGRES_HOST` environment variable to the hostname of your Postgres database instance, if it isn't the default: `localhost`.
- port assignment defaults to 5432

The following example is configured with `America/Chicago` as the timezone and `eddy-postgres-db-1` as the database hostname. 

```powershell
docker run --rm -it `
   -p 8001:8001 `
   -e ASPNETCORE_HTTPS_PORTS=8001 `
   -e ASPNETCORE_ENVIRONMENT=Development `
   -v ${env:APPDATA}/microsoft/UserSecrets/:/home/app/.microsoft/usersecrets `
   -v ${env:USERPROFILE}/.aspnet/https/:/https/ `
   -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx `
   --network eddy-postgres_default `
   -e EDDY_POSTGRES_HOST=eddy-postgres-db-1 `
   -e TZ="America/Chicago" `
   eddypmassistant
```

## Docker Run commands in one line
These code blocks contain single-line commands without any escape symbols for line continuation, i.e. `\n and \\n
These are easier to copy, modify, and paste into command shell terminal windows or scripts w/o word wrap.

### Powershell 7
> tested on: Windows 11, $PSEdition="Core", PowerShell 7.5.4

   ```powershell
   docker run --rm -it -p 8001:8001 -e ASPNETCORE_HTTPS_PORTS=8001 -e ASPNETCORE_ENVIRONMENT=Development -v ${env:APPDATA}/microsoft/UserSecrets/:/home/app/.microsoft/usersecrets -v ${env:USERPROFILE}/.aspnet/https/:/https/ -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx --network eddy-postgres_default -e EDDY_POSTGRES_HOST=eddy-postgres-db-1 -e TZ="America/Chicago" eddypmassistant
   ```
