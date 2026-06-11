using System.Text;
using StudentGroupApp;

var group = CreateGroupWithTestData();

while (true)
{
    PrintMainMenu(group);
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1": AddStudent(group); break;
            case "2": ShowStudentsWithLabGrades(group); break;
            case "3": InitializePortMatrix(group); break;
            case "4": TogglePort(group); break;
            case "5": WriteOrReadPort(group); break;
            case "6": ShowMatrixState(group); break;
            case "7": AssignStudentToPortMenu(group); break;
            case "8": SimulateLabWorkMenu(group); break;
            case "9": ShowPortLog(group); break;
            case "10": SearchOpenPort(group); break;
            case "11": GenerateLargeReport(group); break;
            case "12": SaveOrLoadData(ref group); break;
            case "0":
                Console.WriteLine("До побачення! Система управління студентською групою завершує роботу.");
                return;
            default:
                Console.WriteLine("Невірний пункт меню. Спробуйте ще раз.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Натисніть будь-яку клавішу, щоб продовжити...");
    Console.ReadKey(true);
    Console.Clear();
}

static void PrintMainMenu(StudentGroup group)
{
    var sb = new StringBuilder();
    sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
    sb.AppendLine("║   Система управління студентською групою (ЗПФК)          ║");
    sb.AppendLine("║              Практична робота №2 — Масиви                ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.AppendLine("║  1. Додати студента                                      ║");
    sb.AppendLine("║  2. Вивести студентів з оцінками лабораторних            ║");
    sb.AppendLine("║  3. Ініціалізувати матрицю портів 16×16                  ║");
    sb.AppendLine("║  4. Відкрити/закрити порт                                ║");
    sb.AppendLine("║  5. Записати дані в порт / Прочитати з порту             ║");
    sb.AppendLine("║  6. Вивести стан матриці портів                          ║");
    sb.AppendLine("║  7. Прив'язати студента до порту                         ║");
    sb.AppendLine("║  8. Симулювати лабораторну роботу                        ║");
    sb.AppendLine("║  9. Переглянути лог портів                               ║");
    sb.AppendLine("║ 10. Пошук відкритого порту                               ║");
    sb.AppendLine("║ 11. Згенерувати великий звіт (StringBuilder)             ║");
    sb.AppendLine("║ 12. Зберегти / Завантажити дані                          ║");
    sb.AppendLine("║  0. Вийти                                                ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.Append("║  Група: ").Append(group.GroupName.PadRight(47)).AppendLine("║");
    sb.Append("║  Студентів: ").Append(group.GroupSize.ToString().PadRight(43)).AppendLine("║");
    sb.Append("║  Матриця портів: ").Append((group.PortMatrix?.IsInitialized == true ? "ініціалізована" : "не ініціалізована").PadRight(38)).AppendLine("║");
    sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
    Console.Write(sb.ToString());
}

static StudentGroup CreateGroupWithTestData()
{
    var studentGroup = new StudentGroup
    {
        GroupName = "КН-21",
        Specialty = "Комп'ютерні науки",
        Course = 2
    };

    var student1 = new Student
    {
        FullName = "Коваленко Андрій Іванович",
        DateOfBirth = new DateTime(2005, 4, 12),
        RecordBookNumber = "24010001",
        PersonalEmail = "andriy.kovalenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.Active,
        Notes = "Староста групи."
    };
    student1.Journal.AddOrUpdateGrade("Програмування", 92);
    student1.AddLabGrade(1, 95);
    student1.AddLabGrade(2, 88);

    var student2 = new Student
    {
        FullName = "Шевченко Олена Петрівна",
        DateOfBirth = new DateTime(2006, 1, 25),
        RecordBookNumber = "24010002",
        PersonalEmail = "olena.shevchenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.Active,
        Notes = "Графічний дизайн."
    };
    student2.Journal.AddOrUpdateGrade("Програмування", 85);
    student2.AddLabGrade(1, 78);
    student2.AddLabGrade(3, 82);

    var student3 = new Student
    {
        FullName = "Мельник Тарас Васильович",
        DateOfBirth = new DateTime(2004, 11, 3),
        RecordBookNumber = "24010003",
        PersonalEmail = "taras.melnyk@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.AcademicLeave,
        Notes = "Академічна відпустка."
    };
    student3.AddLabGrade(1, 55);

    var student4 = new Student
    {
        FullName = "Бондаренко Марія Сергіївна",
        DateOfBirth = new DateTime(2005, 8, 17),
        RecordBookNumber = "24010004",
        PersonalEmail = "maria.bondarenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.Active,
        Notes = "Веб-розробка."
    };
    student4.Journal.AddOrUpdateGrade("Бази даних", 96);
    student4.AddLabGrade(1, 94);
    student4.AddLabGrade(2, 91);

    studentGroup.AddStudent(student1);
    studentGroup.AddStudent(student2);
    studentGroup.AddStudent(student3);
    studentGroup.AddStudent(student4);

    studentGroup.InitializePortMatrix();
    studentGroup.AssignStudentToPort(student1, 0, 0);
    studentGroup.AssignStudentToPort(student2, 0, 1);
    studentGroup.AssignStudentToPort(student4, 1, 0);

    return studentGroup;
}

static void AddStudent(StudentGroup group)
{
    Console.WriteLine("--- Додавання нового студента ---");
    Console.Write("ПІБ: ");
    var fullName = ReadRequiredLine();
    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dateOfBirth = ReadDate();
    Console.Write("Номер залікової (8 цифр): ");
    var recordBook = ReadRequiredLine();
    Console.Write("Email: ");
    var email = ReadRequiredLine();
    Console.Write("Дата зарахування (дд.ММ.рррр): ");
    var enrollment = ReadDate();

    var student = new Student
    {
        FullName = fullName,
        DateOfBirth = dateOfBirth,
        RecordBookNumber = recordBook,
        PersonalEmail = email,
        EnrollmentDate = enrollment
    };

    group.AddStudent(student);
    Console.WriteLine($"Студента {student.FullName} успішно додано.");
}

static void ShowStudentsWithLabGrades(StudentGroup group)
{
    Console.WriteLine("--- Студенти з оцінками лабораторних (відсортований масив) ---");

    if (group.GroupSize == 0)
    {
        Console.WriteLine("У групі немає студентів.");
        return;
    }

    var sb = new StringBuilder();
    sb.AppendLine(new string('=', 80));

    foreach (var student in group.Students)
    {
        var sorted = student.GetSortedLabGrades();
        sb.Append("ПІБ: ").AppendLine(student.FullName);
        sb.Append("Залікова: ").AppendLine(student.RecordBookNumber);
        sb.Append("Середній бал (лаби): ").AppendLine(student.GetAverageLabGrade().ToString("F2"));
        sb.Append("Оцінки (відсортовані): [");

        var gradeStrings = sorted.Select(g => g == 0 ? "—" : g.ToString());
        sb.Append(string.Join(", ", gradeStrings));
        sb.AppendLine("]");
        sb.AppendLine(new string('-', 80));
    }

    Console.Write(sb.ToString());
}

static void InitializePortMatrix(StudentGroup group)
{
    if (group.PortMatrix?.IsInitialized == true)
    {
        Console.Write("Матриця вже ініціалізована. Перестворити? (т/н): ");
        if (!ReadYesNo())
        {
            return;
        }
    }

    group.InitializePortMatrix();
    Console.WriteLine("Матрицю портів 16×16 успішно ініціалізовано.");
}

static void TogglePort(StudentGroup group)
{
    EnsureMatrix(group);
    Console.WriteLine("--- Відкриття / закриття порту ---");
    Console.Write("Рядок (0-15): ");
    var row = ReadIntInRange(0, 15);
    Console.Write("Стовпець (0-15): ");
    var col = ReadIntInRange(0, 15);

    Console.WriteLine("1. Відкрити  2. Закрити");
    Console.Write("Вибір: ");
    var choice = Console.ReadLine();

    var port = group.PortMatrix!.GetPort(row, col);
    switch (choice)
    {
        case "1":
            group.PortMatrix.OpenPort(row, col);
            group.PortLogger.LogOperation("Відкриття", port.PortNumber, $"Порт [{row},{col}] відкрито.");
            Console.WriteLine($"Порт #{port.PortNumber} [{row},{col}] відкрито.");
            break;
        case "2":
            group.PortMatrix.ClosePort(row, col);
            group.PortLogger.LogOperation("Закриття", port.PortNumber, $"Порт [{row},{col}] закрито.");
            Console.WriteLine($"Порт #{port.PortNumber} [{row},{col}] закрито.");
            break;
        default:
            Console.WriteLine("Невірний вибір.");
            break;
    }
}

static void WriteOrReadPort(StudentGroup group)
{
    EnsureMatrix(group);
    Console.WriteLine("--- Запис / читання даних порту ---");
    Console.WriteLine("1. Записати  2. Прочитати");
    Console.Write("Вибір: ");
    var choice = Console.ReadLine();
    Console.Write("Рядок (0-15): ");
    var row = ReadIntInRange(0, 15);
    Console.Write("Стовпець (0-15): ");
    var col = ReadIntInRange(0, 15);

    var port = group.PortMatrix!.GetPort(row, col);

    if (choice == "1")
    {
        Console.Write("Текст для запису в буфер (до 64 байт): ");
        var text = ReadRequiredLine();
        var data = Encoding.UTF8.GetBytes(text);
        group.PortMatrix.WriteToPort(row, col, data);
        group.PortLogger.LogOperation("Запис", port.PortNumber, $"Записано {data.Length} байт у порт [{row},{col}].");
        Console.WriteLine("Дані успішно записано.");
    }
    else if (choice == "2")
    {
        var data = group.PortMatrix.ReadFromPort(row, col);
        var text = Encoding.UTF8.GetString(data).TrimEnd('\0');
        group.PortLogger.LogOperation("Читання", port.PortNumber, $"Прочитано дані з порту [{row},{col}].");
        Console.WriteLine($"Вміст буфера: {text}");
    }
    else
    {
        Console.WriteLine("Невірний вибір.");
    }
}

static void ShowMatrixState(StudentGroup group)
{
    EnsureMatrix(group);
    Console.WriteLine(group.PortMatrix!.GetFormattedMatrixState());
}

static void AssignStudentToPortMenu(StudentGroup group)
{
    EnsureMatrix(group);
    Console.WriteLine("--- Прив'язка студента до порту ---");
    Console.Write("Номер залікової студента: ");
    var recordBook = ReadRequiredLine();
    var student = group.FindStudent(recordBook);
    if (student is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    Console.Write("Рядок (0-15): ");
    var row = ReadIntInRange(0, 15);
    Console.Write("Стовпець (0-15): ");
    var col = ReadIntInRange(0, 15);

    group.AssignStudentToPort(student, row, col);
    Console.WriteLine($"Студента {student.FullName} прив'язано до порту [{row},{col}].");
}

static void SimulateLabWorkMenu(StudentGroup group)
{
    Console.WriteLine("--- Симуляція лабораторної роботи ---");
    Console.Write("Номер залікової студента: ");
    var recordBook = ReadRequiredLine();
    Console.Write("Номер лабораторної (1-10): ");
    var labNumber = ReadIntInRange(1, 10);
    Console.Write("Оцінка (0-100): ");
    var grade = (byte)ReadIntInRange(0, 100);

    group.SimulateLabWork(recordBook, labNumber, grade);
    var student = group.FindStudent(recordBook)!;
    Console.WriteLine($"Лабораторну #{labNumber} виконано. Оцінка {grade} збережена.");
    Console.WriteLine($"Середній бал за лаби: {student.GetAverageLabGrade():F2}");
}

static void ShowPortLog(StudentGroup group)
{
    Console.WriteLine("--- Лог операцій портів ---");
    Console.WriteLine(group.PortLogger.GetFullLog());
    Console.Write("Зберегти лог у файл port_log.txt? (т/н): ");
    if (ReadYesNo())
    {
        group.PortLogger.SaveLogToFile("port_log.txt");
        Console.WriteLine("Лог збережено у файл port_log.txt");
    }
}

static void SearchOpenPort(StudentGroup group)
{
    EnsureMatrix(group);
    Console.WriteLine("--- Пошук відкритого порту у матриці 16×16 ---");
    Console.WriteLine("1. За номером порту  2. Сканувати всі відкриті");
    Console.Write("Вибір: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Номер порту (1-256): ");
        var portNumber = ReadIntInRange(1, 256);
        var found = group.PortMatrix!.FindOpenPortByNumber(portNumber);

        if (found is null)
        {
            Console.WriteLine("Відкритий порт з таким номером не знайдено.");
        }
        else
        {
            var port = group.PortMatrix.GetPort(found.Value.Row, found.Value.Col);
            Console.WriteLine($"Знайдено: Порт #{port.PortNumber} [{found.Value.Row},{found.Value.Col}], пристрій: {port.DeviceName}");
            Console.WriteLine($"Вміст буфера: {port.GetBufferAsString()}");
        }
    }
    else if (choice == "2")
    {
        var openPorts = group.PortMatrix!.ScanMatrix();
        var sb = new StringBuilder();
        sb.AppendLine($"Відкритих портів: {openPorts.Count}");
        foreach (var port in openPorts)
        {
            sb.Append("  #").Append(port.PortNumber)
                .Append(" | ").Append(port.DeviceName)
                .Append(" | Буфер: ").AppendLine(port.GetBufferAsString());
        }

        Console.Write(sb.ToString());
    }
    else
    {
        Console.WriteLine("Невірний вибір.");
    }
}

static void GenerateLargeReport(StudentGroup group)
{
    Console.WriteLine("--- Генерація великого звіту (StringBuilder) ---");
    var report = group.GenerateFullReport();
    Console.WriteLine(report);
    Console.Write("Зберегти звіт у файл full_report.txt? (т/н): ");
    if (ReadYesNo())
    {
        File.WriteAllText("full_report.txt", report, Encoding.UTF8);
        Console.WriteLine("Звіт збережено у full_report.txt");
    }
}

static void SaveOrLoadData(ref StudentGroup group)
{
    Console.WriteLine("--- Збереження / завантаження даних ---");
    Console.WriteLine("1. Зберегти  2. Завантажити");
    Console.Write("Вибір: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Шлях до файлу (наприклад, group_data.json): ");
        var path = ReadRequiredLine();
        group.SaveToFile(path);
        Console.WriteLine($"Дані збережено: {path}");
    }
    else if (choice == "2")
    {
        Console.Write("Шлях до JSON файлу: ");
        var path = ReadRequiredLine();
        group = StudentGroup.LoadFromFile(path);
        Console.WriteLine($"Дані завантажено. Студентів: {group.GroupSize}");
    }
    else
    {
        Console.WriteLine("Невірний вибір.");
    }
}

static void EnsureMatrix(StudentGroup group)
{
    if (group.PortMatrix?.IsInitialized != true)
    {
        throw new InvalidOperationException("Спочатку ініціалізуйте матрицю портів (пункт меню 3).");
    }
}

static string ReadRequiredLine()
{
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
    {
        throw new ArgumentException("Поле не може бути порожнім.");
    }

    return line.Trim();
}

static DateTime ReadDate()
{
    var input = ReadRequiredLine();
    if (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
    {
        throw new FormatException("Невірний формат дати. Використовуйте дд.ММ.рррр.");
    }

    return date;
}

static int ReadIntInRange(int min, int max)
{
    var input = ReadRequiredLine();
    if (!int.TryParse(input, out var value) || value < min || value > max)
    {
        throw new FormatException($"Введіть ціле число від {min} до {max}.");
    }

    return value;
}

static bool ReadYesNo()
{
    var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
    return answer is "т" or "так" or "y" or "yes";
}
