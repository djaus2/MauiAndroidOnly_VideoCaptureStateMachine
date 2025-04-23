using MauiLibrary;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace MauiAppAndroidOnly;


using System.ComponentModel;

public partial class MainPage : ContentPage
{
    private readonly IAndroidVideoRecorderService _androidVideoRecorderService;

    public MainPage(IAndroidVideoRecorderService androidVideoRecorderService)
    {
        InitializeComponent();
        PerformPerms();
        BindingContext = new RecordingViewModel();
        Appearing += MainPage_Appearing;
        Disappearing += MainPage_Disappearing;
        _androidVideoRecorderService = androidVideoRecorderService;
        var viewModel = (RecordingViewModel)BindingContext;
        viewModel.State = MediaRecorderState.Stopped; // Button gets disabled
    }


    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public new event PropertyChangedEventHandler PropertyChanged;

    //////////////////////////////////////////////////////////////////////////////////////
    /// In XAML:
    /// <VerticalStackLayout x:Name="MyLayout"
    //////////////////////////////////////////////////////////////////////////////////////
    /// ActivityIndicator is created and shown on Button press to start Stitching
    /// Is disposed when the stitching process is completed
    //////////////////////////////////////////////////////////////////////////////////////

    // The activity indicator
    private ActivityIndicator activityIndicator;

    /// <summary>
    /// Called by Buttun Click event handler on MainThread
    /// </summary>
    private void StartActivity()
    {
        // Start the activity indicator
        activityIndicator = new ActivityIndicator
        {
            IsRunning = true,
            Color = Colors.Blue // Optional: Set a color for visibility
        };
        // Assuming you have a named layout in XAML
        MyLayout.Children.Insert(2,activityIndicator);
    }

    /// <summary>
    /// Called by BackgroundWorker at worker.RunWorkerCompleted.
    /// Calls StopActivity on MainThread
    /// </summary>
    private void StoppActivity()
    {
        MainThread.InvokeOnMainThreadAsync(() => StopActivity());
    }

    /// <summary>
    /// Runs on MainThread to stop the activity indicator and remove it from the layout
    ///  </summary>
    private void StopActivity()
    {
        // Stop the activity indicator
        if (activityIndicator != null)
        {
            activityIndicator.IsRunning = false;
            MyLayout.Children.Remove(activityIndicator);
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////


    string filename { get; set; } = string.Empty;
    void OnFilenameCompleted(object sender, EventArgs e)
    {
        filename = ((Entry)sender).Text;
    }

    private async void OnButton_GetReady4RecordingAsync(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(filename))
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var externalStorageDirectory = Android.OS.Environment.ExternalStorageDirectory;

            if (externalStorageDirectory != null && !string.IsNullOrEmpty(Android.OS.Environment.DirectoryDcim)) // Ensure the directory is not null
            {
                string videoFileName = Path.Combine(
                    externalStorageDirectory.AbsolutePath,
                    Android.OS.Environment.DirectoryDcim,
                    $"{filename}_{timestamp}.mp4"
                );

                GreetingLabel.Text = _androidVideoRecorderService.GetReady4Recording(videoFileName);
                await Task.Delay(500);
                var viewModel = (RecordingViewModel)BindingContext;
                viewModel.State = MediaRecorderState.Prepared;
            }
            else
            {
                // Handle the case where ExternalStorageDirectory or DirectoryDcim is null
                GreetingLabel.Text = "Error: External storage directory or DCIM directory is not available.";
            }
        }
    }

    private async void OnButton_StartRecording_ClickedAsync(object sender, EventArgs e)
    {
        var viewModel = (RecordingViewModel)BindingContext;
        viewModel.State = MediaRecorderState.Processing;
        GreetingLabel.Text = _androidVideoRecorderService.StartRecording();
        await Task.Delay(500);
        viewModel.State = MediaRecorderState.Recording; // Button gets disabled
        StartActivity();

    }

    private async void OnButton_PauseRecording_ClickedAsync(object sender, EventArgs e)
    {
        var viewModel = (RecordingViewModel)BindingContext;
        viewModel.State = MediaRecorderState.Processing;
        GreetingLabel.Text = _androidVideoRecorderService.PauseRecording();;
        await Task.Delay(500);
        viewModel.State = MediaRecorderState.Paused; // Button gets disabled
        StopActivity();
    }

    private async void OnButton_ContinueRecording_ClickedAsync(object sender, EventArgs e)
    {
        var viewModel = (RecordingViewModel)BindingContext;
        viewModel.State = MediaRecorderState.Processing;
        GreetingLabel.Text = _androidVideoRecorderService.ContinueRecording();
        await Task.Delay(500);
        viewModel.State = MediaRecorderState.Recording; // Button gets disabled
        StartActivity();
    }

    private async void OnButton_StopRecordingClickedAsync(object sender, EventArgs e)
    {
        var viewModel = (RecordingViewModel)BindingContext;
        viewModel.State = MediaRecorderState.Processing;
        GreetingLabel.Text = _androidVideoRecorderService.StopRecording();
        await Task.Delay(500);
        viewModel.State = MediaRecorderState.Stopped; // Button gets disabled
        StopActivity();
    }


    #region Page Lifecycle

    private async void MainPage_Appearing(object sender, EventArgs e)
    {
        // Setup camera when page appears
        // Task.Run(async () => await _cameraService.SetupCameraAsync());
        //_videoRecorderService..Wait();
    }

    private async void MainPage_Disappearing(object sender, EventArgs e)
    {
        // Cleanup resources when page disappears
        await _androidVideoRecorderService.CleanupResourcesAsync();
        //_videoRecorderService.CleanupResourcesAsync.Wait();
    }

    #endregion

    #region Permissions

    private void PerformPerms()
    {
        // Request necessary permissions
        var permissions = new string[]
        {
                Android.Manifest.Permission.Camera,
                Android.Manifest.Permission.RecordAudio,
                Android.Manifest.Permission.ReadExternalStorage,
                Android.Manifest.Permission.WriteExternalStorage
        };

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            status = await Permissions.RequestAsync<Permissions.StorageRead>();
            status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        });
    }

    #endregion


}

