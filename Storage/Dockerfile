FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
EXPOSE 80


# copy everything else and build app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
COPY --from=build /app/out .
# ENTRYPOINT ["dotnet", "Storage.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Storage.dll