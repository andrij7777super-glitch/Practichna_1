using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Розширений логер системи на основі StringBuilder.
/// </summary>
public class AdvancedLogger
{
    private readonly StringBuilder _logBuilder = new();
    private readonly List<string> _logLines = new();

    /// <summary>
    /// Додає запис до логу з рівнем і повідомленням.
    /// </summary>
    public void Log(string level, string message)
    {
        if (string.IsNullOrWhiteSpace(level))
        {
            throw new ArgumentException("Рівень логу не може бути порожнім.", nameof(level));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Повідомлення не може бути порожнім.", nameof(message));
        }

        var entry = $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] [{level.ToUpperInvariant()}] {message.Trim()}";
        _logLines.Add(entry);
        _logBuilder.AppendLine(entry);
    }

    /// <summary>
    /// Зберігає лог у файл.
    /// </summary>
    public void SaveToFile(string path)
    {
        File.WriteAllText(path, _logBuilder.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Повертає записи заданого рівня.
    /// </summary>
    public string GetLogsByLevel(string level)
    {
        var sb = new StringBuilder();
        var marker = $"[{level.ToUpperInvariant()}]";

        foreach (var line in _logLines.Where(l => l.Contains(marker, StringComparison.OrdinalIgnoreCase)))
        {
            sb.AppendLine(line);
        }

        return sb.Length == 0 ? $"Записів рівня '{level}' не знайдено." : sb.ToString();
    }

    /// <summary>
    /// Очищує лог.
    /// </summary>
    public void Clear()
    {
        _logBuilder.Clear();
        _logLines.Clear();
    }

    /// <summary>
    /// Повертає останні N записів логу.
    /// </summary>
    public string GetLast(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість записів має бути більше нуля.");
        }

        var sb = new StringBuilder();
        foreach (var line in _logLines.TakeLast(count))
        {
            sb.AppendLine(line);
        }

        return sb.Length == 0 ? "Лог порожній." : sb.ToString();
    }

    /// <summary>
    /// Повертає повний лог.
    /// </summary>
    public string GetFullLog() =>
        _logBuilder.Length == 0 ? "Лог системи порожній." : _logBuilder.ToString();
}
