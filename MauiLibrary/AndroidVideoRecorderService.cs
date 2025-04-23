// This file is part of the MAUI Library project.
using Android.Hardware.Camera2;
using Android.Media;
using AndroidX.Annotations;

namespace MauiLibrary;

public enum MediaRecorderState
{
    Initial,
    Prepared,
    Recording,
    Paused,
    Stopped,
    Released,
    Error,
    Processing
}

public interface IAndroidVideoRecorderService
{
    public MediaRecorderState _state { get; }
    string GetReady4Recording(string filename);
    string StartRecording();
    string PauseRecording();
    string ContinueRecording();
    string StopRecording();

    // Resource management
    Task CleanupResourcesAsync();
}

public class AndroidVideoRecorderService : IAndroidVideoRecorderService
{

    public MediaRecorderState _state { get; set; } = MediaRecorderState.Initial;
    private bool _disposed = false;

    public AndroidVideoRecorderService()
    {
        // Initialize in constructor
        _state = MediaRecorderState.Stopped;
        _disposed = false;
    }
    public string GetReady4Recording(string filename="")
    {
        if(!string.IsNullOrEmpty(filename))
        {   filename = filename.Trim();
            _state = MediaRecorderState.Prepared;
            return $"1. Recording setup to  {filename}.mp4";
        }
        else
        {
            _state = MediaRecorderState.Error;
            return "1. Recording setup fail. Blank filename";
        }
    }

    public string StartRecording() 
    {
        _state = MediaRecorderState.Recording; 
        return "2. Starting/ed Recording";  
    }

    public string PauseRecording()
    {
        _state = MediaRecorderState.Paused;
        return "3. Pausing/ed Recording";
    }

    public string ContinueRecording()
    {
        _state = MediaRecorderState.Recording;
        return "4. Continuing/ed Recording";
    }

    public string StopRecording()
    {
        _state = MediaRecorderState.Initial;
        return "5. Done";
    }


    public async Task CleanupResourcesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Cleaning up resources...");

            switch(_state)
            {
                case MediaRecorderState.Initial:
                    // Handle initial state cleanup if necessary
                    break;
                case MediaRecorderState.Prepared:
                    // Handle prepared state cleanup if necessary
                    _state = MediaRecorderState.Stopped;
                    break;
                case MediaRecorderState.Recording:
                    // Handle paused state cleanup if necessary
                    _ = Task.Run(StopRecording);
                    _state = MediaRecorderState.Stopped;
                    break;
                case MediaRecorderState.Paused:
                    // Handle paused state cleanup if necessary
                    _ = Task.Run(StopRecording);
                    _state= MediaRecorderState.Stopped;
                    break;
                case MediaRecorderState.Stopped:
                    // Handle stopped state cleanup if necessary
                    break;
                case MediaRecorderState.Released:
                    // Handle released state cleanup if necessary
                    break;
                default:
                    // Handle other states if necessary
                    break;
            }
            await Task.Delay(500);
            System.Diagnostics.Debug.WriteLine("All resources cleaned up.");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cleaning up resources: {ex.Message}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                CleanupResourcesAsync().Wait();
            }

            _disposed = true;
        }
    }

    ~AndroidVideoRecorderService()
    {
        Dispose(false);
    }
}
