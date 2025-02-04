#!/usr/bin/env bash
cd "$(dirname "$(readlink -f "$0")")"
cmake -P build.cmake
