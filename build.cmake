cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

list(INSERT CMAKE_MODULE_PATH 0 "${CMAKE_CURRENT_LIST_DIR}/cmake/msbuild2cmake/cmake")
include(msbuild2cmake)

if(NOT DEFINED CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Debug)
endif()

set(build_dir ${CMAKE_CURRENT_LIST_DIR}/build)

invoke_cmake(
    BUILD_DIR ${build_dir}
    DIRECTORY ${CMAKE_CURRENT_LIST_DIR}
    PASS_VARIABLES
		CMAKE_BUILD_TYPE
    EXTRA_ARGUMENTS
        -DCMAKE_INSTALL_PREFIX=${build_dir}/out
)

execute_process(
    COMMAND ${CMAKE_COMMAND} --install ${build_dir}
    RESULT_VARIABLE retcode
)

if(NOT ${retcode} EQUAL 0)
    message(FATAL_ERROR "cmake --install failed with exit code ${retcode}")
endif()