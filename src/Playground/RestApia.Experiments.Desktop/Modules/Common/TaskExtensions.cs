using Avalonia.Threading;
using CustomMessageBox.Avalonia;
namespace RestApia.Experiments.Desktop.Modules.Common;

public static class TaskExtensions
{
    public static Task AlertOnError(this Task task) => task
        .ContinueWith(t =>
            {
                var exception = t.Exception!.InnerException ?? t.Exception;
                Dispatcher.UIThread.Post(() => MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            },
            TaskContinuationOptions.OnlyOnFaulted);
}
