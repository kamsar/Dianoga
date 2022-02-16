# escape=`

ARG BASE_IMAGE=mcr.microsoft.com/windows/nanoserver:1809
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/framework/sdk:4.8

FROM ${BUILD_IMAGE} AS build-env
ARG BUILD_CONFIGURATION=
ARG DOTNET_VERSION_ARG=

WORKDIR /
# Add Sitecore Nuget source
RUN dotnet nuget add source https://sitecore.myget.org/F/sc-packages/api/v3/index.json

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY src/Dianoga/Dianoga.csproj ./src/Dianoga/
COPY src/Dianoga.Tests/Dianoga.Tests.csproj ./src/Dianoga.Tests/
RUN dotnet restore

# Copy everything else and build
COPY src ./src
RUN dotnet build -c $env:BUILD_CONFIGURATION
RUN mkdir ./src/bin
RUN Copy ./src/Dianoga/bin/$env:BUILD_CONFIGURATION/$env:DOTNET_VERSION_ARG/Dianoga.* ./src/bin/
RUN Copy ./src/Dianoga/bin/$env:BUILD_CONFIGURATION/$env:DOTNET_VERSION_ARG/System.Threading.Tasks.Dataflow.dll ./src/bin/

FROM ${BASE_IMAGE}

# Copy Dianoga dll and pdb (if present)
COPY --from=build-env /src/bin/ ./module/cd/content/bin/
COPY --from=build-env /src/bin/ ./module/cm/content/bin/


# Copy Dianoga Tools
ARG src="/src/Dianoga/Dianoga Tools"
ARG target="./module/cd/content/App_Data/Dianoga Tools"
COPY --from=build-env ${src} ${target}

ARG src="/src/Dianoga/Dianoga Tools"
ARG target="./module/cd/content/App_Data/Dianoga Tools"
COPY --from=build-env ${src} ${target}

# Copy Configs
ARG src="/src/Dianoga/Default Config Files"
ARG target="./module/cm/content/App_Config/Include/Dianoga"
COPY --from=build-env ${src} ${target}

ARG src="/src/Dianoga/Default Config Files"
ARG target="./module/cd/content/App_Config/Include/Dianoga"
COPY --from=build-env ${src} ${target}
