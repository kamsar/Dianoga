# escape=`

ARG BASE_IMAGE
ARG SXA_IMAGE
ARG SPE_IMAGE

FROM ${SPE_IMAGE} as spe
FROM ${SXA_IMAGE} as sxa
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Add SPE module
COPY --from=spe \module\db \spe_data
RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\spe_data; `
    Remove-Item -Path C:\spe_data -Recurse -Force;

# Add SXA module
COPY --from=sxa \module\db \sxa_data
COPY --from=sxa \module\db\Sitecore.master.dacpac \sxa_data\Sitecore.web.dacpac
RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\sxa_data; `
    Remove-Item -Path C:\sxa_data -Recurse -Force;

# Add Test data
COPY Sitecore.master.dacpac \test_data\Sitecore.master.dacpac
COPY Sitecore.master.dacpac \test_data\Sitecore.web.dacpac

RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\test_data; `
    Remove-Item -Path C:\test_data -Recurse -Force;