using CD_Vault.Pages;
using CD_Vault.Services;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace CD_Vault
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

            builder.Services.AddSingleton<CdDatabase>();
            builder.Services.AddHttpClient<AlbumSearchService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<CollectionPage>();
            builder.Services.AddTransient<DiscoverPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
