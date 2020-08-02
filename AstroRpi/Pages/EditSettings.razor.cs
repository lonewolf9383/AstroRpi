using AstroLib.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AstroRpi.Pages
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class EditSettingsBase : ComponentBase
	{
        [Inject] 
        SettingsService SettingsService { get; set; }
        
        [Inject] 
        IJSRuntime jsRuntime { get; set; }

        [Inject]
        NavigationManager NavManager { get; set; }

        [Parameter]
        public string Name { get; set; }

        protected Dictionary<SupportedResolution, string> Resolutions { get; private set; } = new Dictionary<SupportedResolution, string>();
        protected Dictionary<ISO, string> Isos { get; private set; } = new Dictionary<ISO, string>();

        protected SettingConfig Config { get; set; }

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

            foreach (var c in (SupportedResolution[])Enum.GetValues(typeof(SupportedResolution)))
            {
                Resolutions.Add(c, Enum.GetName(typeof(SupportedResolution), c));
            }

            foreach (var c in (ISO[])Enum.GetValues(typeof(ISO)))
            {
                Isos.Add(c, Enum.GetName(typeof(ISO), c));
            }

            // Find the config if it exists
            SettingConfig exitingConfig = SettingsService.Settings.Where(x => string.Compare(x.Name, Name) == 0).FirstOrDefault();
            if (exitingConfig != null)
                Config = new SettingConfig(exitingConfig.Name, exitingConfig.Settings);
        }

     
        public async void OnValidSubmit()
        {
            // Apply changes
            SettingConfig existingConfig = SettingsService.Settings.Where(x => string.Compare(x.Name, Name) == 0).FirstOrDefault();
            if (existingConfig == null)
			{
                await jsRuntime.InvokeVoidAsync("alert", "Unexpected error. Config no longer exists");
			}
            else
			{
                if (string.Compare(existingConfig.Name, Config.Name, true) != 0)
                {
                    // Make sure another config with the new name does not already exist
                    if (SettingsService.Settings.Count(x => string.Compare(x.Name, Config.Name, true) == 0 && x != existingConfig) > 0)
                    {
                        await jsRuntime.InvokeVoidAsync("alert", "A config with that name already exists. Please select another name and try again");
                        return;
                    }
                   
                }
     
                existingConfig.Name = Config.Name;
                existingConfig.Settings.Copy(Config.Settings);
                SettingsService.SaveConfigs();
			}

            MoveBack();
        }

        public void MoveBack()
		{
            if (NavManager.Uri.Contains("takepicture", StringComparison.InvariantCultureIgnoreCase))
            {
                NavManager.NavigateTo("takepicture");
            }
            else
            {
                NavManager.NavigateTo("settings");
            }

        }

        public void Cancel_Clicked()
		{
            MoveBack();
		}

      /*  public async void Copy_Clicked()
		{
            try
            {
                string newName = await jsRuntime.InvokeAsync<string>("prompt", "Enter new name");
                if (string.IsNullOrWhiteSpace(newName))
                    return;

                SelectedConfig = SettingsService.CreateConfig(newName, SelectedConfig.Settings);
            }
            catch(Exception ex)
			{
                await jsRuntime.InvokeVoidAsync("alert", "Copy Failed - " + ex.Message);
            }
		}

        public async void Delete_Clicked()
		{
            bool confirm = await jsRuntime.InvokeAsync<bool>("confirm", string.Format("Delete '{0}'?", SelectedConfig.Name));
            if (confirm)
            {
                SettingsService.DeleteConfig(SelectedConfig);
                SelectedConfig = SettingsService.ActiveConfig;
            }
		}*/
    }
}
