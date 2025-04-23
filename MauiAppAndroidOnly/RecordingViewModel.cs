using MauiLibrary;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace MauiAppAndroidOnly;

using System.ComponentModel;

/// <summary>
/// State Machine for app
/// There are 5 Buttons
/// Only one enabled at a time, except Pause and Stop
/// </summary>
public class RecordingViewModel : INotifyPropertyChanged
{
    private MediaRecorderState _state = MediaRecorderState.Initial;
    public MediaRecorderState State
    {
        get => _state;
        set
        {
            _state = value;
            NotifyStateChange();
        }
    }

    public bool IsGetReadyButtonEnabled => State == MediaRecorderState.Stopped; // Enable button only if idle
    public bool IsStartRecordingButtonEnabled => State == MediaRecorderState.Prepared; // Enable button only if idle

    public bool IsPauseRecordingButtonEnabled => State == MediaRecorderState.Recording; // Enable button only if idle

    public bool IsContinueRecordingButtonEnabled => State == MediaRecorderState.Paused; // Enable button only if idle

    public bool IsStopRecordingButtonEnabled => State == MediaRecorderState.Recording; // Enable button only if idle

     public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NotifyStateChange()
    {
        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(IsGetReadyButtonEnabled));
        OnPropertyChanged(nameof(IsStartRecordingButtonEnabled));
        OnPropertyChanged(nameof(IsPauseRecordingButtonEnabled));
        OnPropertyChanged(nameof(IsContinueRecordingButtonEnabled));
        OnPropertyChanged(nameof(IsStopRecordingButtonEnabled));
    }
}
