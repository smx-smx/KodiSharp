project(KodiSharp)
cmake_minimum_required(VERSION 3.18)

include(FetchContent)

FetchContent_Declare(
    EzDotNet
    GIT_REPOSITORY https://github.com/smx-smx/EzDotnet.git
    GIT_TAG 88bcb516a14199fbed3afdb0d3f560ea02901112
    GIT_PROGRESS TRUE
)
FetchContent_MakeAvailable(EzDotNet)

FetchContent_GetProperties(EzDotNet
    SOURCE_DIR EzDotNet_SOURCE_DIR
)

list(APPEND CMAKE_MODULE_PATH "${EzDotNet_SOURCE_DIR}/cmake")
find_package(Dotnet REQUIRED)

set(plugin_framework netstandard2.0)
set(runtime_framework net8.0)


if(LINUX)
    set(_rid_os linux)
elseif(WIN32)
    set(_rid_os win)
elseif(APPLE)
    set(_rid_os osx)
else()
    message(FATAL_ERROR "Unknown operating system")
endif()

if(CMAKE_SYSTEM_PROCESSOR MATCHES "x86_64|AMD64")
    set(_rid_cpu x64)
elseif(CMAKE_SYSTEM_PROCESSOR MATCHES "i386|i686|x86")
    set(_rid_cpu x86)
elseif(CMAKE_SYSTEM_PROCESSOR MATCHES "arm64|aarch64")
    set(_rid_cpu arm64)
elseif(CMAKE_SYSTEM_PROCESSOR MATCHES "arm")
    set(_rid_cpu arm)
else()
    message(FATAL_ERROR "Unknown CPU architecture: ${CMAKE_SYSTEM_PROCESSOR}")
endif()

set(rid ${_rid_os}-${_rid_cpu})

function(add_plugin)
    cmake_parse_arguments(plg "" "NAME;PROJECT" "" ${ARGN})
    add_dotnet(${plg_PROJECT}
        CONFIG ${CMAKE_BUILD_TYPE}
        FRAMEWORK ${plugin_framework}
        TARGET_NAME ${plg_NAME}
    )

    set(plugin_binary_dir
        ${CMAKE_CURRENT_BINARY_DIR}/${plg_NAME}/${plugin_framework}/
    )
    install(DIRECTORY ${plugin_binary_dir} DESTINATION bin/TestPlugin)
endfunction()

add_dotnet(KodiInterop/KodiInterop/KodiInterop.csproj
    CONFIG ${CMAKE_BUILD_TYPE}
    FRAMEWORK ${runtime_framework}
    RID ${rid}
    PUBLISH
)

set(runtime_binary_dir
    ${CMAKE_CURRENT_BINARY_DIR}/KodiInterop/${runtime_framework}/${rid}/publish/)


install(DIRECTORY ${runtime_binary_dir} DESTINATION bin/KodiInterop)
add_plugin(
    NAME TestPlugin
    PROJECT KodiInterop/TestPlugin/TestPlugin.csproj
)