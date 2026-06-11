using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

/// <summary>
/// Звичайний студент ЗПФК, успадковує UniversityMember.
/// </summary>
public class Student : UniversityMember
{
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private int _courseProgress;

    [JsonInclude]
    private GradeJournal _gradeJournal;

    protected Student()
    {
        _gradeJournal = new GradeJournal(SetAverageGradeInternal);
        Status = StudentStatus.Active;
        GradePoints = new List<GradePoint>();
    }

    public Student(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, notes)
    {
        _gradeJournal = new GradeJournal(SetAverageGradeInternal);
        RecordBookNumber = recordBookNumber;
        Status = StudentStatus.Active;
        GradePoints = new List<GradePoint>();
        Enroll();
    }

    public string RecordBookNumber
    {
        get => _recordBookNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^\d{8}$"))
            {
                throw new ArgumentException("Номер залікової книжки має містити рівно 8 цифр.", nameof(RecordBookNumber));
            }

            _recordBookNumber = value;
        }
    }

    public double AverageGrade
    {
        get => _averageGrade;
        private set => SetAverageGradeInternal(value);
    }

    public int CourseProgress
    {
        get => _courseProgress;
        set
        {
            if (value is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(CourseProgress), "Прогрес курсу має бути від 0 до 100.");
            }

            _courseProgress = value;
        }
    }

    public List<GradePoint> GradePoints { get; set; } = new();
    public StudentStatus Status { get; set; }

    [JsonIgnore]
    public GradeJournal Journal => _gradeJournal;

    public override void Enroll()
    {
        base.Enroll();
        Status = StudentStatus.Active;
    }

    public override decimal CalculateScholarship()
    {
        return AverageGrade >= 60 ? 1500m : 0m;
    }

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine($"Тип: {GetType().Name}");
        sb.AppendLine($"Залікова: {RecordBookNumber}");
        sb.AppendLine($"Середній бал: {AverageGrade:F2}");
        sb.AppendLine($"Прогрес курсу: {CourseProgress}%");
        sb.AppendLine($"Статус: {GetStatusDisplayName(Status)}");
        sb.Append($"Стипендія: {CalculateScholarship():F2} грн");
        return sb.ToString();
    }

    public void UpdateAverageGrade(double newGrade) => AverageGrade = newGrade;
    public bool IsExcellent() => AverageGrade >= 90;
    public bool IsFailing() => AverageGrade < 60;

    internal void RestoreJournalBinding()
    {
        _gradeJournal.ConnectAverageChangedCallback(SetAverageGradeInternal);
    }

    private void SetAverageGradeInternal(double value)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Середній бал має бути в діапазоні від 0 до 100.");
        }

        _averageGrade = Math.Round(value, 2);
    }

    private static string GetStatusDisplayName(StudentStatus status) => status switch
    {
        StudentStatus.Active => "Активний",
        StudentStatus.AcademicLeave => "Академічна відпустка",
        StudentStatus.Expelled => "Відрахований",
        StudentStatus.Graduated => "Випускник",
        _ => status.ToString()
    };
}
