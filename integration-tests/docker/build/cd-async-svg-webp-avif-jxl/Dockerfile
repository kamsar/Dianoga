# escape=`

ARG BASE_IMAGE
ARG SXA_IMAGE
ARG TOOLING_IMAGE
ARG DIANOGA_IMAGE

FROM ${TOOLING_IMAGE} as tooling
FROM ${SXA_IMAGE} as sxa
FROM ${DIANOGA_IMAGE} as dianoga
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Copy development tools and entrypoint
COPY --from=tooling \tools\ \tools\

WORKDIR C:\inetpub\wwwroot

# Add SXA module
COPY --from=sxa \module\cd\content .\
COPY --from=sxa \module\tools \module\tools
RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `
    Remove-Item -Path C:\module -Recurse -Force;

# Add Dianoga module
# Install Microsoft Visual C++ Runtime, because image tools(e.g. pngquant.exe, avifenc.exe) depends on it.
# Probably, less generic Visual C++ Runtime will be OK.
RUN Powershell Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
RUN choco install vcredist-all -y
COPY --from=dianoga \module\cd\content .\

# Enable SVG optimization
RUN Rename-Item -Path C:\inetpub\wwwroot\App_Config\Modules\Dianoga\Dianoga.Svg.config.disabled -NewName Dianoga.Svg.config
# Enable WebP optimization
RUN Rename-Item -Path C:\inetpub\wwwroot\App_Config\Modules\Dianoga\z.01.Dianoga.NextGenFormats.WebP.config.disabled -NewName z.01.Dianoga.NextGenFormats.WebP.config
# Enable AVIF optimization
RUN Rename-Item -Path C:\inetpub\wwwroot\App_Config\Modules\Dianoga\z.02.Dianoga.NextGenFormats.Avif.config.disabled -NewName z.02.Dianoga.NextGenFormats.Avif.config.config
# Enable JXL optimization
RUN Rename-Item -Path C:\inetpub\wwwroot\App_Config\Modules\Dianoga\z.03.Dianoga.NextGenFormats.JpegXL.config.disabled -NewName z.03.Dianoga.NextGenFormats.JpegXL.config

# Change default Media Request Handler to be able to use NextGenFormats
COPY Web.config.xdt ./Web.config.Dianoga.xdt
RUN Powershell C:\tools\scripts\Invoke-XdtTransform.ps1 -Path Web.config -XdtPath Web.config.Dianoga.xdt

# Disable Media Request Protection for simplicity of testing (no need to worry about hash get parameter)
RUN Powershell Rename-Item -Path "C:\inetpub\wwwroot\App_Config\Sitecore\CMS.Core\Sitecore.Media.RequestProtection.config" -NewName "Sitecore.Media.RequestProtection.config.disabled"