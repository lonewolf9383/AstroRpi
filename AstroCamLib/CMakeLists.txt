cmake_minimum_required(VERSION 3.14)

project(AstroCamLib LANGUAGES CXX)

set(CMAKE_INCLUDE_CURRENT_DIR ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package( OpenCV REQUIRED )
include_directories( ${OpenCV_INCLUDE_DIRS} )

#################
## GRPC
#################

include(FetchContent)

FetchContent_Declare(
  gRPC
  GIT_REPOSITORY https://github.com/grpc/grpc
  GIT_TAG        v1.51.1
  )
set(FETCHCONTENT_QUIET OFF)
FetchContent_MakeAvailable(gRPC)

    set(_PROTOBUF_PROTOC $<TARGET_FILE:protobuf::protoc>)

  

  find_program(_GRPC_CPP_PLUGIN_EXECUTABLE grpc_cpp_plugin)


# Proto file
get_filename_component(hw_proto "messages.proto" ABSOLUTE)
get_filename_component(hw_proto_path "${hw_proto}" PATH)

# Generated sources
set(hw_proto_srcs "${CMAKE_CURRENT_BINARY_DIR}/messages.pb.cc")
set(hw_proto_hdrs "${CMAKE_CURRENT_BINARY_DIR}/messages.pb.h")
set(hw_grpc_srcs "${CMAKE_CURRENT_BINARY_DIR}/messages.grpc.pb.cc")
set(hw_grpc_hdrs "${CMAKE_CURRENT_BINARY_DIR}/messages.grpc.pb.h")
add_custom_command(
      OUTPUT "${hw_proto_srcs}" "${hw_proto_hdrs}" "${hw_grpc_srcs}" "${hw_grpc_hdrs}"
      COMMAND ${_PROTOBUF_PROTOC}
      ARGS --grpc_out "${CMAKE_CURRENT_BINARY_DIR}"
        --cpp_out "${CMAKE_CURRENT_BINARY_DIR}"
        -I "${hw_proto_path}"
        --plugin=protoc-gen-grpc="${_GRPC_CPP_PLUGIN_EXECUTABLE}"
        "${hw_proto}"
      DEPENDS "${hw_proto}")

# Include generated *.pb.h files
include_directories("${CMAKE_CURRENT_BINARY_DIR}")

# hw_grpc_proto
add_library(hw_grpc_proto
  ${hw_grpc_srcs}
  ${hw_grpc_hdrs}
  ${hw_proto_srcs}
  ${hw_proto_hdrs})
target_link_libraries(hw_grpc_proto
  ${GRPCPP_LIBRARIES}
  ${_PROTOBUF_LIBPROTOBUF})


#set(OpenCV_DIR "C:/Devel/OpenCV/build")

add_library(AstroCamLib STATIC
  AstroCamLib_global.h
  astrocamlib.cpp
  astrocamlib.h
  ImageStacker.h
  ImageStacker.cpp
  Image.h
)

target_compile_definitions(AstroCamLib PRIVATE ASTROCAMLIB_LIBRARY)
target_link_libraries( AstroCamLib PUBLIC ${OpenCV_LIBS} hw_grpc_proto )

