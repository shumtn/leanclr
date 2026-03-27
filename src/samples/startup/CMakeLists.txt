cmake_minimum_required(VERSION 3.15)
project(startup CXX)

# Bring in the runtime library
add_subdirectory(../../runtime runtime_build)

# Startup executable
add_executable(startup main.cpp)

target_link_libraries(startup PRIVATE leanclr)

set_target_properties(startup PROPERTIES
    CXX_STANDARD 11
    CXX_STANDARD_REQUIRED ON
)

# Optional: place binaries under build tree for convenience
set_target_properties(startup PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
)
