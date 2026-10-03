FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props .editorconfig Olevin.slnx ./
COPY Olevin.Api/Olevin.Api.csproj Olevin.Api/packages.lock.json ./Olevin.Api/
RUN dotnet restore Olevin.Api/Olevin.Api.csproj --locked-mode

COPY Olevin.Api/ ./Olevin.Api/
RUN dotnet publish Olevin.Api/Olevin.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-azurelinux3.0-distroless AS runtime
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "Olevin.Api.dll"]
