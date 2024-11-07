using Avalonia.Controls;
using Avalonia.Interactivity;
using RestApia.Experiments.Desktop.Experiments.Auth;
using RestApia.Experiments.Desktop.Experiments.Secrets;

namespace RestApia.Experiments.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Auth_OAuth2_AuthCode_OnClick(object? sender, RoutedEventArgs e) => OAuth2Experiments.RunAuthCodeAsync().AlertOnError();
    private void Auth_OAuth2_Implicit_OnClick(object? sender, RoutedEventArgs e) => OAuth2Experiments.RunImplicitAsync().AlertOnError();
    private void Auth_Basic_OnClick(object? sender, RoutedEventArgs e) => BasicAuthExperiments.RunBasicAsync().AlertOnError();
    private void Secrets_AzureKeyVault_OnClick(object? sender, RoutedEventArgs e) => AzureKeyVaultExperiments.RunAsync().AlertOnError();
}
