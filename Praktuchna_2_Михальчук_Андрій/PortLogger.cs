using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Логер операцій портів з використанням StringBuilder.
/// </summary>
public class PortLogger
{
    private readonly StringBuilder _logBuilder = new();

    /// <summary>
    /// Додає запис про операцію з портом до логу.
    /// </summary>
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

    /// <summary>
    /// Повертає повний текст логу.
    /// </summary>
    public string GetFullLog()
    {
        if (_logBuilder.Length == 0)
        {
            return "Лог операцій порту порожній.";
        }

        return _logBuilder.ToString();
    }

    /// <summary>
    /// Зберігає лог у текстовий файл.
    /// </summary>
    public void SaveLogToFile(string path = "port_log.txt")
    {
        File.WriteAllText(path, GetFullLog(), Encoding.UTF8);
    }

    /// <summary>
    /// Відновлює лог із збереженого тексту.
    /// </summary>
    public void RestoreFromText(string? logText)
    {
        _logBuilder.Clear();
        if (!string.IsNullOrWhiteSpace(logText))
        {
            _logBuilder.Append(logText);
        }
    }

    /// <summary>
    /// Очищує накопичений лог.
    /// </summary>
    public void Clear()
    {
        _logBuilder.Clear();
    }
}
