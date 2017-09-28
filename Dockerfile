FROM microsoft/aspnetcore-build:2.0.0

ENV APPLICATION_NAME=simulation-csharp-client
COPY . /tmp
WORKDIR /tmp
ARG key

# Restore main projects
RUN dotnet restore src/SimulationCSharpClient

# Build Release
RUN dotnet build --configuration Release src/SimulationCSharpClient

# Build nuget package
RUN dotnet pack --no-build --configuration Release src/SimulationCSharpClient

# Publish
RUN dotnet nuget push src/SimulationCSharpClient/bin/Release/SimulationCSharpClient.0.1.1.nupkg -s https://www.nuget.org/api/v2/package -k $key



