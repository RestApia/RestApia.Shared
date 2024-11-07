using System.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using CustomMessageBox.Avalonia;
namespace RestApia.Experiments.Desktop.Modules.Common;

public static class MessageGrid
{
    public static Task<MessageBoxResult> ShowAsync(IEnumerable data)
    {
        var dataGrid = new DataGrid
        {
            AutoGenerateColumns = false,
            CanUserSortColumns = false,
            IsReadOnly = true,
            Width = 700,
            MaxHeight = 800,

            Columns =
            {
                new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), Width = DataGridLength.Auto },
                new DataGridTextColumn { Header = "Value", Binding = new Binding("Value") },
            },
        };

        dataGrid.ItemsSource = data;

        return MessageBox.Show(
            dataGrid,
            "Authorization");
    }
}
