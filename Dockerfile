# Use the Microsoft-provided .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0

# Set the working directory in the container to /app
WORKDIR /app

# Copy the .csproj file(s) and restore dependencies
COPY *.csproj ./
RUN dotnet restore /p:EnableWindowsTargeting=true

# Copy the rest of the application files
COPY . ./

ENTRYPOINT ["dotnet", "publish", "-p:PublishProfile=FolderProfile", "-p:EnableWindowsTargeting=true"]