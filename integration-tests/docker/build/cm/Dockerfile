# escape=`

ARG BASE_IMAGE
ARG SXA_IMAGE
ARG SPE_IMAGE
ARG TOOLING_IMAGE
ARG HORIZON_RESOURCES_IMAGE
ARG DIANOGA_IMAGE

FROM ${TOOLING_IMAGE} as tooling
FROM ${SPE_IMAGE} as spe
FROM ${SXA_IMAGE} as sxa
FROM ${HORIZON_RESOURCES_IMAGE} as horizon_resources
FROM ${DIANOGA_IMAGE} as dianoga
FROM ${BASE_IMAGE}


SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]



# Copy development tools and entrypoint
COPY --from=tooling \tools\ \tools\

WORKDIR C:\inetpub\wwwroot

# Add Dianoga module

#RUN Powershell Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
#RUN choco install vcredist-all -y
#COPY --from=dianoga \module\cd\content .\

# Add SPE module
COPY --from=spe \module\cm\content .\

# Add SXA module
COPY --from=sxa \module\cm\content .\
COPY --from=sxa \module\tools \module\tools
RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `
    Remove-Item -Path C:\module -Recurse -Force;

# Add horizon module
COPY --from=horizon_resources \module\cm\content \inetpub\wwwroot

# Disable Media Request Protection for simplicity of testing (no need to worry about hash get parameter)
RUN Powershell Rename-Item -Path "C:\inetpub\wwwroot\App_Config\Sitecore\CMS.Core\Sitecore.Media.RequestProtection.config" -NewName "Sitecore.Media.RequestProtection.config.disabled"