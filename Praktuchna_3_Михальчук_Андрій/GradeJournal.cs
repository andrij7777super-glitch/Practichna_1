using System.Text.Json.Serialization;

namespace StudentGroupApp;
public class GradeJournal
{
    private readonly Dictionary<string, double> _subjectGrades = new(StringComparer.OrdinalIgnoreCase);
    private Action<double>? _onAverageChanged;

    [JsonConstructor]
    public GradeJournal()
    {
    }

    internal GradeJournal(Action<double> onAverageChanged)
    {
        _onAverageChanged = onAverageChanged;
    }

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

    internal void ConnectAverageChangedCallback(Action<double> onAverageChanged)
    {
        _onAverageChanged = onAverageChanged;
        NotifyAverageChanged();
    }

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

    public double RecalculateAverageGrade()
    {
        if (_subjectGrades.Count == 0)
        {
            return 0;
        }

        return Math.Round(_subjectGrades.Values.Average(), 2);
    }

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
