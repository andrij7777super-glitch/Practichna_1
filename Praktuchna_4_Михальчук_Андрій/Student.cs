using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

public class Student : ICloneable
{
    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private string _personalEmail = string.Empty;
    private int _courseProgress;

    [JsonInclude]
    private GradeJournal _gradeJournal;

    [JsonConstructor]
    public Student()
    {
        _gradeJournal = new GradeJournal(SetAverageGradeInternal);
        EnrollmentDate = DateTime.Today;
        Status = StudentStatus.Active;
        GradePoints = new List<GradePoint>();
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Повне ім'я не може бути порожнім.", nameof(FullName));
            }

            var trimmed = Regex.Replace(value.Trim(), @"\s+", " ");
            if (trimmed.Length < 5)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 5 символів.", nameof(FullName));
            }

            var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 3 слова.", nameof(FullName));
            }

            _fullName = trimmed;
        }
    }

    public DateTime DateOfBirth { get; init; }
    public int Age => CalculateAge();

    public required string RecordBookNumber
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
    public DateTime EnrollmentDate { get; set; }

    public string PersonalEmail
    {
        get => _personalEmail;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Електронна пошта не може бути порожньою.", nameof(PersonalEmail));
            }

            var trimmed = value.Trim();
            if (!Regex.IsMatch(trimmed, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Невірний формат електронної пошти.", nameof(PersonalEmail));
            }

            _personalEmail = trimmed;
        }
    }

    public string Notes { get; set; } = string.Empty;

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
            EnrollmentDate = DateTime.Today,
            Status = StudentStatus.Active,
            CourseProgress = (a.CourseProgress + b.CourseProgress) / 2,
            Notes = $"{a.Notes} | {b.Notes}"
        };

        merged.UpdateAverageGrade(Math.Round((a.AverageGrade + b.AverageGrade) / 2, 2));

        foreach (var gp in a.GradePoints.Concat(b.GradePoints))
        {
            merged.GradePoints.Add(new GradePoint(Math.Min(gp.Value, 10)));
        }

        return merged;
    }

    public string GetFormattedInfo(bool detailed = false)
    {
        var sb = new StringBuilder();
        sb.Append("ПІБ: ").AppendLine(FullName);
        sb.Append("Залікова: ").AppendLine(RecordBookNumber);
        sb.Append("Середній бал: ").AppendLine(AverageGrade.ToString("F2"));
        sb.Append("Прогрес курсу: ").AppendLine(CourseProgress.ToString());
        sb.Append("Статус: ").AppendLine(GetStatusDisplayName(Status));

        if (detailed)
        {
            sb.Append("Email: ").AppendLine(PersonalEmail);
            sb.Append("Вік: ").AppendLine(Age.ToString());
            if (GradePoints.Count > 0)
            {
                sb.Append("Бали GradePoint: ").AppendLine(string.Join(", ", GradePoints));
            }
        }

        return sb.ToString();
    }

    public object Clone()
    {
        var clone = new Student
        {
            FullName = FullName,
            DateOfBirth = DateOfBirth,
            RecordBookNumber = RecordBookNumber,
            PersonalEmail = PersonalEmail,
            EnrollmentDate = EnrollmentDate,
            Status = Status,
            Notes = Notes,
            CourseProgress = CourseProgress
        };

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

    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

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
