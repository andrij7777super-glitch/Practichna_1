using System.Text;
using System.Text.RegularExpressions;

namespace StudentGroupApp;

public abstract class Person
{
    private string _fullName = string.Empty;
    private string _personalEmail = string.Empty;

    protected Person()
    {
    }

    protected Person(string fullName, DateTime dateOfBirth, string personalEmail, string notes)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        PersonalEmail = personalEmail;
        Notes = notes;
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

    public DateTime DateOfBirth { get; set; }

    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }

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

    public virtual string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ПІБ: {FullName}");
        sb.AppendLine($"Дата народження: {DateOfBirth:dd.MM.yyyy}");
        sb.AppendLine($"Вік: {Age}");
        sb.AppendLine($"Email: {PersonalEmail}");
        if (!string.IsNullOrWhiteSpace(Notes))
        {
            sb.AppendLine($"Примітки: {Notes}");
        }

        return sb.ToString().TrimEnd();
    }
}
