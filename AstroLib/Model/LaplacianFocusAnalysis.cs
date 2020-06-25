using AstroLib.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AstroLib.Model
{
	public class LaplacianFocusAnalysis : IFocusAnalysis
	{
		public double Score(byte[] imgData)
		{
			OpenCvSharp.Mat img = OpenCvSharp.Mat.FromImageData(imgData, OpenCvSharp.ImreadModes.Grayscale);
			OpenCvSharp.Mat mean = OpenCvSharp.Mat.Zeros(3, 3);
			OpenCvSharp.Mat stddev = OpenCvSharp.Mat.Zeros(3, 3);
			img.Laplacian(OpenCvSharp.MatType.CV_64F).MeanStdDev(mean, stddev);
			double d = stddev.Get<double>(0);
			return d * d;
		}
	}
}
