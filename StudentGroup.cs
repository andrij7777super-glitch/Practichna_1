using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentGroupApp;

/// <summary>
/// Навчальна група студентів ЗПФК.
/// </summary>
public class StudentGroup
{
    private readonly List<Student> _students = new();

    /// <summary>
    /// Назва навчальної групи.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// Спеціальність групи.
    /// </summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>
    /// Курс навчання.
    /// </summary>
    public int Course { get; set; }

    /// <summary>
    /// Список студентів групи (для серіалізації JSON).
    /// </summary>
    [JsonInclude]
    public List<Student> Students
    {
        get => _students;
        set
        {
            _students.Clear();
            if (value is not null)
            {
                _students.AddRange(value);
            }
        }
    }

    /// <summary>
    /// Кількість студентів у групі.
    /// </summary>
    [JsonIgnore]
    public int GroupSize => _students.Count;

    /// <summary>
    /// Середній бал групи.
    /// </summary>
    [JsonIgnore]
    public double AverageGroupGrade
    {
        get
        {
            if (_students.Count == 0)
            {
                return 0;
            }

            return Math.Round(_students.Average(s => s.AverageGrade), 2);
        }
    }

    /// <summary>
    /// Додає студента до групи.
    /// </summary>
    public void AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (_students.Any(s => s.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException($"Студент із номером залікової {student.RecordBookNumber} вже існує в групі.");
        }

        _students.Add(student);
    }

    /// <summary>
    /// Видаляє студента за номером залікової книжки.
    /// </summary>
    public bool RemoveStudent(string recordBookNumber)
    {
        var student = FindStudent(recordBookNumber);
        if (student is null)
        {
            return false;
        }

        _students.Remove(student);
        return true;
    }

    /// <summary>
    /// Знаходить студента за номером залікової книжки.
    /// </summary>
    public Student? FindStudent(string recordBookNumber)
    {
        return _students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);
    }

    /// <summary>
    /// Знаходить студентів за фрагментом ПІБ.
    /// </summary>
    public List<Student> FindStudentByName(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
        {
            return new List<Student>();
        }

        return _students
            .Where(s => s.FullName.Contains(nameFragment.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Повертає список відмінників (середній бал ≥ 90).
    /// </summary>
    public List<Student> GetExcellentStudents()
    {
        return _students.Where(s => s.IsExcellent()).ToList();
    }

    /// <summary>
    /// Повертає студентів із заданим статусом.
    /// </summary>
    public List<Student> GetStudentsByStatus(StudentStatus status)
    {
        return _students.Where(s => s.Status == status).ToList();
    }

    /// <summary>
    /// Повертає студентів із середнім балом нижче 60.
    /// </summary>
    public List<Student> GetFailingStudents()
    {
        return _students.Where(s => s.IsFailing()).ToList();
    }

    /// <summary>
    /// Обчислює відсоток відмінників у групі.
    /// </summary>
    public double GetExcellentPercentage()
    {
        if (_students.Count == 0)
        {
            return 0;
        }

        return Math.Round((double)GetExcellentStudents().Count / _students.Count * 100, 2);
    }

    /// <summary>
    /// Зберігає дані групи у JSON-файл.
    /// </summary>
    public void SaveToFile(string path)
    {
        var options = CreateJsonOptions();
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Завантажує дані групи з JSON-файлу.
    /// </summary>
    public static StudentGroup LoadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл із даними групи не знайдено.", path);
        }

        var json = File.ReadAllText(path);
        var options = CreateJsonOptions();
        var group = JsonSerializer.Deserialize<StudentGroup>(json, options)
            ?? throw new InvalidOperationException("Не вдалося десеріалізувати дані групи.");

        foreach (var student in group._students)
        {
            student.RestoreJournalBinding();
        }

        return group;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
