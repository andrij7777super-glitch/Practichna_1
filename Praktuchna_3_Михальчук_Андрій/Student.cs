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

    [JsonInclude]
    private GradeJournal _gradeJournal;

    [field: JsonInclude]
    public byte[] LabGrades { get; } = new byte[10];

    [JsonConstructor]
    public Student()
    {
        _gradeJournal = new GradeJournal(SetAverageGradeInternal);
        EnrollmentDate = DateTime.Today;
        Status = StudentStatus.Active;
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

            var trimmed = TextProcessor.Normalize(value);
            if (trimmed.Length < 5)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 5 символів.", nameof(FullName));
            }

            var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 3 слова (Прізвище Ім'я По батькові).", nameof(FullName));
            }

            foreach (var word in words)
            {
                if (word.Length < 2)
                {
                    throw new ArgumentException($"Слово '{word}' занадто коротке. Кожне слово ПІБ має містити щонайменше 2 літери.", nameof(FullName));
                }

                if (!Regex.IsMatch(word, @"^[А-ЯІЇЄҐа-яіїєґ'\-]+$"))
                {
                    throw new ArgumentException($"Слово '{word}' містить недопустимі символи. Дозволені лише літери українського алфавіту.", nameof(FullName));
                }
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
            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(trimmed, emailPattern))
            {
                throw new ArgumentException("Невірний формат електронної пошти.", nameof(PersonalEmail));
            }

            _personalEmail = trimmed;
        }
    }

    public string Notes { get; set; } = string.Empty;

    [JsonIgnore]
    public GradeJournal Journal => _gradeJournal;
    public string GetFormattedInfo(bool detailed = false)
    {
        var sb = new StringBuilder();
        sb.Append("ПІБ: ").AppendLine(FullName);
        sb.Append("Залікова: ").AppendLine(RecordBookNumber);
        sb.Append("Середній бал: ").AppendLine(AverageGrade.ToString("F2"));
        sb.Append("Статус: ").AppendLine(GetStatusDisplayName(Status));

        if (detailed)
        {
            sb.Append("Email: ").AppendLine(PersonalEmail);
            sb.Append("Вік: ").AppendLine(Age.ToString());
            sb.Append("Дата народження: ").AppendLine(DateOfBirth.ToString("dd.MM.yyyy"));
            sb.Append("Нотатки: ").AppendLine(string.IsNullOrWhiteSpace(Notes) ? "—" : Notes);
            sb.Append("Середній бал (лаби): ").AppendLine(GetAverageLabGrade().ToString("F2"));
        }

        return sb.ToString();
    }
    public bool ContainsKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return false;
        }

        var fields = new[]
        {
            FullName,
            RecordBookNumber,
            PersonalEmail,
            Notes,
            GetStatusDisplayName(Status)
        };

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
            EnrollmentDate = EnrollmentDate,
            Status = Status,
            Notes = Notes
        };

        Array.Copy(LabGrades, clone.LabGrades, LabGrades.Length);

        foreach (var grade in Journal.GetAllGrades())
        {
            clone.Journal.AddOrUpdateGrade(grade.Key, grade.Value);
        }

        return clone;
    }

    public void ShowDetailedInfo() => Console.WriteLine(GetFormattedInfo(detailed: true));

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

    public double GetYearsToGraduation()
    {
        var graduationDate = EnrollmentDate.AddYears(4);
        var remainingDays = (graduationDate - DateTime.Today).TotalDays;
        return remainingDays <= 0 ? 0 : Math.Round(remainingDays / 365.25, 2);
    }

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
