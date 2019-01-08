FROM microsoft/dotnet:2.2.100-sdk-alpine3.8 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o /out

# Build runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine3.8
RUN mkdir -p /tmp/app-logs
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Docker.Downstream.dll"]
