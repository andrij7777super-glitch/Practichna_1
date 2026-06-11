using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentGroupApp;

/// <summary>
/// Навчальна група студентів із інтеграцією матриці портів.
/// </summary>
public class StudentGroup
{
    private readonly List<Student> _students = new();
    private readonly Dictionary<string, (int Row, int Col)> _studentPortAssignments = new();
    private PortMatrix? _portMatrix;
    private readonly PortLogger _portLogger = new();

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

    /// <summary>
    /// Знімки стану матриці портів для JSON-збереження.
    /// </summary>
    public List<PortCellSnapshot> PortSnapshots { get; set; } = new();

    /// <summary>
    /// Прив'язки студентів до координат портів (для JSON-збереження).
    /// </summary>
    public List<PortAssignmentEntry> PortAssignmentList { get; set; } = new();

    /// <summary>
    /// Збережений текст логу портів.
    /// </summary>
    public string PortLogText { get; set; } = string.Empty;

    [JsonIgnore]
    public int GroupSize => _students.Count;

    [JsonIgnore]
    public double AverageGroupGrade =>
        _students.Count == 0 ? 0 : Math.Round(_students.Average(s => s.AverageGrade), 2);

    [JsonIgnore]
    public PortMatrix? PortMatrix => _portMatrix;

