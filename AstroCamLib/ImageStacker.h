#pragma once

#include <opencv2/opencv.hpp>

namespace AstroCamLib
{

class ImageStacker
{
private:

    int MaxFeatures = 500;
    float MatchPercent = 0.15f;

public:

    ImageStacker();
    ~ImageStacker();

    void AlignImage(cv::Mat &primaryImg, cv::Mat &alignImage, cv::Mat &alignedImage);
};

}
