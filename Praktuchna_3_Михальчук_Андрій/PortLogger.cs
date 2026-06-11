using System.Text;

namespace StudentGroupApp;
public class PortLogger
{
    private readonly StringBuilder _logBuilder = new();
    public void LogOperation(string operation, int portNumber, string details)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            throw new ArgumentException("Назва операції не може бути порожньою.", nameof(operation));
        }

        _logBuilder.Append('[')
            .Append(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"))
            .Append("] Порт #")
            .Append(portNumber)
            .Append(" | ")
            .Append(operation.Trim())
            .Append(" | ")
            .Append(details.Trim())
            .AppendLine();
    }
    public string GetFullLog()
    {
        if (_logBuilder.Length == 0)
        {
            return "Лог операцій порту порожній.";
        }

        return _logBuilder.ToString();
    }
    public void SaveLogToFile(string path = "port_log.txt")
    {
        File.WriteAllText(path, GetFullLog(), Encoding.UTF8);
    }
    public void RestoreFromText(string? logText)
    {
        _logBuilder.Clear();
        if (!string.IsNullOrWhiteSpace(logText))
        {
            _logBuilder.Append(logText);
        }
    }
    public void Clear()
    {
        _logBuilder.Clear();
    }
}
