#include "ImageStacker.h"
#include <vector>
#include <algorithm>

namespace AstroCamLib
{

ImageStacker::ImageStacker()
{

}

ImageStacker::~ImageStacker()
{

}

void ImageStacker::AlignImage(cv::Mat &primaryImage, cv::Mat &alignImage, cv::Mat &alignedImage)
{
    // Convert to greyscale
    cv::Mat primaryGray, alignGray;
    cv::cvtColor(primaryImage, primaryGray, cv::COLOR_BGR2GRAY);
    cv::cvtColor(alignImage, alignGray, cv::COLOR_BGR2GRAY);

    std::vector<cv::KeyPoint> keypoints1, keypoints2;
    cv::Mat descriptors1, descriptors2;

    // Detect features
    cv::Ptr<cv::Feature2D> orb = cv::ORB::create(MaxFeatures);
    orb->detectAndCompute(primaryGray, cv::Mat(), keypoints1, descriptors1);
    orb->detectAndCompute(alignGray, cv::Mat(), keypoints2, descriptors2);

    // Match the features
    std::vector<cv::DMatch> matches;
    cv::Ptr<cv::DescriptorMatcher> matcher = cv::DescriptorMatcher::create("BruteForce-Hamming");
    matcher->match(descriptors1, descriptors2, matches, cv::Mat());

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
        points1.push_back(keypoints1[matches[i].queryIdx].pt);
        points2.push_back(keypoints2[matches[i].trainIdx].pt);
    }

    // Find homography
    cv::Mat h = cv::findHomography(points1, points2, cv::RANSAC);
    cv::warpPerspective(alignImage, alignedImage, h, primaryImage.size());
}

}
