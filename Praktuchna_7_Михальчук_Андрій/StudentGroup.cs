using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentGroupApp;

public class StudentGroup
{
    private readonly List<UniversityMember> _members = new();
    private readonly Dictionary<string, (int Row, int Col)> _studentPortAssignments = new();
    private PortMatrix? _portMatrix;
    private readonly PortLogger _portLogger = new();

    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int Course { get; set; }
    public Point[] LabSeats { get; set; } = Array.Empty<Point>();

    [JsonInclude]
    public List<Student> Students
    {
        get => _members.OfType<Student>().ToList();
        set
        {
            _members.RemoveAll(m => m is Student);
            if (value is not null)
            {
                foreach (var student in value)
                {
                    _members.Add(student);
                }
            }
        }
    }

    public List<PortCellSnapshot> PortSnapshots { get; set; } = new();
    public List<PortAssignmentEntry> PortAssignmentList { get; set; } = new();
    public string PortLogText { get; set; } = string.Empty;

    [JsonIgnore]
    public int GroupSize => _members.Count;

    [JsonIgnore]
    public int StudentCount => _members.Count(m => m is Student);

    [JsonIgnore]
    public double AverageGroupGrade
    {
        get
        {
            var students = _members.OfType<Student>().ToList();
            return students.Count == 0 ? 0 : Math.Round(students.Average(s => s.AverageGrade), 2);
        }
    }

    [JsonIgnore]
    public IReadOnlyList<UniversityMember> Members => _members.AsReadOnly();

    [JsonIgnore]
    public PortMatrix? PortMatrix => _portMatrix;

    [JsonIgnore]
    public PortLogger PortLogger => _portLogger;

    public Student? this[string recordBookNumber] => FindStudent(recordBookNumber);

