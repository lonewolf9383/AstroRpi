cmake_minimum_required(VERSION 3.14)

project(AstroCamLib LANGUAGES CXX)

set(CMAKE_INCLUDE_CURRENT_DIR ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package( OpenCV REQUIRED )
include_directories( ${OpenCV_INCLUDE_DIRS} )

set(OpenCV_DIR "C:/Devel/OpenCV/build")

add_library(AstroCamLib STATIC
  AstroCamLib_global.h
  astrocamlib.cpp
  astrocamlib.h
  ImageStacker.h
  ImageStacker.cpp
)

target_compile_definitions(AstroCamLib PRIVATE ASTROCAMLIB_LIBRARY)
target_link_libraries( AstroCamLib ${OpenCV_LIBS} )
