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

		public DummyCamera()
		{
			
		}

		public void ApplySettings (CameraSettings settings)
		{

		}

		public Task<byte[]> TakePicture(int quality)
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "moon.jpg");
			return Task.FromResult(File.ReadAllBytes(filePath).ToArray());
		}
	}
}
