#include "ImageStacker.h"
#include <vector>
#include <algorithm>

namespace AstroCamLib
{

ImageStacker::ImageStacker(ImagePtr ptrPrimary)
{
    _ptrPrimary = ptrPrimary;
    cv::cvtColor(ptrPrimary->Get(), _ptrPrimaryBW->Get(), cv::COLOR_BGR2GRAY);

    cv::Ptr<cv::Feature2D> orb = cv::ORB::create(MaxFeatures);
    orb->detectAndCompute(_ptrPrimaryBW->Get(), cv::Mat(), _primaryKeypoints, _primaryDescriptors);
}

ImageStacker::~ImageStacker()
{

}

void ImageStacker::AlignImage(ImagePtr ptrNewFrame)
{
    // Convert to greyscale
    cv::Mat alignGray;
    cv::cvtColor(ptrNewFrame->Get(), alignGray, cv::COLOR_BGR2GRAY);

    std::vector<cv::KeyPoint> keypoints;
    cv::Mat descriptors;

    // Detect features
    cv::Ptr<cv::Feature2D> orb = cv::ORB::create(MaxFeatures);
    orb->detectAndCompute(alignGray, cv::Mat(), keypoints, descriptors);

    // Match the features
    std::vector<cv::DMatch> matches;
    cv::Ptr<cv::DescriptorMatcher> matcher = cv::DescriptorMatcher::create("BruteForce-Hamming");
    matcher->match(_primaryDescriptors, descriptors, matches, cv::Mat());

    // Sort by score
    std::sort(matches.begin(), matches.end());

    // Remove bad matches
    const int goodMatches = static_cast<int>(matches.size() * MatchPercent);
    matches.erase(matches.begin() + goodMatches, matches.end());

    // Draw top matches
//   Mat imMatches;
//    drawMatches(im1, keypoints1, im2, keypoints2, matches, imMatches);
//     imwrite("matches.jpg", imMatches);


    // Extract location of good matches
    std::vector<cv::Point2f> points1, points2;
    for(size_t i = 0; i < matches.size(); ++i)
    {
        points1.push_back(_primaryKeypoints[matches[i].queryIdx].pt);
        points2.push_back(keypoints[matches[i].trainIdx].pt);
    }

    // Find homography
    cv::Mat h = cv::findHomography(points1, points2, cv::RANSAC);
    cv::warpPerspective(ptrNewFrame->Get(), _ptrOutput->Get(), h, _ptrPrimary->Get().size());
}

}
