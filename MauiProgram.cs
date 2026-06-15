using BehaviorTracker.Services;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace BehaviorTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<BehaviorPage>();
            builder.Services.AddTransient<LogsPage>();
            return builder.Build();
        }
    }
}