    public void AddMember(UniversityMember member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (member is Student student &&
            _members.OfType<Student>().Any(s => s.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException(
                $"Студент із номером залікової {student.RecordBookNumber} вже існує в групі.");
        }

        _members.Add(member);
    }

    public void AddStudent(Student student) => AddMember(student);

    public bool RemoveMember(UniversityMember member)
    {
        if (!_members.Remove(member))
        {
            return false;
        }

        if (member is Student s)
        {
            _studentPortAssignments.Remove(s.RecordBookNumber);
        }

        return true;
    }

    public bool RemoveStudent(string recordBookNumber)
    {
        var student = FindStudent(recordBookNumber);
        if (student is null)
        {
            return false;
        }

        return RemoveMember(student);
    }

    public Student? FindStudent(string recordBookNumber) =>
        _members.OfType<Student>().FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public List<Student> FindStudentByName(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
        {
            return new List<Student>();
        }

        return _members
            .OfType<Student>()
            .Where(s => s.FullName.Contains(nameFragment.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<T> GetMembersByType<T>() where T : UniversityMember =>
        _members.OfType<T>().ToList();

    public decimal GetTotalScholarship() =>
        _members.Sum(m => m.CalculateScholarship());

    public void OptimizeStorage()
    {
        var count = StudentCount;
        var seats = new Point[count];
        for (var i = 0; i < count; i++)
        {
            seats[i] = new Point(i % 4, i / 4);
        }

        LabSeats = seats;
    }

    public StudentRecord[] GetAllRecords() =>
        _members.OfType<Student>().Select(s => s.GetRecord()).ToArray();

    public double GetTotalAreaOfAllShapes() =>
        _members.OfType<Student>().SelectMany(s => s.Projects).Sum(s => s.CalculateArea());

    public void DrawAllShapes()
    {
        foreach (var shape in _members.OfType<Student>().SelectMany(s => s.Projects))
        {
            shape.Draw();
        }
    }

    public void ResizeAllShapes(double factor)
    {
        foreach (var shape in _members.OfType<Student>().SelectMany(s => s.Projects))
        {
            shape.Resize(factor);
        }
    }

    public List<Student> GetExcellentStudents() =>
        _members.OfType<Student>().Where(s => s.IsExcellent()).ToList();

    public List<Student> GetStudentsByStatus(StudentStatus status) =>
        _members.OfType<Student>().Where(s => s.Status == status).ToList();

    public List<Student> GetFailingStudents() =>
        _members.OfType<Student>().Where(s => s.IsFailing()).ToList();

    public double GetExcellentPercentage()
    {
        var students = _members.OfType<Student>().ToList();
        return students.Count == 0 ? 0 : Math.Round((double)GetExcellentStudents().Count / students.Count * 100, 2);
    }

    public string SearchByNameFragment(string fragment)
    {
        var sb = new StringBuilder();
        sb.AppendLine("--- Результати пошуку за фрагментом ПІБ ---");

        if (string.IsNullOrWhiteSpace(fragment))
        {
            sb.AppendLine("Фрагмент пошуку не задано.");
            return sb.ToString();
        }

        var found = FindStudentByName(fragment);
        if (found.Count == 0)
        {
            sb.AppendLine("Збігів не знайдено.");
            return sb.ToString();
        }

        sb.Append("Знайдено: ").AppendLine(found.Count.ToString());
        foreach (var student in found)
        {
            sb.AppendLine(student.GetFormattedInfo(detailed: true));
            sb.AppendLine(new string('-', 40));
        }

        return sb.ToString();
    }

    public string ExportToCsv()
    {
        var sb = new StringBuilder();
        sb.AppendLine("FullName,RecordBookNumber,Email,AverageGrade,Status,Notes,MemberType");

        foreach (var member in _members)
        {
            if (member is Student student)
            {
                sb.Append('"').Append(student.FullName.Replace("\"", "\"\"")).Append('"').Append(',');
                sb.Append(student.RecordBookNumber).Append(',');
                sb.Append(student.PersonalEmail).Append(',');
                sb.Append(student.AverageGrade.ToString("F2")).Append(',');
                sb.Append(student.Status).Append(',');
                sb.Append('"').Append(student.Notes.Replace("\"", "\"\"")).Append('"').Append(',');
                sb.AppendLine(student.GetType().Name);
            }
            else
            {
                sb.Append('"').Append(member.FullName.Replace("\"", "\"\"")).Append('"').Append(',');
                sb.Append("N/A,");
                sb.Append(member.PersonalEmail).Append(',');
                sb.Append("0,");
                sb.Append("N/A,");
                sb.Append('"').Append(member.Notes.Replace("\"", "\"\"")).Append('"').Append(',');
                sb.AppendLine(member.GetType().Name);
            }
        }

        return sb.ToString();
    }

    public void ImportStudentsFromText(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            throw new ArgumentException("Текст для імпорту не може бути порожнім.", nameof(rawText));
        }

        var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var imported = 0;

        foreach (var line in lines)
        {
            if (line.StartsWith("FullName", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parts = line.Split(';');
            if (parts.Length < 4)
            {
                continue;
            }

            var fullName = TextProcessor.Normalize(parts[0]);
            var recordBook = parts[1].Trim();
            var email = parts[2].Trim();

            if (!DateTime.TryParseExact(parts[3].Trim(), "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var birthDate))
            {
                continue;
            }

            if (FindStudent(recordBook) is not null)
            {
                continue;
            }

            var student = new Student
            {
                FullName = fullName,
                RecordBookNumber = recordBook,
                PersonalEmail = email,
                DateOfBirth = birthDate
            };
            student.Enroll();

            AddMember(student);
            imported++;
        }

        if (imported == 0)
        {
            throw new InvalidOperationException(
                "Не вдалося імпортувати жодного студента. Формат: ПІБ;залікова;email;дд.ММ.рррр");
        }
    }

    public int NormalizeAllNotes()
    {
        var count = 0;
        foreach (var member in _members)
        {
            var normalized = TextProcessor.Normalize(member.Notes);
            if (!string.Equals(member.Notes, normalized, StringComparison.Ordinal))
            {
                member.Notes = normalized;
                count++;
            }
        }

        return count;
    }

    public Student? BestStudent()
    {
        var students = _members.OfType<Student>().ToList();
        if (students.Count == 0)
        {
            return null;
        }

        var best = students[0];
        for (var i = 1; i < students.Count; i++)
        {
            if (students[i] > best)
            {
                best = students[i];
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

        foreach (var member in a._members)
        {
            if (member is Student student)
            {
                merged.AddMember((Student)student.Clone());
            }
            else
            {
                merged.AddMember(member);
            }
        }

        foreach (var member in b._members)
        {
            if (member is Student student)
            {
                if (merged.FindStudent(student.RecordBookNumber) is null)
                {
                    merged.AddMember((Student)student.Clone());
                }
            }
            else if (!merged._members.Contains(member))
            {
                merged.AddMember(member);
            }
        }

        return merged;
    }

    public void InitializePortMatrix()
    {
        _portMatrix = new PortMatrix();
        _portMatrix.InitializeMatrix();
        _portLogger.LogOperation("Ініціалізація", 0, "Матрицю портів 16×16 створено та готово до роботи.");
    }

    public void AssignStudentToPort(Student student, int row, int col)
    {
        ArgumentNullException.ThrowIfNull(student);
        EnsurePortMatrixInitialized();

        if (!_members.Contains(student))
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

    public List<Student> GetStudentsByPortStatus(bool isOpen)
    {
        EnsurePortMatrixInitialized();
        var result = new List<Student>();

        foreach (var assignment in _studentPortAssignments)
        {
            if (FindStudent(assignment.Key) is not Student student)
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

    public void SimulateLabWork(string recordBookNumber, int labNumber, byte grade)
    {
        if (FindStudent(recordBookNumber) is not Student student)
        {
            throw new InvalidOperationException("Студента не знайдено.");
        }

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
        sb.Append("Учасників: ").AppendLine(GroupSize.ToString());
        sb.Append("Студентів: ").AppendLine(StudentCount.ToString());
        sb.Append("Середній бал групи: ").AppendLine(AverageGroupGrade.ToString("F2"));
        sb.Append("Загальна стипендія: ").AppendLine(GetTotalScholarship().ToString("F2") + " грн");
        sb.Append("Відсоток відмінників: ").AppendLine(GetExcellentPercentage().ToString("F2") + "%");
        sb.AppendLine();
        sb.AppendLine("─── УЧАСНИКИ ТА ЛАБОРАТОРНІ ОЦІНКИ ───");

        foreach (var member in _members)
        {
            sb.AppendLine();
            sb.AppendLine(member.GetInfo());

            if (member is Student student)
            {
                sb.Append("Середній бал (лаби): ").AppendLine(student.GetAverageLabGrade().ToString("F2"));
                sb.Append("Відсортовані оцінки лаб: [");
                sb.Append(string.Join(", ", student.GetSortedLabGrades().Select(g => g == 0 ? "—" : g.ToString())));
                sb.AppendLine("]");
            }
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

    public void PrintAllMembers()
    {
        if (_members.Count == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        for (var i = 0; i < _members.Count; i++)
        {
            Console.WriteLine($"--- Учасник #{i + 1} ({_members[i].GetType().Name}) ---");
            Console.WriteLine(_members[i].GetInfo());
            Console.WriteLine();
        }
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
        var group = JsonSerializer.Deserialize<StudentGroup>(json, CreateJsonOptions())
            ?? throw new InvalidOperationException("Не вдалося десеріалізувати дані групи.");

        group._members.Clear();
        foreach (var student in group.Students)
        {
            group._members.Add(student);
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
            throw new InvalidOperationException(
                "Матриця портів не ініціалізована. Використайте пункт меню «Ініціалізувати матрицю портів».");
        }
    }

    private static JsonSerializerOptions CreateJsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}

public class PortAssignmentEntry
{
    public string RecordBookNumber { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
}
