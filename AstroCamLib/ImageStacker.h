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
    ImagePtr _primaryBW;
    ImagePtr _output;

public:

    ImageStacker(ImagePtr primary);
    ~ImageStacker();

    ImagePtr GetResult() { return _output; }

    void AlignImage(ImagePtr ptrNewFrame);
};

}
