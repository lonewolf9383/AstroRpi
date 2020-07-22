using AstroLib.Model;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AstroLib.Interfaces
{
	
	public interface ICamera
	{
		string Name { get; }

		void ApplySettings(CameraSettings settings);

		Task<byte[]> TakePicture(int quality = 100);
	}
}