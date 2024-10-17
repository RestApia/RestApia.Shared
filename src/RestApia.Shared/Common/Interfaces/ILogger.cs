namespace RestApia.Shared.Common.Interfaces;

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Fail(string message);
    void Fail(Exception? ex, string message = "");
}
