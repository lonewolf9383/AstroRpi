using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AstroLib.Interfaces
{
	public enum SupportedResolution
	{
		High = 0,
		Medium,
		Low,
	}

	public interface ICamera
	{
		string Name { get; }
		int Brightness { get; set; }
		int Contrast { get; set; }
		int ExposureCompensation { get; set; }
		int ISO { get; set; }
		SupportedResolution Resolution { get; set; }
		int Saturation { get; set; }
		int Sharpness { get; set; }
		int ShutterSpeed { get; set; }

		Task<byte[]> TakePicture(int quality = 100);
	}
}