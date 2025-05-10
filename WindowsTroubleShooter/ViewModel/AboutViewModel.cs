using System;


namespace WindowsTroubleShooter.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private string _version;

        public string Version
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
            Version = "Version 2.0.1";
        }
    }
}
