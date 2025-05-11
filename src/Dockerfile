# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy everything
COPY . .

# restore and build
RUN dotnet restore
RUN dotnet publish ./PromomashTestTask.API/PromomashTestTask.API.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "PromomashTestTask.API.dll"]