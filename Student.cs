using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

/// <summary>
/// Представляє студента навчальної групи ЗПФК.
/// </summary>
public class Student
{
    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private string _personalEmail = string.Empty;

    [JsonInclude]
    private GradeJournal _gradeJournal;

    /// <summary>
    /// Створює нового студента з ініціалізованим журналом оцінок.
    /// </summary>
    [JsonConstructor]
    public Student()
    {
        _gradeJournal = new GradeJournal(SetAverageGradeInternal);
        EnrollmentDate = DateTime.Today;
        Status = StudentStatus.Active;
    }

    /// <summary>
    /// Повне ім'я студента (ПІБ) з валідацією.
    /// </summary>
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

    /// <summary>
    /// Дата народження (встановлюється лише під час ініціалізації).
    /// </summary>
    public DateTime DateOfBirth { get; init; }

    /// <summary>
    /// Поточний вік студента (обчислювана властивість).
    /// </summary>
    public int Age => CalculateAge();

    /// <summary>
    /// Номер залікової книжки (обов'язкове поле, рівно 8 цифр).
    /// </summary>
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

    /// <summary>
    /// Середній бал (0–100), автоматично пов'язаний із журналом оцінок.
    /// </summary>
    public double AverageGrade
    {
        get => _averageGrade;
        private set => SetAverageGradeInternal(value);
    }

    /// <summary>
    /// Поточний статус студента.
    /// </summary>
    public StudentStatus Status { get; set; }

    /// <summary>
    /// Дата зарахування до коледжу.
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Особиста електронна пошта з перевіркою формату.
    /// </summary>
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

    /// <summary>
    /// Додаткові нотатки про студента.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Журнал оцінок студента.
    /// </summary>
    [JsonIgnore]
    public GradeJournal Journal => _gradeJournal;

    /// <summary>
    /// Виводить детальну інформацію про студента в консоль.
    /// </summary>
    public void ShowDetailedInfo()
    {
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"ПІБ:                 {FullName}");
        Console.WriteLine($"Дата народження:     {DateOfBirth:dd.MM.yyyy}");
        Console.WriteLine($"Вік:                 {Age} років");
        Console.WriteLine($"Залікова книжка:     {RecordBookNumber}");
        Console.WriteLine($"Середній бал:        {AverageGrade:F2}");
        Console.WriteLine($"Статус:              {GetStatusDisplayName(Status)}");
        Console.WriteLine($"Дата зарахування:    {EnrollmentDate:dd.MM.yyyy}");
        Console.WriteLine($"Років до випуску:    {GetYearsToGraduation():F2}");
        Console.WriteLine($"Email:               {PersonalEmail}");
        Console.WriteLine($"Нотатки:             {(string.IsNullOrWhiteSpace(Notes) ? "—" : Notes)}");

        var grades = Journal.GetAllGrades();
        if (grades.Count > 0)
        {
            Console.WriteLine("Оцінки за предметами:");
            foreach (var grade in grades)
            {
                Console.WriteLine($"  • {grade.Key}: {grade.Value:F2}");
            }
        }
        else
        {
            Console.WriteLine("Оцінки за предметами:  відсутні");
        }

        Console.WriteLine(new string('=', 50));
    }

    /// <summary>
    /// Оновлює середній бал з валідацією діапазону 0–100.
    /// </summary>
    public void UpdateAverageGrade(double newGrade)
    {
        AverageGrade = newGrade;
    }

    /// <summary>
    /// Перевіряє, чи є студент відмінником (середній бал ≥ 90).
    /// </summary>
    public bool IsExcellent() => AverageGrade >= 90;

    /// <summary>
    /// Перевіряє, чи має студент академічні заборгованості (середній бал &lt; 60).
    /// </summary>
    public bool IsFailing() => AverageGrade < 60;

    /// <summary>
    /// Обчислює точний вік студента на поточну дату.
    /// </summary>
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

    /// <summary>
    /// Повертає кількість років до випуску (термін навчання — 4 роки).
    /// </summary>
    public double GetYearsToGraduation()
    {
        var graduationDate = EnrollmentDate.AddYears(4);
        var remainingDays = (graduationDate - DateTime.Today).TotalDays;

        if (remainingDays <= 0)
        {
            return 0;
        }

        return Math.Round(remainingDays / 365.25, 2);
    }

    /// <summary>
    /// Відновлює зв'язок журналу оцінок із середнім балом після завантаження з файлу.
    /// </summary>
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
