using AstroLib.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AstroLib.Model
{
	public class DummyCamera : ICamera
	{
		public string Name => "Dummy Camera";

		public int Brightness { get; set; } = 50;
		public int Contrast { get; set; } = 50;
		public int ExposureCompensation { get; set; } = 50;
		public int ISO { get; set; } = 100;
		public SupportedResolution Resolution { get; set; } = SupportedResolution.High;
		public int Saturation { get; set; } = 50;
		public int Sharpness { get; set; } = 50;
		public int ShutterSpeed { get; set; } = 1000;

		public DummyCamera()
		{
			
		}

		public Task<byte[]> TakePicture(int quality)
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "moon.jpg");
			return Task.FromResult(File.ReadAllBytes(filePath).ToArray());
		}
	}
}
