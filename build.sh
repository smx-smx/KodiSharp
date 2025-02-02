#!/usr/bin/env bash
# rudimentary bash script to build the project
# FIXME: convert to CMakeLists.txt, and make it cross platform
cd "$(dirname "$(readlink -f "$0")")"
echo "==> building project"
dotnet build -c Debug -f netstandard2.0 KodiInterop/TestPlugin.sln
echo "==> publishing assembly"
dotnet publish -c Debug -f net8.0 -r linux-x64 KodiInterop/KodiInterop/KodiInterop.csproj
