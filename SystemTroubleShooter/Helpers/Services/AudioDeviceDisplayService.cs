using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SystemTroubleShooter.Interfaces;
using SystemTroubleShooter.View;
using SystemTroubleShooter.ViewModel;

namespace SystemTroubleShooter.Helpers.Services
{
    public class AudioDeviceDisplayService : IAudioDeviceDisplayService
    {

        public async Task<string?> SelectAudioDeviceAsync(List<string> outputDevices, List<string> inputDevices)
        {
            // This method now needs to be async if you await something,
            // but ShowDialog is synchronous. We'll wrap the logic.

            string? selectedDevice = null;
            var tcs = new TaskCompletionSource<string?>();

            // Ensure this runs on the UI thread if called from a background thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewModel = new SoundDevicesViewModel(outputDevices, inputDevices);
                var view = new SoundDevicesView
                {
                    DataContext = viewModel
                };

                viewModel.CloseWindowAction = (dialogResult) =>
                {
                    view.DialogResult = dialogResult;
                    view.Close();
                };

                bool? result = view.ShowDialog(); // This blocks until the window is closed

                if (result == true)
                {
                    selectedDevice = viewModel.ConfirmedDevice;
                }
                else
                {
                    selectedDevice = null; // Cancelled or closed
                }
                tcs.SetResult(selectedDevice);
            });

            return await tcs.Task;
        }

        public void DisplayAudioDevices(List<string> outputDevices, List<string> inputDevices)
        {
            var audioDeviceView = new SoundDevicesView
            {
                DataContext = new SoundDevicesViewModel(outputDevices, inputDevices)
            };
            audioDeviceView.Show();
           
        }
    }
}