    [JsonIgnore]
    public PortLogger PortLogger => _portLogger;

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
        _studentPortAssignments.Remove(recordBookNumber);
        return true;
    }

    public Student? FindStudent(string recordBookNumber) =>
        _students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

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

    public List<Student> GetExcellentStudents() => _students.Where(s => s.IsExcellent()).ToList();
    public List<Student> GetStudentsByStatus(StudentStatus status) => _students.Where(s => s.Status == status).ToList();
    public List<Student> GetFailingStudents() => _students.Where(s => s.IsFailing()).ToList();

    public double GetExcellentPercentage() =>
        _students.Count == 0 ? 0 : Math.Round((double)GetExcellentStudents().Count / _students.Count * 100, 2);

    /// <summary>
    /// Ініціалізує матрицю портів 16×16.
    /// </summary>
    public void InitializePortMatrix()
    {
        _portMatrix = new PortMatrix();
        _portMatrix.InitializeMatrix();
        _portLogger.LogOperation("Ініціалізація", 0, "Матрицю портів 16×16 створено та готово до роботи.");
    }

    /// <summary>
    /// Прив'язує студента до порту за координатами матриці.
    /// </summary>
    public void AssignStudentToPort(Student student, int row, int col)
    {
        ArgumentNullException.ThrowIfNull(student);
        EnsurePortMatrixInitialized();

        if (!_students.Contains(student))
        {
            throw new InvalidOperationException("Студент не належить до цієї групи.");
        }

        _portMatrix!.OpenPort(row, col);
        var port = _portMatrix.GetPort(row, col);
        port.DeviceName = student.FullName;
        _studentPortAssignments[student.RecordBookNumber] = (row, col);

        _portLogger.LogOperation(
            "Прив'язка студента",
            port.PortNumber,
            $"Студент {student.FullName} прив'язано до координат [{row},{col}].");
    }

    /// <summary>
    /// Повертає студентів, прив'язаних до портів із заданим статусом (відкритий/закритий).
    /// </summary>
    public List<Student> GetStudentsByPortStatus(bool isOpen)
    {
        EnsurePortMatrixInitialized();
        var result = new List<Student>();

        foreach (var assignment in _studentPortAssignments)
        {
            var student = FindStudent(assignment.Key);
            if (student is null)
            {
                continue;
            }

            var port = _portMatrix!.GetPort(assignment.Value.Row, assignment.Value.Col);
            if (port.IsOpen == isOpen)
            {
                result.Add(student);
            }
        }

        return result;
    }

    /// <summary>
    /// Симулює виконання лабораторної роботи: оцінка → масив LabGrades → порт → лог.
    /// </summary>
    public void SimulateLabWork(string recordBookNumber, int labNumber, byte grade)
    {
        var student = FindStudent(recordBookNumber)
            ?? throw new InvalidOperationException("Студента не знайдено.");

        if (!_studentPortAssignments.TryGetValue(recordBookNumber, out var coordinates))
        {
            throw new InvalidOperationException("Студент не прив'язаний до порту. Спочатку виконайте прив'язку.");
        }

        EnsurePortMatrixInitialized();

        student.AddLabGrade(labNumber, grade);

        var port = _portMatrix!.GetPort(coordinates.Row, coordinates.Col);
        var payload = Encoding.UTF8.GetBytes(
            $"LAB#{labNumber}|GRADE:{grade}|STUDENT:{student.FullName}|TIME:{DateTime.Now:HH:mm:ss}");

        _portMatrix.WriteToPort(coordinates.Row, coordinates.Col, payload);

        _portLogger.LogOperation(
            "Лабораторна робота",
            port.PortNumber,
            $"Студент {student.FullName} виконав лаб. #{labNumber}, оцінка {grade}, дані записано в порт [{coordinates.Row},{coordinates.Col}].");
    }

    /// <summary>
    /// Генерує великий звіт про стан групи через StringBuilder.
    /// </summary>
    public string GenerateFullReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("╔══════════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║         ЗВІТ СИСТЕМИ УПРАВЛІННЯ СТУДЕНТСЬКОЮ ГРУПОЮ             ║");
        sb.AppendLine("║                  Звягельський політехнічний                      ║");
        sb.AppendLine("║                     фаховий коледж (ЗПФК)                      ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════════════╝");
        sb.AppendLine();
        sb.Append("Дата формування: ").AppendLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        sb.Append("Група: ").AppendLine(GroupName);
        sb.Append("Спеціальність: ").AppendLine(Specialty);
        sb.Append("Курс: ").AppendLine(Course.ToString());
        sb.Append("Кількість студентів: ").AppendLine(GroupSize.ToString());
        sb.Append("Середній бал групи: ").AppendLine(AverageGroupGrade.ToString("F2"));
        sb.Append("Відсоток відмінників: ").AppendLine(GetExcellentPercentage().ToString("F2") + "%");
        sb.AppendLine();
        sb.AppendLine("─── СТУДЕНТИ ТА ЛАБОРАТОРНІ ОЦІНКИ ───");

        foreach (var student in _students)
        {
            sb.AppendLine();
            sb.Append("ПІБ: ").AppendLine(student.FullName);
            sb.Append("Залікова: ").AppendLine(student.RecordBookNumber);
            sb.Append("Середній бал (лаби): ").AppendLine(student.GetAverageLabGrade().ToString("F2"));
            sb.Append("Відсортовані оцінки лаб: [");
            sb.Append(string.Join(", ", student.GetSortedLabGrades().Select(g => g == 0 ? "—" : g.ToString())));
            sb.AppendLine("]");
        }

        sb.AppendLine();
        sb.AppendLine("─── СТАН МАТРИЦІ ПОРТІВ ───");
        if (_portMatrix is not null && _portMatrix.IsInitialized)
        {
            sb.AppendLine(_portMatrix.GetFormattedMatrixState());
            sb.Append("Прив'язок студентів до портів: ").AppendLine(_studentPortAssignments.Count.ToString());
        }
        else
        {
            sb.AppendLine("Матриця портів не ініціалізована.");
        }

        sb.AppendLine();
        sb.AppendLine("─── ЛОГ ОПЕРАЦІЙ ПОРТІВ ───");
        sb.AppendLine(_portLogger.GetFullLog());

        return sb.ToString();
    }

    public void SaveToFile(string path)
    {
        if (_portMatrix is not null && _portMatrix.IsInitialized)
        {
            PortSnapshots = _portMatrix.ExportState();
        }

        PortAssignmentList = _studentPortAssignments
            .Select(pair => new PortAssignmentEntry
            {
                RecordBookNumber = pair.Key,
                Row = pair.Value.Row,
                Col = pair.Value.Col
            })
            .ToList();

        PortLogText = _portLogger.GetFullLog();

        var options = CreateJsonOptions();
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(path, json);
    }

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

        if (group.PortSnapshots.Count > 0)
        {
            group._portMatrix = new PortMatrix();
            group._portMatrix.ImportState(group.PortSnapshots);
        }

        group._studentPortAssignments.Clear();
        foreach (var entry in group.PortAssignmentList)
        {
            group._studentPortAssignments[entry.RecordBookNumber] = (entry.Row, entry.Col);
        }

        group._portLogger.RestoreFromText(group.PortLogText);

        return group;
    }

    private void EnsurePortMatrixInitialized()
    {
        if (_portMatrix is null || !_portMatrix.IsInitialized)
        {
            throw new InvalidOperationException("Матриця портів не ініціалізована. Використайте пункт меню «Ініціалізувати матрицю портів».");
        }
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

/// <summary>
/// Запис про прив'язку студента до порту.
/// </summary>
public class PortAssignmentEntry
{
    public string RecordBookNumber { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
}
