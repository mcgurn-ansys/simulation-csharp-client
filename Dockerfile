FROM microsoft/aspnetcore-build:2.0.0

ENV APPLICATION_NAME=simulation-csharp-client
ENV PACKAGE=SimulationCSharpClient
ENV SRC=src/${PACKAGE}
ENV TEST=test/${PACKAGE}.Test
COPY . /tmp
WORKDIR /tmp
ARG version

# Run unit tests
RUN dotnet test ${TEST}

# Build Release
RUN dotnet build --configuration Release ${SRC}

# Create nuget package
RUN dotnet pack --no-build --configuration Release ${SRC}

# Publish
RUN curl -X POST "https://www.myget.org/F/3dsim-utility/api/v2/package" -H "X-NuGet-ApiKey: 14dd5115-5433-4da8-b8a3-69fe02cf49cf" -T ${SRC}/bin/Release/${PACKAGE}.${version}.nupkg
