using AstroLib.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstroRpi.Pages
{
    public class TakePictureBase : ComponentBase
    {

        [Inject]
        protected CameraService CamService { get; set; }

        [Inject]
        protected SettingsService SettingsService { get; set; }

        [Inject]
        protected NavigationManager NavManager { get; set; }

        protected enum CaptureState
        {
            Preview,
            Single,
            Continous,
            ContinousStacked
        }

        protected string ImgData { get; set; }
        protected double FocusScore { get; set; }
        protected string StackedImgData { get; set; }
        protected double StackedFocusScore { get; set; }
        protected bool ShowStacked { get; set; }

        protected string DisplayedImgData => ShowStacked ? StackedImgData : ImgData;
        protected double DisplayedFocusScore => ShowStacked ? StackedFocusScore : FocusScore;

        protected string CameraButtonText { get { return CamService.IsRunning ? "Stop" : "Start"; } }
        protected bool DisableEdits{ get { return CamService.IsRunning && SelectedState != CaptureState.Preview; } }
        protected int FrameCount { get; set; }
        protected Dictionary<CaptureState, string> CameraStateList = new Dictionary<CaptureState, string>();
        protected CaptureState SelectedState { get; set; }

        [PropertyChanged.DependsOn("SelectedConfig")]
        protected string SelectedConfigName
		{
            get
			{
                return SettingsService.ActiveConfig?.Name;
			}
            set
			{
                SettingsService.ActiveConfig = SettingsService.Settings.Where(x => string.Compare(x.Name, value) == 0).FirstOrDefault();
			}
		}


     
        protected override void OnInitialized()
        {
            base.OnInitialized();

            CamService.FrameReady += OnFrameReady;
            CamService.StackedFrameReady += OnStackedFrameReady;
            CameraStateList.Add(CaptureState.Preview, "Preview");
            CameraStateList.Add(CaptureState.Continous, "Continuous");
            CameraStateList.Add(CaptureState.ContinousStacked, "Continuous + Stacked");
            CameraStateList.Add(CaptureState.Single, "Single Frame");
        }

        private void OnStackedFrameReady(CameraService sender, PictureFrame frame)
        {
            InvokeAsync(
                () =>
                {
                    StackedImgData = frame.ImageUrl;
                    StackedFocusScore = frame.FocusScore;

                    StateHasChanged();
                }
            );
        }

        protected void OnFrameReady(CameraService sender, PictureFrame frame)
        {
            InvokeAsync(
                () =>
                {
                    ImgData = frame.ImageUrl;
                    FocusScore = frame.FocusScore;
                    ++FrameCount;

                    StateHasChanged();
                }
            );
        }

        protected async void OnTakePicture()
        {
            if (CamService.IsRunning)
            {
                CamService.StopContinuousPictures();
            }
            else
            {
                FrameCount = 0;
                switch (SelectedState)
                {
                    case CaptureState.Continous:
                    case CaptureState.Preview:
                    case CaptureState.ContinousStacked:
                        CamService.StartContinuousPictures(100, SelectedState == CaptureState.Preview, SelectedState == CaptureState.ContinousStacked);
                        break;

                    case CaptureState.Single:
                        await CamService.TakePicture(100);
                        break;

                }

            }

            StateHasChanged();
        }

        protected void EditSelectedConfig_Clicked()
		{
            NavManager.NavigateTo(string.Format("takepicture/Settings/{0}", SelectedConfigName));
		}
    }
}
