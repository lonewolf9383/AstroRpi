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

		public void ApplySettings(CameraSettings settings)
		{
			lock (this)
			{
				// Apply settings to the camera
				switch (settings.Resolution)
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

				MMALCameraConfig.Sharpness = settings.Sharpness;
				MMALCameraConfig.Contrast = settings.Contrast;
				MMALCameraConfig.Brightness = settings.Brightness;
				MMALCameraConfig.Saturation = settings.Saturation;
				MMALCameraConfig.ISO = (int)settings.ISO;
				MMALCameraConfig.ExposureCompensation = settings.Exposure;
				MMALCameraConfig.ShutterSpeed = Math.Max(1000, settings.ShutterSpeedMs * 1000);
			}
		}

		public RpiHDCamera()
		{
		}

		public async Task<byte[]> TakePicture(int quality)
		{
			using (var jpegCaptureHandler = new InMemoryCaptureHandler())
			using (var imageEncoder = new MMALImageEncoder())
			using (var nullSink = new MMALNullSinkComponent())
			{
				lock (this)
				{
					MMALCamera.Instance.ConfigureCameraSettings();
				}
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
