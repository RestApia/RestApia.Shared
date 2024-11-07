using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaWebView;
using RestApia.Experiments.Desktop.Views;

namespace RestApia.Experiments.Desktop;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            AppWindow = desktop.MainWindow;
            AvaloniaWebViewBuilder.Initialize(config =>
            {
                // some settings could be here
            });
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static Window AppWindow { get; private set; } = null!;
}
