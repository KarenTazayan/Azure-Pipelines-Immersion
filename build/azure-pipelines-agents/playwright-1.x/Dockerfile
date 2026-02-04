# The image is based on Ubuntu 24.04
FROM mcr.microsoft.com/playwright/python:v1.58.0-noble

ENV TARGETARCH=linux-x64
ENV RUN_AGENT_ONCE=False

RUN DEBIAN_FRONTEND=noninteractive apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get upgrade -y && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
    apt-transport-https apt-utils ca-certificates \
    curl git iputils-ping jq lsb-release openssh-client \
    software-properties-common

# Install the .NET SDK 10.0
RUN add-apt-repository ppa:dotnet/backports && \
    apt-get update && apt-get install -y dotnet-sdk-10.0

WORKDIR /azp

COPY ./start.sh .
RUN chmod +x start.sh

# Create u1 user and set up home directory
RUN useradd -m -d /home/u1 u1 \
    && chown -R u1:u1 /azp /home/u1

USER u1

ENTRYPOINT [ "./start.sh" ]