#pragma once

#include <opencv2/opencv.hpp>
#include "Image.h"

namespace AstroCamLib
{

class ImageStacker
{
private:

    int MaxFeatures = 500;
    float MatchPercent = 0.15f;
    ImagePtr _ptrPrimaryBW;
    ImagePtr _ptrPrimary;
    std::vector<cv::KeyPoint> _primaryKeypoints;
    cv::Mat _primaryDescriptors;
    ImagePtr _ptrOutput;

public:

    ImageStacker(ImagePtr primary);
    ~ImageStacker();

    ImagePtr GetResult() { return _ptrOutput; }

    void AlignImage(ImagePtr ptrNewFrame);
};

}
