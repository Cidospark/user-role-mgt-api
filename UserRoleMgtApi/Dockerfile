FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 5003
COPY --from=build /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet UserRoleMgtApi.Core.dll
