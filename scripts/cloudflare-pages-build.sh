#!/usr/bin/env bash
# Build command for Cloudflare Pages: bash scripts/cloudflare-pages-build.sh
# Output directory: publish/wwwroot
#
# Cloudflare's build image doesn't include .NET, so this installs the SDK
# locally and then publishes the Blazor WebAssembly app as static files.
set -euo pipefail

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

DOTNET_DIR="$PWD/.dotnet"
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-${HOME:-$PWD}}"

curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
bash /tmp/dotnet-install.sh --channel 10.0 --install-dir "$DOTNET_DIR" --no-path
"$DOTNET_DIR/dotnet" --version

"$DOTNET_DIR/dotnet" publish src/DijkstraSimulator.Web --configuration Release --output publish

# Cloudflare compresses responses itself, so the precompressed copies aren't needed
find publish/wwwroot -type f \( -name '*.br' -o -name '*.gz' \) -delete
echo "Built $(find publish/wwwroot -type f | wc -l) files into publish/wwwroot"
