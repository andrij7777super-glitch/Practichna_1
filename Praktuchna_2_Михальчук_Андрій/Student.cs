using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

/// <summary>
/// Представляє студента навчальної групи ЗПФК.
/// </summary>
public class Student : ICloneable
{
    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private string _personalEmail = string.Empty;

    [JsonInclude]
    private GradeJournal _gradeJournal;

    /// <summary>
    /// Оцінки за 10 лабораторних робіт (одновимірний масив).
    /// </summary>
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

            var trimmed = value.Trim();
            if (trimmed.Length < 5)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 5 символів.", nameof(FullName));
            }

            var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 3 слова (Прізвище Ім'я По батькові).", nameof(FullName));
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

    /// <summary>
    /// Додає оцінку за лабораторну роботу в одновимірний масив.
    /// </summary>
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

    /// <summary>
    /// Обчислює середній бал за виконані лабораторні роботи.
    /// </summary>
    public double GetAverageLabGrade()
    {
        var completedLabs = LabGrades.Where(g => g > 0).ToArray();
        if (completedLabs.Length == 0)
        {
            return 0;
        }

        return Math.Round(completedLabs.Average(g => (double)g), 2);
    }

    /// <summary>
    /// Повертає копію масиву лабораторних оцінок, відсортовану за зростанням.
    /// </summary>
    public byte[] GetSortedLabGrades()
    {
        var sorted = (byte[])LabGrades.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    /// <summary>
    /// Створює глибоку копію студента.
    /// </summary>
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

    public void ShowDetailedInfo()
    {
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"ПІБ:                 {FullName}");
        Console.WriteLine($"Дата народження:     {DateOfBirth:dd.MM.yyyy}");
        Console.WriteLine($"Вік:                 {Age} років");
        Console.WriteLine($"Залікова книжка:     {RecordBookNumber}");
        Console.WriteLine($"Середній бал:        {AverageGrade:F2}");
        Console.WriteLine($"Середній бал (лаби): {GetAverageLabGrade():F2}");
        Console.WriteLine($"Статус:              {GetStatusDisplayName(Status)}");
        Console.WriteLine($"Дата зарахування:    {EnrollmentDate:dd.MM.yyyy}");
        Console.WriteLine($"Років до випуску:    {GetYearsToGraduation():F2}");
        Console.WriteLine($"Email:               {PersonalEmail}");
        Console.WriteLine($"Нотатки:             {(string.IsNullOrWhiteSpace(Notes) ? "—" : Notes)}");

        Console.WriteLine("Лабораторні оцінки (відсортовані):");
        var sortedLabs = GetSortedLabGrades();
        for (var i = 0; i < sortedLabs.Length; i++)
        {
            var display = sortedLabs[i] == 0 ? "—" : sortedLabs[i].ToString();
            Console.WriteLine($"  Лаб. #{i + 1,2}: {display}");
        }

        var grades = Journal.GetAllGrades();
        if (grades.Count > 0)
        {
            Console.WriteLine("Оцінки за предметами:");
            foreach (var grade in grades)
            {
                Console.WriteLine($"  • {grade.Key}: {grade.Value:F2}");
            }
        }

        Console.WriteLine(new string('=', 50));
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
