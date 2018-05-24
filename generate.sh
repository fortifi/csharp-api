#!/bin/bash
# File: generate.sh
# Description: run to update api from swagger

set -e

# Make sure dependency is installed
command -v dotnet >/dev/null 2>&1 || { echo "'dotnet' is required but missing. (https://www.microsoft.com/net/download/macos)" >&2; exit 1; }
command -v nswag >/dev/null 2>&1 || { echo "'nswag' tool is required but missing. (npm install nswag -g)" >&2; exit 1; }

# Get latest swagger spec from citadel
echo -e "\033[4mGetting latest swagger...\033[0m"
rm -f swagger.yaml
wget "https://api.fortifi.io/swagger.yaml"

# Generate new client from spec
rm -rf FortifiAPI/FortifiClient.cs
echo -e "\033[4mGenerating fortifi API from Swagger spec...\033[0m"
(nswag swagger2csclient /input:swagger.yaml /classname:Client /Namespace:FortifiAPI /output:FortifiAPI/FortifiClient.cs)
