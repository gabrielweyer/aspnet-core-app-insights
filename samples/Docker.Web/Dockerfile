FROM microsoft/dotnet:2.2.102-sdk-alpine3.8 AS build
WORKDIR /app

# Copy everything, restore Docker.Web and publish
COPY . ./
RUN dotnet restore ./samples/Docker.Web/
RUN dotnet publish -c Release -o /out ./samples/Docker.Web/

# Build runtime image
FROM microsoft/dotnet:2.2.1-aspnetcore-runtime-alpine3.8
RUN mkdir -p /tmp/app-logs
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Docker.Web.dll"]
