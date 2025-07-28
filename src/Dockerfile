FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
RUN groupadd -g 1001 appgroup && useradd -u 1001 -g appgroup -m appuser


WORKDIR /app
 
# Copy the .csproj files and restore dependencies
COPY src/Shapy/Shapy.API/*.csproj ./Shapy/Shapy.API/
COPY src/Shapy/Shapy.Application/*.csproj ./Shapy/Shapy.Application/
COPY src/Shapy/Shapy.Domain/*.csproj ./Shapy/Shapy.Domain/
COPY src/Shapy/Shapy.Infrastructure/*.csproj ./Shapy/Shapy.Infrastructure/

# Restore the dependencies
RUN dotnet restore ./Shapy/Shapy.API/Shapy.API.csproj
 
# Copy the rest of the application code
COPY src/Shapy/ ./Shapy/
 
# Build the project
WORKDIR /app/Shapy/Shapy.API/

RUN dotnet publish -c Release -o out --no-restore
 
FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN groupadd -g 1001 appgroup && useradd -u 1001 -g appgroup -m appuser


WORKDIR /app/

COPY --from=build-env /app/Shapy/Shapy.API/out .
 
# Run and expose the application.
EXPOSE 5287
RUN chown -R 1001:1001 /app
USER 1001:1001

ENTRYPOINT ["dotnet", "Shapy.API.dll"]