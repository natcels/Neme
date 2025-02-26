using Neme.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Neme.ViewModels
{
    public class ScreenShareViewModel: BaseViewModel
    {
     
            private BitmapImage _sharedScreenFeed;

            public BitmapImage SharedScreenFeed
            {
                get => _sharedScreenFeed;
               // set => SetProperty(ref _sharedScreenFeed, value);
               set=> _sharedScreenFeed = value;
            }

            public ICommand StopScreenShareCommand { get; }

            public ScreenShareViewModel()
            {
                StopScreenShareCommand = new RelayCommand(StopScreenSharing);
            }

            private void StopScreenSharing()
            {
                LoggerUtility.LogInfo("Screen sharing stopped.");
                // Logic to stop screen sharing
            }
    }
}



