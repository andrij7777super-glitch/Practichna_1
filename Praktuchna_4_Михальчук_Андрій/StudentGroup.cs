using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentGroupApp;

public class StudentGroup
{
    private readonly List<Student> _students = new();

    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int Course { get; set; }

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

    [JsonIgnore]
    public int GroupSize => _students.Count;

    [JsonIgnore]
    public double AverageGroupGrade =>
        _students.Count == 0 ? 0 : Math.Round(_students.Average(s => s.AverageGrade), 2);

    public Student? this[string recordBookNumber] => FindStudent(recordBookNumber);

    public void AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (_students.Any(s => s.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException($"Студент із номером залікової {student.RecordBookNumber} вже існує в групі.");
        }

        _students.Add(student);
    }

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

    public Student? FindStudent(string recordBookNumber) =>
        _students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public List<Student> GetExcellentStudents() => _students.Where(s => s.IsExcellent()).ToList();
    public List<Student> GetFailingStudents() => _students.Where(s => s.IsFailing()).ToList();

    public Student? BestStudent()
    {
        if (_students.Count == 0)
        {
            return null;
        }

        var best = _students[0];
        for (var i = 1; i < _students.Count; i++)
        {
            if (_students[i] > best)
            {
                best = _students[i];
            }
        }

        return best;
    }

    public StudentGroup MergeGroups(StudentGroup other) => this + other;

    public static StudentGroup operator +(StudentGroup a, StudentGroup b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var merged = new StudentGroup
        {
            GroupName = $"{a.GroupName}+{b.GroupName}",
            Specialty = string.IsNullOrWhiteSpace(a.Specialty) ? b.Specialty : a.Specialty,
            Course = Math.Max(a.Course, b.Course)
        };

        foreach (var student in a._students)
        {
            merged.AddStudent((Student)student.Clone());
        }

        foreach (var student in b._students)
        {
            if (merged.FindStudent(student.RecordBookNumber) is null)
            {
                merged.AddStudent((Student)student.Clone());
            }
        }

        return merged;
    }

    public void SaveToFile(string path)
    {
        var options = CreateJsonOptions();
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(path, json);
    }

    public static StudentGroup LoadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл не знайдено.", path);
        }

        var json = File.ReadAllText(path);
        var group = JsonSerializer.Deserialize<StudentGroup>(json, CreateJsonOptions())
            ?? throw new InvalidOperationException("Не вдалося завантажити дані.");

        foreach (var student in group._students)
        {
            student.RestoreJournalBinding();
        }

        return group;
    }

    private static JsonSerializerOptions CreateJsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
