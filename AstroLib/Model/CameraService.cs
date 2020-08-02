using AstroLib.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AstroLib.Model
{
	public class PictureFrame
	{
		public string ImageUrl { get; set; }
		public double FocusScore { get; set; }
	}

	public class CameraService
	{
		public ICamera Camera { get; private set; }
		private readonly IFocusAnalysis _focusAnalysis;
		private readonly IHostEnvironment _env;
		readonly SettingsService _settingService;

		public bool IsRunning { get { return _runningTask != null; } }
		CancellationTokenSource _runningToken;
		Task _runningTask;

		public delegate void FrameReadyHandler(CameraService sender, PictureFrame frame);
		public event FrameReadyHandler FrameReady;
		public event FrameReadyHandler StackedFrameReady;


		public string SessionName { get; private set; }
		
		private string SessionPath => Path.Combine("wwwroot", "Sessions", SessionName);
		private string GetServerSessionPath(string urlPath) => Path.Combine("wwwroot", urlPath);
		private string GetUrlSessionPath (string fileName) => Path.Combine("Sessions", SessionName, fileName);

		public CameraService(ICamera camera, IFocusAnalysis focusAnalysis, IHostEnvironment env, SettingsService settingService)
		{
			Camera = camera;
			_focusAnalysis = focusAnalysis;
			_env = env;
			_settingService = settingService;
		}

		private async Task<byte[]> CapturePicture(int quality)
		{
			Camera.ApplySettings(_settingService.ActiveConfig.Settings);
			return await Camera.TakePicture(quality);	
		}

		public void StartContinuousPictures(int quality, bool isPreview, bool createStacked)
		{
			if (IsRunning)
				return;

			if (!isPreview)
				StartSession();

			_runningToken = new CancellationTokenSource();
			_runningTask = Task.Run(async () => { await TakeContinousPicturesAsync(quality, isPreview, createStacked, _runningToken.Token); });
		}

		public void StopContinuousPictures()
		{
			if (_runningTask != null)
			{
				_runningToken.Cancel();

				_runningTask?.Wait();
				_runningTask = null;
			}
		}

		public void StartSession()
		{
			SessionName = string.Format("{0}_{1}", _settingService.ActiveConfig.Name, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
			if (!System.IO.Directory.Exists(SessionPath))
			{
				System.IO.Directory.CreateDirectory(SessionPath);
			}

		}

		private PictureFrame CreatePictureFrame(string fileName, byte[] imageData, bool stacked, bool appendCrumb)
        {
			// Append a different value to the img value to ensure it does not use a cached version on the client side (Preview Only)
			if (appendCrumb)
			{
				fileName = string.Format("{0}?r={1}", fileName, DateTime.Now.Ticks.ToString("X"));
			}


			PictureFrame frame = new PictureFrame { ImageUrl = fileName, FocusScore = _focusAnalysis.Score(imageData) };
			if (stacked)
				StackedFrameReady?.Invoke(this, frame);
			else
				FrameReady?.Invoke(this, frame);
			return frame;
		}

		public async Task<PictureFrame> TakePicture(int quality)
		{
			StartSession();
			string imageFileName = GetUrlSessionPath("1.jpg");
			byte[] result = await CapturePicture(quality);

			using (Stream fs = File.OpenWrite(GetServerSessionPath(imageFileName)))
			{
				await fs.WriteAsync(result, 0, result.Length);
			}
			return CreatePictureFrame(imageFileName, result, false, false);
		}

		public async Task TakeContinousPicturesAsync(int quality, bool isPreview, bool createStacked, CancellationToken token)
		{
			
			string stackedFile = createStacked ? GetUrlSessionPath("stacked.png") : string.Empty;
			StackedImage stackedImage = null;

			// Set a maximum rate to prevent performance issues
			TimeSpan frameTime = TimeSpan.FromSeconds(0.5);
			Stopwatch s = new Stopwatch();
			s.Start();

			int count = 1;
			while (!token.IsCancellationRequested)
			{
				s.Restart();

				// Save to different file based on timestamp
				string imageFileName;
				if (!isPreview)
					imageFileName = GetUrlSessionPath(string.Format("{0}.jpg", count++));
				else
					imageFileName = "preview.jpg";

				byte[] imageData = await CapturePicture(quality);

				// Save to file
				using (Stream fs = File.OpenWrite(GetServerSessionPath(imageFileName)))
				{
					await fs.WriteAsync(imageData, 0, imageData.Length);
				}

				CreatePictureFrame(imageFileName, imageData, false, isPreview);

				// Stacking?
				if (createStacked)
                {
					if (stackedImage == null)
                    {
						// First frame
						stackedImage = new StackedImage(imageData);
                    }
					else
                    {
						// Add
						stackedImage.AddImage(imageData);
                    }

					// Save
					byte[] stackedImageData = stackedImage.GetStackedImage();
					if (stackedImageData != null)
                    {
						using (Stream fs = File.OpenWrite(GetServerSessionPath(stackedFile)))
						{
							await fs.WriteAsync(stackedImageData, 0, stackedImageData.Length);
						}
					}

					CreatePictureFrame(stackedFile, stackedImageData, true, true);
                }

				TimeSpan time = frameTime - s.Elapsed;
				if (time > TimeSpan.Zero)
					await Task.Delay(time);
			}
		}
	}
}
