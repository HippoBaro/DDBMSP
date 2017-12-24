FROM microsoft/dotnet:2-sdk-jessie AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . ./
WORKDIR /app/DDBMSP.Silo
RUN dotnet restore

# copy sources and build
RUN dotnet publish -c Release -o /app/out
COPY DDBMSP.Silo/OrleansConfiguration.xml /app/out

# copy bins
COPY Bin/ /app/out

# build runtime image
FROM microsoft/dotnet:2-runtime
WORKDIR /app
COPY --from=build-env /app/out ./

# set launch params
ENV BOOTSTRAP false

RUN ["chmod", "+x", "/entrypoint"]
ENTRYPOINT ["/entrypoint"]