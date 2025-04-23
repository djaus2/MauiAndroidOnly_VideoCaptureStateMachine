using MauiAppAndroidOnly;
using MauiLibrary;
using Microsoft.Extensions.DependencyInjection;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Register the service from the class library
#if ANDROID
        builder.Services.AddSingleton<IAndroidVideoRecorderService, AndroidVideoRecorderService>();
#endif
        return builder.Build();
    }
}
