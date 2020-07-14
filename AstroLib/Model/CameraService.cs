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

		public bool IsRunning { get { return _runningTask != null; } }
		CancellationTokenSource _runningToken;
		Task _runningTask;

		public delegate void FrameReadyHandler(CameraService sender, PictureFrame frame);
		public event FrameReadyHandler FrameReady;


		public string SessionName { get; private set; }

		private string SessionPath { get { return Path.Combine("wwwroot", "Sessions", SessionName);  } }
		//private string GetServerSessionPath (string fileName) { return Path.Combine("wwwroot", GetUrlSessionPath(fileName)); }
		private string GetUrlSessionPath (string fileName) {  return Path.Combine("Sessions", SessionName, fileName); }

		public CameraService(ICamera camera, IFocusAnalysis focusAnalysis, IHostEnvironment env)
		{
			Camera = camera;
			_focusAnalysis = focusAnalysis;
			_env = env;

			StartSession(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
		}

		private async Task<PictureFrame> CapturePicture(int quality, string fileName, bool appendVersion)
		{
			string savePath = Path.Combine("wwwroot", fileName);
			byte[] imageData = await Camera.TakePicture(quality);

			using (Stream fs = File.OpenWrite(savePath))
			{
				await fs.WriteAsync(imageData, 0, imageData.Length);
			}
			if (appendVersion)
			{
				fileName = string.Format("{0}?r={1}", fileName, DateTime.Now.Ticks.ToString("X"));
			}
			PictureFrame frame = new PictureFrame { ImageUrl = fileName, FocusScore = _focusAnalysis.Score(imageData) };
			FrameReady?.Invoke(this, frame);
			return frame;
		}

		public void StartContinuousPictures(int quality, bool isPreview)
		{
			if (IsRunning)
				return;

			_runningToken = new CancellationTokenSource();
			_runningTask = Task.Run(async () => { await TakeContinousPicturesAsync(quality, isPreview, _runningToken.Token); });
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

		public void StartSession(string name)
		{
			SessionName = name;
			if (!System.IO.Directory.Exists(SessionPath))
			{
				System.IO.Directory.CreateDirectory(SessionPath);
			}

		}

		public async Task<PictureFrame> TakePicture(int quality)
		{
			string imageFileName = GetUrlSessionPath(string.Format("{0}.jpg", DateTime.Now.ToString("HH_mm_ss_ff")));
			return await CapturePicture(quality, imageFileName, false);
		}

		public async Task TakeContinousPicturesAsync(int quality, bool isPreview, CancellationToken token)
		{
			string imageFileName = "preview.jpg";

			// Set a maximum rate to prevent performance issues
			TimeSpan frameTime = TimeSpan.FromSeconds(0.5);
			Stopwatch s = new Stopwatch();
			s.Start();

			while (!token.IsCancellationRequested)
			{
				s.Restart();

				// Save to different file based on timestamp
				if (!isPreview)
					imageFileName = GetUrlSessionPath(string.Format("{0}.jpg", DateTime.Now.ToString("HH_mm_ss_ff")));

				await CapturePicture(quality, imageFileName, isPreview);

				TimeSpan time = frameTime - s.Elapsed;
				if (time > TimeSpan.Zero)
					await Task.Delay(time);
			}
		}
	}
}
