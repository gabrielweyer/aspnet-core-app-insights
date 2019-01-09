FROM microsoft/dotnet:2.2.102-sdk-alpine3.8 AS build
WORKDIR /app

# Copy everything, restore Docker.Downstream and publish
COPY . ./
RUN dotnet restore ./samples/Docker.Downstream/
RUN dotnet publish -c Release -o /out ./samples/Docker.Downstream/

# Build runtime image
FROM microsoft/dotnet:2.2.1-aspnetcore-runtime-alpine3.8
RUN mkdir -p /tmp/app-logs
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Docker.Downstream.dll"]
