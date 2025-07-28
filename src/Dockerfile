FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app
 
# Copy the .csproj files and restore dependencies
COPY Shapy/Shapy.API/*.csproj ./Shapy/Shapy.API/
COPY Shapy/Shapy.Application/*.csproj ./Shapy/Shapy.Application/
COPY Shapy/Shapy.Domain/*.csproj ./Shapy/Shapy.Domain/
COPY Shapy/Shapy.Infrastructure/*.csproj ./Shapy/Shapy.Infrastructure/

# Restore the dependencies
RUN dotnet restore ./Shapy/Shapy.API/Shapy.API.csproj
 
# Copy the rest of the application code
COPY Shapy/ ./Shapy/
 
# Build the project
WORKDIR /app/Shapy/Shapy.API/

RUN dotnet publish -c Release -o out
 
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app/

COPY --from=build-env /app/Shapy/Shapy.API/out .
 
# Run and expose the application.
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Shapy.API.dll"]