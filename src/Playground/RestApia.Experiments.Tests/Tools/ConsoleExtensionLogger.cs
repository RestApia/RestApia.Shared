using RestApia.Shared.Common.Interfaces;
namespace RestApia.Experiments.Tests.Tools;

public class ConsoleExtensionLogger : ILogger
{
    public void Debug(string message) => Console.WriteLine($"[DBG] {message}");
    public void Info(string message) => Console.WriteLine($"[INF] {message}");
    public void Warn(string message) => Console.WriteLine($"[WRN] {message}");
    public void Fail(string message) => Console.WriteLine($"[ERR] {message}");
    public void Fail(Exception? ex, string message = "") => Console.WriteLine($"[ERR] {message} {ex?.Message}");

    public static ConsoleExtensionLogger Instance { get; } = new ();
}
