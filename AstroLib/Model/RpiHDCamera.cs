using AstroLib.Interfaces;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AstroLib.Model
{
	public class RpiHDCamera : IDisposable, ICamera
	{
		public string Name => "Rpi HD Camera";

		private SupportedResolution _resolution = SupportedResolution.Low;
		public SupportedResolution Resolution
		{
			get
			{
				return _resolution;
			}
			set
			{
				_resolution = value;
				switch (_resolution)
				{
					case SupportedResolution.High:
						MMALSharp.MMALCameraConfig.Resolution = new MMALSharp.Common.Utility.Resolution(4056, 3040);
						MMALSharp.MMALCameraConfig.SensorMode = MMALSensorMode.Mode3;
						break;
					case SupportedResolution.Medium:
						MMALSharp.MMALCameraConfig.Resolution = new MMALSharp.Common.Utility.Resolution(2028, 1520);
						MMALSharp.MMALCameraConfig.SensorMode = MMALSensorMode.Mode2;
						break;
					case SupportedResolution.Low:
						MMALSharp.MMALCameraConfig.Resolution = new MMALSharp.Common.Utility.Resolution(1012, 760);
						MMALSharp.MMALCameraConfig.SensorMode = MMALSensorMode.Mode4;
						break;
				}
			}
		}

		public int Sharpness
		{
			get => (int)MMALCameraConfig.Sharpness;
			set => MMALCameraConfig.Sharpness = value;
		}

		public int Contrast
		{
			get => (int)MMALCameraConfig.Contrast;
			set => MMALCameraConfig.Contrast = value;
		}

		public int Brightness
		{
			get => (int)MMALCameraConfig.Brightness;
			set => MMALCameraConfig.Brightness = value;
		}

		public int Saturation
		{
			get => (int)MMALCameraConfig.Saturation;
			set => MMALCameraConfig.Saturation = value;
		}

		public int ISO
		{
			get => MMALCameraConfig.ISO;
			set => MMALCameraConfig.ISO = value;
		}

		public int ExposureCompensation
		{
			get => MMALCameraConfig.ISO;
			set => MMALCameraConfig.ISO = value;
		}

		public int ShutterSpeed
		{
			get => MMALCameraConfig.ShutterSpeed;
			set => MMALCameraConfig.ShutterSpeed = value;
		}

		public RpiHDCamera()
		{
			Resolution = SupportedResolution.Low;
			MMALCamera.Instance.ConfigureCameraSettings();
		}

		public async Task<byte[]> TakePicture(int quality)
		{
			using (var jpegCaptureHandler = new InMemoryCaptureHandler())
			using (var imageEncoder = new MMALImageEncoder())
			using (var nullSink = new MMALNullSinkComponent())
			{
				MMALCamera.Instance.ConfigureCameraSettings();

				MMALPortConfig portConfigJPEG = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, quality: quality);

				imageEncoder.ConfigureOutputPort(portConfigJPEG, jpegCaptureHandler);

				MMALCamera.Instance.Camera.StillPort.ConnectTo(imageEncoder);
				MMALCamera.Instance.Camera.PreviewPort.ConnectTo(nullSink);
				await MMALCamera.Instance.ProcessAsync(MMALCamera.Instance.Camera.StillPort);
				return jpegCaptureHandler.WorkingData.ToArray();
			}
		}

		private bool disposedValue;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				MMALCamera.Instance.Cleanup();
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		~RpiHDCamera()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
