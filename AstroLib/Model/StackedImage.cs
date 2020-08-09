using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AstroLib.Model
{
    public class StackedImage
    {
        OpenCvSharp.Mat _stackedImage;
        OpenCvSharp.Mat _originalImageGrey;

        public StackedImage(byte[] originalFrame)
        {
            _stackedImage = OpenCvSharp.Mat.FromImageData(originalFrame, OpenCvSharp.ImreadModes.Color);
            _originalImageGrey = OpenCvSharp.Mat.FromImageData(originalFrame, OpenCvSharp.ImreadModes.Grayscale);
        }

        public bool AddImage(byte[] imageData)
        {
            try
            {
                const int MaxIterations = 1000;
                const double TerminateEps = 1e-8;

                OpenCvSharp.Mat greyscale = OpenCvSharp.Mat.FromImageData(imageData, OpenCvSharp.ImreadModes.Grayscale);

                OpenCvSharp.Mat warpMatrix = OpenCvSharp.Mat.Eye(2, 3, OpenCvSharp.MatType.CV_32F);
                OpenCvSharp.TermCriteria criteria = new OpenCvSharp.TermCriteria(OpenCvSharp.CriteriaType.Count | OpenCvSharp.CriteriaType.Eps, MaxIterations, TerminateEps);

                OpenCvSharp.Cv2.FindTransformECC(_originalImageGrey, greyscale, warpMatrix, OpenCvSharp.MotionTypes.Euclidean, criteria);

                // Load the original colour image
                OpenCvSharp.Mat frame = OpenCvSharp.Mat.FromImageData(imageData, OpenCvSharp.ImreadModes.Color);

                OpenCvSharp.Mat warpedFrame = frame.WarpAffine(warpMatrix, _originalImageGrey.Size(), OpenCvSharp.InterpolationFlags.Linear | OpenCvSharp.InterpolationFlags.WarpInverseMap);
                _stackedImage += warpedFrame;
                return true;
            }
            catch(Exception ex)
			{
                Trace.WriteLine("Error - " + ex.Message);
                return false;
			}
        }

        public byte[] GetStackedImage()
        {
            if (_stackedImage != null)
            {
               return _stackedImage.ToBytes(".png");
            }

            return null;
        }
    }
}
