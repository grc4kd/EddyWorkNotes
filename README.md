# Prerequisites
- Setup a postgres database container with Docker and `postgres`.
- Setup connection information in app secrets for `ui` project using `dotnet user-secrets` tool.
- Setup dev cert and export to password-protected file as shown here: https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md
- Store this password in `$DEV_CERT_PASSWORD` before running the docker container.
- And following those instructions, run the container using dev secrets for local Docker development using HTTPS and user secrets.

# Important Note
This setup is not sufficient for production mode security, but it is a good testbed for running the app inside of a docker container.

# Web App w/ Local Database Container
```powershell
docker run --rm -it `
    --network eddy-postgres_default `
    -e "EDDY_POSTGRES_HOST=eddy-postgres-db-1" `
    -e "TZ=America/Chicago" `
    -p 8000:80 `
    -p 8001:443 `
    -e ASPNETCORE_HTTPS_PORTS=8001 `
    -e ASPNETCORE_URLS="https://+;http://+" `
    -e ASPNETCORE_ENVIRONMENT=Development `
    -v ${env:APPDATA}/microsoft/UserSecrets/:/home/app/.microsoft/usersecrets `
    -v $env:USERPROFILE\.aspnet\https:/https/ `
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx `
    -e ASPNETCORE_Kestrel__Certificates__Default__Password="$DEV_CERT_PASSWORD" `
    eddypmassistant:latest
```

> NOTE: The name of the network `eddy-postgres_default` and internal database host `eddy-postgres-db-1` may change depending on the docker and host networks.
