FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
EXPOSE 80

# create folder for temporary storing repos
RUN mkdir serialized

# copy everything else and build app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "RepoAPI.dll"]