using System.Text.Json.Serialization;

namespace StudentGroupApp;

/// <summary>
/// Журнал оцінок студента за предметами.
/// Відповідає за зберігання оцінок та перерахунок середнього балу.
/// </summary>
public class GradeJournal
{
    private readonly Dictionary<string, double> _subjectGrades = new(StringComparer.OrdinalIgnoreCase);
    private Action<double>? _onAverageChanged;

    /// <summary>
    /// Конструктор для десеріалізації JSON.
    /// </summary>
    [JsonConstructor]
    public GradeJournal()
    {
    }

    /// <summary>
    /// Створює журнал із зворотним викликом для оновлення середнього балу студента.
    /// </summary>
    internal GradeJournal(Action<double> onAverageChanged)
    {
        _onAverageChanged = onAverageChanged;
    }

    /// <summary>
    /// Оцінки за предметами (для серіалізації JSON).
    /// </summary>
    [JsonInclude]
    public Dictionary<string, double> SubjectGrades
    {
        get => _subjectGrades;
        set
        {
            _subjectGrades.Clear();
            if (value is null)
            {
                return;
            }

            foreach (var pair in value)
            {
                ValidateGrade(pair.Value);
                _subjectGrades[pair.Key] = Math.Round(pair.Value, 2);
            }
        }
    }

    /// <summary>
    /// Підключає зворотний виклик після завантаження з файлу.
    /// </summary>
    internal void ConnectAverageChangedCallback(Action<double> onAverageChanged)
    {
        _onAverageChanged = onAverageChanged;
        NotifyAverageChanged();
    }

    /// <summary>
    /// Додає або оновлює оцінку за предметом.
    /// </summary>
    public void AddOrUpdateGrade(string subject, double grade)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Назва предмету не може бути порожньою.", nameof(subject));
        }

        ValidateGrade(grade);
        _subjectGrades[subject.Trim()] = Math.Round(grade, 2);
        NotifyAverageChanged();
    }

    /// <summary>
    /// Повертає середнє значення всіх оцінок у журналі.
    /// </summary>
    public double RecalculateAverageGrade()
    {
        if (_subjectGrades.Count == 0)
        {
            return 0;
        }

        return Math.Round(_subjectGrades.Values.Average(), 2);
    }

    /// <summary>
    /// Повертає копію словника оцінок (лише для читання).
    /// </summary>
    public IReadOnlyDictionary<string, double> GetAllGrades()
    {
        return new Dictionary<string, double>(_subjectGrades);
    }

    private void NotifyAverageChanged()
    {
        _onAverageChanged?.Invoke(RecalculateAverageGrade());
    }

    private static void ValidateGrade(double grade)
    {
        if (grade is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Оцінка має бути в діапазоні від 0 до 100.");
        }
    }
}
