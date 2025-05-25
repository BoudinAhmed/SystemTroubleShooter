using System;
using SystemTroubleShooter.ViewModel;


namespace SystemTroubleShooter.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private string? _version;

        public string? Version
        {
            get =>  _version; 
            set => SetProperty(ref _version, value);
        }

        public AboutViewModel() 
        { 
        LoadVersionInfo();
        }
        private void LoadVersionInfo()
        {
            // Hardcoded for now but will load it from assembly info
            if (Version is not null)
                Version = "Version 2.0.1";

            else Version = "Version x.x.x";
        }
    }
}
