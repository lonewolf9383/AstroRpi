using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace AstroLib.Model
{
	[PropertyChanged.AddINotifyPropertyChangedInterface]
	public class SettingConfig
	{
		
		[System.ComponentModel.DataAnnotations.Required]
		public string Name { get; set; }

		public CameraSettings Settings { get; set; } = new CameraSettings();

		private SettingConfig() { }

		public SettingConfig(string name, CameraSettings settings)
		{
			Name = name;
			Settings = settings;
		}

		public SettingConfig Clone(string newName)
		{
			return new SettingConfig(newName, Settings.Clone());
		}
	}

	public class SettingsXml
	{
		public SettingConfig[] Configs { get; set; } = Array.Empty<SettingConfig>();

		public SettingsXml()
		{

		}
	}

	[PropertyChanged.AddINotifyPropertyChangedInterface]
	public class SettingsService
	{
		public SettingConfig ActiveConfig { get; set; }

		public List<SettingConfig> Settings { get; private set; } = new List<SettingConfig>();


		public SettingsService()
		{
			LoadConfigs();
		}

		public void AddConfig(SettingConfig config)
		{
			// Make sure another config with the new name does not already exist
			if (Settings.Count(x => string.Compare(x.Name, config.Name, true) == 0) > 0)
			{
				throw new Exception("A config with this name already exists");
			}

			Settings.Add(config);
			SaveConfigs();
		}
		public void DeleteConfig(SettingConfig config)
		{
			if (Settings.Contains(config))
			{
				if (Settings.Count <= 1)
					throw new Exception("Can not delete last config");
			}
			Settings.Remove(config);
			if (ActiveConfig == config)
			{
				ActiveConfig = Settings.First();
			}

			SaveConfigs();
		}

		private static string ConfigPath { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AstroCamPi"); } }
		public void SaveConfigs()
		{
			if (!Directory.Exists(ConfigPath))
				Directory.CreateDirectory(ConfigPath);

			SettingsXml xml = new SettingsXml { Configs = Settings.ToArray() };
			string fileName = Path.Combine(ConfigPath, "Configs.xml"); 
			File.WriteAllText(fileName, JsonSerializer.Serialize<SettingsXml>(xml));
		}

		public void LoadConfigs()
		{
			Settings.Clear();

			string fileName = Path.Combine(ConfigPath, "Configs.xml");
			if (File.Exists(fileName))
			{
				try
				{
					SettingsXml xml = JsonSerializer.Deserialize<SettingsXml>(File.ReadAllText(fileName));
					if (xml != null)
					{
						Settings.AddRange(xml.Configs);
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("Error Loading Config {0} - {1}", fileName, ex.Message));
				}
			}

			if (Settings.Count == 0)
			{
				// Create some defaults
				Settings.Add(new SettingConfig("Default", new CameraSettings()));
			}

			ActiveConfig = Settings.First();
		}
	}
}
