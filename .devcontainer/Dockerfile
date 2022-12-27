# Find the Dockerfile at this URL
# https://github.com/devcontainers/images/tree/main/src/dotnet
FROM mcr.microsoft.com/devcontainers/dotnet:6.0

## AZURE FUNCTIONS CORE TOOLS ##
# Uncomment the below to install Azure Functions Core Tools
RUN apt-get update && apt-get upgrade -y && \
    apt-get install -y nodejs \
    npm
RUN npm i -g azure-functions-core-tools@4 --unsafe-perm true
