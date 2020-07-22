using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AstroLib.Model
{
	public enum SupportedResolution
	{
		High = 0,
		Medium,
		Low,
	}

	public enum ISO
	{
		Auto = 0,
		ISO_100 = 100,
		ISO_200 = 200,
		ISO_400 = 400,
		ISO_800 = 800,
	}

	[PropertyChanged.AddINotifyPropertyChangedInterface]
	public class CameraSettings
	{
		[Required]
		[Range(0, 100)]
		public int Brightness { get; set; }

		[Required]
		[Range(-100, 100)]
		public int Contrast { get; set; }

		[Required]
		[Range(-10, 10)]
		public int Exposure { get; set; }

		[Required]
		public ISO ISO { get; set; } = ISO.Auto;

		[Required]
		public SupportedResolution Resolution { get; set; } = SupportedResolution.Low;

		[Required]
		[Range(-100, 100)]
		public int Saturation { get; set; }

		[Required]
		[Range(0, 100)]
		public int Sharpness { get; set; }

		[Required]
		[Range(0, 200000)]
		public int ShutterSpeedMs {get;set;}

		public CameraSettings Clone()
		{
			return new CameraSettings
			{
				Brightness = Brightness,
				Contrast = Contrast,
				Exposure = Exposure,
				ISO = ISO,
				Resolution = Resolution,
				Saturation = Saturation,
				Sharpness = Sharpness,
				ShutterSpeedMs = ShutterSpeedMs
			};
		}

		public void Copy(CameraSettings from)
		{
			Brightness = from.Brightness;
			Contrast = from.Contrast;
			Exposure = from.Exposure;
			ISO = from.ISO;
			Resolution = from.Resolution;
			Saturation = from.Saturation;
			Sharpness = from.Sharpness;
			ShutterSpeedMs = from.ShutterSpeedMs;
		}
	}
}
