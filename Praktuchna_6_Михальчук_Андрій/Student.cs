using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

public class Student : UniversityMember, ICloneable
{
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private int _courseProgress;

    [JsonInclude]
    private GradeJournal _gradeJournal;

    [field: JsonInclude]
    public byte[] LabGrades { get; } = new byte[10];

    public Student()
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
    public List<Shape> Projects { get; set; } = new();
    public StudentStatus Status { get; set; }

    [JsonIgnore]
    public GradeJournal Journal => _gradeJournal;

    public static bool operator >(Student a, Student b) => Compare(a, b) > 0;
    public static bool operator <(Student a, Student b) => Compare(a, b) < 0;
    public static bool operator >=(Student a, Student b) => Compare(a, b) >= 0;
    public static bool operator <=(Student a, Student b) => Compare(a, b) <= 0;

    public static bool operator ==(Student? a, Student? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return Compare(a, b) == 0;
    }

    public static bool operator !=(Student? a, Student? b) => !(a == b);

    public static Student operator +(Student a, Student b)
    {
        var wordsA = a.FullName.Split(' ');
        var wordsB = b.FullName.Split(' ');
        var combinedName = $"{wordsA[0]} {wordsB[0]} Об'єднаний";

        var merged = new Student
        {
            FullName = combinedName,
            DateOfBirth = a.DateOfBirth < b.DateOfBirth ? a.DateOfBirth : b.DateOfBirth,
            RecordBookNumber = GenerateMergedRecordBook(a, b),
            PersonalEmail = "merged@student.zpfk.edu.ua",
            Status = StudentStatus.Active,
            CourseProgress = (a.CourseProgress + b.CourseProgress) / 2,
            Notes = $"{a.Notes} | {b.Notes}"
        };

        merged.EnrollmentDate = DateTime.Today;
        merged.UpdateAverageGrade(Math.Round((a.AverageGrade + b.AverageGrade) / 2, 2));

        foreach (var gp in a.GradePoints.Concat(b.GradePoints))
        {
            merged.GradePoints.Add(new GradePoint(Math.Min(gp.Value, 10)));
        }

        return merged;
    }

    public override void Enroll()
    {
        base.Enroll();
        Status = StudentStatus.Active;
    }

    public override decimal CalculateScholarship() => AverageGrade >= 60 ? 1500m : 0m;

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

    public string GetFormattedInfo(bool detailed = false)
    {
        var sb = new StringBuilder();
        sb.AppendLine(GetInfo());

        if (detailed)
        {
            sb.AppendLine($"Середній бал (лаби): {GetAverageLabGrade():F2}");
            sb.Append("Відсортовані оцінки лаб: [");
            sb.Append(string.Join(", ", GetSortedLabGrades().Select(g => g == 0 ? "—" : g.ToString())));
            sb.AppendLine("]");
            if (GradePoints.Count > 0)
            {
                sb.AppendLine($"Бали GradePoint: {string.Join(", ", GradePoints)}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public bool ContainsKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return false;
        }

        var fields = new[] { FullName, RecordBookNumber, PersonalEmail, Notes, GetStatusDisplayName(Status) };
        return fields.Any(f => f.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void AddLabGrade(int labNumber, byte grade)
    {
        if (labNumber is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(labNumber), "Номер лабораторної має бути від 1 до 10.");
        }

        if (grade > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Оцінка має бути в діапазоні від 0 до 100.");
        }

        LabGrades[labNumber - 1] = grade;
    }

    public double GetAverageLabGrade()
    {
        var completedLabs = LabGrades.Where(g => g > 0).ToArray();
        return completedLabs.Length == 0 ? 0 : Math.Round(completedLabs.Average(g => (double)g), 2);
    }

    public byte[] GetSortedLabGrades()
    {
        var sorted = (byte[])LabGrades.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    public object Clone()
    {
        var clone = new Student
        {
            FullName = FullName,
            DateOfBirth = DateOfBirth,
            RecordBookNumber = RecordBookNumber,
            PersonalEmail = PersonalEmail,
            Status = Status,
            Notes = Notes,
            CourseProgress = CourseProgress
        };

        clone.EnrollmentDate = EnrollmentDate;
        Array.Copy(LabGrades, clone.LabGrades, LabGrades.Length);

        foreach (var grade in Journal.GetAllGrades())
        {
            clone.Journal.AddOrUpdateGrade(grade.Key, grade.Value);
        }

        foreach (var gp in GradePoints)
        {
            clone.GradePoints.Add(new GradePoint(gp.Value));
        }

        return clone;
    }

    public void UpdateAverageGrade(double newGrade) => AverageGrade = newGrade;
    public bool IsExcellent() => AverageGrade >= 90;
    public bool IsFailing() => AverageGrade < 60;

    internal void RestoreJournalBinding()
    {
        _gradeJournal.ConnectAverageChangedCallback(SetAverageGradeInternal);
    }

    public override bool Equals(object? obj) => obj is Student s && this == s;

    public override int GetHashCode() => HashCode.Combine(RecordBookNumber, AverageGrade, CourseProgress);

    private static int Compare(Student a, Student b)
    {
        var gradeCompare = a.AverageGrade.CompareTo(b.AverageGrade);
        return gradeCompare != 0 ? gradeCompare : a.CourseProgress.CompareTo(b.CourseProgress);
    }

    private static string GenerateMergedRecordBook(Student a, Student b)
    {
        var hash = Math.Abs(HashCode.Combine(a.RecordBookNumber, b.RecordBookNumber)) % 100000000;
        return hash.ToString("D8");
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
