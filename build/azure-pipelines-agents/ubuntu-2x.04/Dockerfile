FROM ubuntu:24.04

RUN DEBIAN_FRONTEND=noninteractive apt-get update
RUN DEBIAN_FRONTEND=noninteractive apt-get upgrade -y

RUN DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
    apt-transport-https \
    apt-utils \
    ca-certificates \
    curl \
    git \
    iputils-ping \
    jq \
    lsb-release \
    software-properties-common

# Install the Azure CLI
# RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
WORKDIR /azcli
COPY ./az_cli_install.sh .
RUN chmod +x az_cli_install.sh
RUN ./az_cli_install.sh
ENV AZURE_CLI=True
# Fetch the latest Bicep CLI binary
RUN curl -Lo bicep https://github.com/Azure/bicep/releases/latest/download/bicep-linux-x64
# Mark it as executable
RUN chmod +x ./bicep
# Add bicep to your PATH (requires admin)
RUN mv ./bicep /usr/local/bin/bicep

# Install the .NET SDK 9.0
RUN apt-get install wget
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt-get update && apt-get install -y dotnet-sdk-9.0

# https://blog.jetbrains.com/dotnet/2020/07/16/resharper-command-line-tools-cross-platform-and-global-tools/#usage-as-global-tools
RUN dotnet tool install JetBrains.ReSharper.GlobalTools --tool-path /resharpercli --version 2025.1.3
ENV JETBRAINS_RESHARPER_GLOBALTOOLS=2025.1.3

# Install PowerShell
RUN apt-get install -y powershell

# Install Docker Engine on Debian
# Use Docker within a Docker container
# Warning!!! Doing this has serious security implications. 
# The code inside the container can now run as root on your Docker host.
RUN curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
RUN add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable"
RUN apt-cache policy docker-ce
RUN apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Can be 'linux-x64', 'linux-arm64', 'linux-arm', 'rhel.6-x64'.
ENV TARGETARCH=linux-x64

WORKDIR /azp

# Run a self-hosted Azure Pipelines agent in Docker
COPY ./start.sh .
RUN chmod +x start.sh

# Skip creating a restricted user for running Docker in Docker.
# Warning! Doing Docker within a Docker by using Docker socket has serious security implications. 
# The code inside the container can now run as root on your Docker host. Please be very careful.

ENV AGENT_ALLOW_RUNASROOT="true"

# Create agent user and set up home directory
# RUN useradd -m -d /home/agent agent
# RUN chown -R agent:agent /azp /home/agent

ENTRYPOINT [ "./start.sh" ]