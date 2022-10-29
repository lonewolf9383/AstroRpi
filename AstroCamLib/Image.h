#pragma once

#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <memory>

namespace AstroCamLib
{
    class Image;
    typedef std::shared_ptr<Image> ImagePtr;

    class Image
    {
        private:
            cv::Mat _image;

        public:

        static ImagePtr FromFile(std::string path)
        {
            return std::make_shared<Image> (cv::imread(path, cv::IMREAD_COLOR));
        }


        Image(cv::Mat mat)
        : _image(mat)
        {

        }

        ~Image()
        {
        }

        cv::Mat &Get() {return _image;}
    };

    typedef std::shared_ptr<Image> ImagePtr;
}