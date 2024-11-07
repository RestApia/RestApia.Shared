using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Web.WebView2.Core;
using RestApia.Shared.Common.Models;
using WebViewCore.Events;

namespace RestApia.Experiments.Desktop.Modules.Auth;

public partial class AuthWindow : Window
{
    private readonly string _startUrl;
    private readonly string _stopUrl;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public AuthWindow()
    {
        if (!Design.IsDesignMode)
            throw new Exception("This constructor is only for design mode");

        _stopUrl = string.Empty;
        _startUrl = string.Empty;

        InitializeComponent();
    }

    public AuthWindow(string startUrl, string stopUrl)
    {
        _startUrl = startUrl;
        _stopUrl = stopUrl;

        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Design.IsDesignMode) return;
        WebViewControl.NavigationStarting += NavigateStarting;
        WebViewControl.Url = new Uri(_startUrl);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        WebViewControl.NavigationStarting -= NavigateStarting;
        base.OnUnloaded(e);
    }

    private void NavigateStarting(object? sender, WebViewUrlLoadingEventArg e)
    {
        var url = e.Url?.ToString();
        if (string.IsNullOrWhiteSpace(url))
            throw new ("Browser navigation url is empty");

        if (!url.StartsWith(_stopUrl, StringComparison.OrdinalIgnoreCase)) return;

        // stop URL detected
        e.Cancel = true;

        var headers = new Dictionary<string, string>();
        if (e.RawArgs is CoreWebView2NavigationStartingEventArgs args)
            headers = args.RequestHeaders.ToDictionary(header => header.Key, header => header.Value);

        Dispatcher.UIThread.Post(() => Close(new BrowserDialogResult
        {
            Url = url,
            Headers = headers,
        }));
    }
}
