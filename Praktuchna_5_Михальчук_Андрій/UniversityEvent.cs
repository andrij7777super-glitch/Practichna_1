namespace StudentGroupApp;

/// <summary>
/// Абстрактний студентський захід університету (варіант 10).
/// </summary>
public abstract class UniversityEvent
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    /// <summary>
    /// Поліморфне проведення заходу — кожен тип реалізує власну поведінку.
    /// </summary>
    public abstract void ConductEvent();
}
