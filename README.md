# Prerequisites
- Setup a postgres database container with Docker and `postgres`.
- Setup connection information in app secrets for `ui` project using `dotnet user-secrets` tool.
- Setup dev cert and export to password-protected file as shown here: https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md
- Store this password in `$DEV_CERT_PASSWORD` for docker run command.
- And following those instructions, run the container using dev secrets for local Docker development using HTTPS and user secrets.

# Important Note
This setup is not sufficient for production mode security, but it is a good testbed for running the app inside of a docker container.

# Web App w/ Local Database - Docker Run Command
```powershell
docker run --rm -it `
    --network postgres-net `
    -p 8001:8001 `
    -e ASPNETCORE_HTTPS_PORTS=8001 `
    -e ASPNETCORE_ENVIRONMENT=Development `
    -v ${env:APPDATA}/microsoft/UserSecrets/:/home/app/.microsoft/usersecrets `
    -v ${env:USERPROFILE}/.aspnet/https/:/https/ `
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx `
    -e ASPNETCORE_Kestrel__Certificates__Default__Password="$DEV_CERT_PASSWORD" `
    -e EDDY_POSTGRES_HOST=eddy-postgres `
    -e TZ=America/Chicago `
    eddypmassistant:latest
```

# Interesting Note
The docker container runs with international settings, so time is formatted as HH:MM in "military time",
instead of the more common AM/PM format found in some countries, like my own. Please note you can adjust
the TZ=America/Chicago setting in the docker run command to match your local timezone, while timestamps
themselves are stored as UTC time codes in the database layer.