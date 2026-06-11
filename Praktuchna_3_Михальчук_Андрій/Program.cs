using System.Text;
using StudentGroupApp;

var group = CreateGroupWithTestData();
var logger = new AdvancedLogger();
logger.Log("INFO", "Систему Student Group Management System (ПР №3) запущено.");

while (true)
{
    PrintMainMenu(group);
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1": ManageStudents(group, logger); break;
            case "2": ShowAndSearchStudents(group, logger); break;
            case "3": ShowStatistics(group); break;
            case "4": ExportImportMenu(group, logger); break;
            case "5": GenerateReport(group, logger); break;
            case "6": NormalizeNotes(group, logger); break;
            case "7": CheckPalindromes(group); break;
            case "8": GenerateSqlMenu(logger); break;
            case "9": ViewLogs(logger); break;
            case "10": RunBenchmark(logger); break;
            case "11": SaveOrLoad(ref group, logger); break;
            case "0":
                logger.Log("INFO", "Завершення роботи програми.");
                Console.WriteLine("До побачення!");
                return;
            default:
                Console.WriteLine("Невірний пункт меню.");
                break;
        }
    }
    catch (Exception ex)
    {
        logger.Log("ERROR", ex.Message);
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Натисніть будь-яку клавішу...");
    Console.ReadKey(true);
    Console.Clear();
}

static void PrintMainMenu(StudentGroup group)
{
    var sb = new StringBuilder();
    sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
    sb.AppendLine("║   СГМС (ЗПФК) — Практична робота №3: Робота з текстом    ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.AppendLine("║  1. Управління студентами (Додати/Видалити/Редагувати)   ║");
    sb.AppendLine("║  2. Вивести студентів / Пошук за фрагментом ПІБ          ║");
    sb.AppendLine("║  3. Статистика та фільтри (Відмінники, боржники)        ║");
    sb.AppendLine("║  4. Експорт у CSV / Імпорт з тексту                      ║");
    sb.AppendLine("║  5. Згенерувати повний звіт групи (StringBuilder)        ║");
    sb.AppendLine("║  6. Нормалізувати нотатки всіх студентів                 ║");
    sb.AppendLine("║  7. Перевірити паліндроми в нотатках                     ║");
    sb.AppendLine("║  8. Автогенерація SQL-запиту з тексту (Варіант 5)        ║");
    sb.AppendLine("║  9. Переглянути логи системи (AdvancedLogger)            ║");
    sb.AppendLine("║ 10. Порівняти string vs StringBuilder (бенчмарк)         ║");
    sb.AppendLine("║ 11. Зберегти / Завантажити дані (JSON)                   ║");
    sb.AppendLine("║  0. Вийти                                                ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.Append("║  Група: ").Append(group.GroupName.PadRight(47)).AppendLine("║");
    sb.Append("║  Студентів: ").Append(group.GroupSize.ToString().PadRight(43)).AppendLine("║");
    sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
    Console.Write(sb.ToString());
}

static StudentGroup CreateGroupWithTestData()
{
    var g = new StudentGroup { GroupName = "КН-21", Specialty = "Комп'ютерні науки", Course = 2 };

    var s1 = new Student
    {
        FullName = "Коваленко Андрій Іванович",
        DateOfBirth = new DateTime(2005, 4, 12),
        RecordBookNumber = "24010001",
        PersonalEmail = "andriy.kovalenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Notes = "  Староста   групи  "
    };
    s1.Journal.AddOrUpdateGrade("Програмування", 92);

    var s2 = new Student
    {
        FullName = "Шевченко Олена Петрівна",
        DateOfBirth = new DateTime(2006, 1, 25),
        RecordBookNumber = "24010002",
        PersonalEmail = "olena.shevchenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Notes = "А роза упала на лапу Азора"
    };
    s2.Journal.AddOrUpdateGrade("Програмування", 85);

    var s3 = new Student
    {
        FullName = "Бондаренко Марія Сергіївна",
        DateOfBirth = new DateTime(2005, 8, 17),
        RecordBookNumber = "24010004",
        PersonalEmail = "maria.bondarenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Notes = "Веб-розробка"
    };
    s3.Journal.AddOrUpdateGrade("Бази даних", 96);

    g.AddStudent(s1);
    g.AddStudent(s2);
    g.AddStudent(s3);
    return g;
}

static void ManageStudents(StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Додати  2. Видалити  3. Редагувати");
    Console.Write("Вибір: ");
    switch (Console.ReadLine())
    {
        case "1":
            Console.Write("ПІБ: ");
            var name = ReadLine();
            Console.Write("Залікова (8 цифр): ");
            var rb = ReadLine();
            Console.Write("Email: ");
            var email = ReadLine();
            Console.Write("Дата народження (дд.ММ.рррр): ");
            var dob = ReadDate();
            var student = new Student
            {
                FullName = name,
                RecordBookNumber = rb,
                PersonalEmail = email,
                DateOfBirth = dob,
                EnrollmentDate = DateTime.Today
            };
            group.AddStudent(student);
            logger.Log("INFO", $"Додано студента: {name}");
            Console.WriteLine("Студента додано.");
            break;
        case "2":
            Console.Write("Залікова: ");
            if (group.RemoveStudent(ReadLine()))
            {
                logger.Log("INFO", "Студента видалено.");
                Console.WriteLine("Видалено.");
            }
            else Console.WriteLine("Не знайдено.");
            break;
        case "3":
            Console.Write("Залікова: ");
            var s = group.FindStudent(ReadLine());
            if (s is null) { Console.WriteLine("Не знайдено."); return; }
            Console.Write("Нові нотатки: ");
            s.Notes = Console.ReadLine() ?? "";
            Console.Write("Новий статус (1-Активний,2-Відпустка,3-Відрахований,4-Випускник): ");
            s.Status = ReadStatus();
            logger.Log("INFO", $"Оновлено студента: {s.FullName}");
            Console.WriteLine("Оновлено.");
            break;
    }
}

static void ShowAndSearchStudents(StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Вивести всіх  2. Пошук за фрагментом  3. Пошук за ключовим словом");
    Console.Write("Вибір: ");
    switch (Console.ReadLine())
    {
        case "1":
            var sb = new StringBuilder();
            foreach (var s in group.Students)
            {
                sb.AppendLine(s.GetFormattedInfo(detailed: true));
                sb.AppendLine(new string('-', 40));
            }
            Console.Write(sb.ToString());
            break;
        case "2":
            Console.Write("Фрагмент ПІБ: ");
            var result = group.SearchByNameFragment(ReadLine());
            Console.WriteLine(result);
            logger.Log("INFO", "Виконано пошук за фрагментом ПІБ.");
            break;
        case "3":
            Console.Write("Ключове слово: ");
            var kw = ReadLine();
            foreach (var s in group.Students.Where(st => st.ContainsKeyword(kw)))
            {
                Console.WriteLine(s.GetFormattedInfo());
            }
            break;
    }
}

static void ShowStatistics(StudentGroup group)
{
    var sb = new StringBuilder();
    sb.AppendLine("--- Статистика групи ---");
    sb.Append("Студентів: ").AppendLine(group.GroupSize.ToString());
    sb.Append("Середній бал: ").AppendLine(group.AverageGroupGrade.ToString("F2"));
    sb.AppendLine("\nВідмінники:");
    foreach (var s in group.GetExcellentStudents())
        sb.Append("  • ").AppendLine(s.FullName);
    sb.AppendLine("\nБоржники (<60):");
    foreach (var s in group.GetFailingStudents())
        sb.Append("  • ").AppendLine(s.FullName);
    Console.Write(sb.ToString());
}

static void ExportImportMenu(StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Експорт CSV  2. Імпорт з тексту");
    Console.Write("Вибір: ");
    if (Console.ReadLine() == "1")
    {
        var csv = group.ExportToCsv();
        File.WriteAllText("students.csv", csv, Encoding.UTF8);
        Console.WriteLine("Збережено у students.csv");
        Console.WriteLine(csv);
        logger.Log("INFO", "Експорт CSV виконано.");
    }
    else
    {
        Console.WriteLine("Введіть рядки (ПІБ;залікова;email;дд.ММ.рррр), порожній рядок — завершити:");
        var sb = new StringBuilder();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) break;
            sb.AppendLine(line);
        }
        group.ImportStudentsFromText(sb.ToString());
        logger.Log("INFO", "Імпорт студентів з тексту виконано.");
        Console.WriteLine("Імпорт завершено.");
    }
}

static void GenerateReport(StudentGroup group, AdvancedLogger logger)
{
    var report = TextProcessor.BuildGroupReport(group);
    Console.WriteLine(report);
    logger.Log("INFO", "Згенеровано звіт групи.");
}

static void NormalizeNotes(StudentGroup group, AdvancedLogger logger)
{
    var count = group.NormalizeAllNotes();
    Console.WriteLine($"Нормалізовано нотаток: {count}");
    logger.Log("INFO", $"Нормалізовано {count} нотаток.");
}

static void CheckPalindromes(StudentGroup group)
{
    var sb = new StringBuilder();
    sb.AppendLine("--- Перевірка паліндромів у нотатках ---");
    foreach (var s in group.Students)
    {
        if (string.IsNullOrWhiteSpace(s.Notes)) continue;
        var isPal = TextProcessor.IsPalindrome(s.Notes);
        sb.Append(s.FullName).Append(": ").AppendLine(isPal ? "ПАЛІНДРОМ" : "не паліндром");
    }
    Console.Write(sb.ToString());
}

static void GenerateSqlMenu(AdvancedLogger logger)
{
    Console.WriteLine("Приклади:");
    Console.WriteLine("  вибрати всіх студентів де середній бал більше 90");
    Console.WriteLine("  додати студента Петренко Петро Петрович");
    Console.Write("Ваш опис: ");
    var sql = TextProcessor.GenerateSqlQuery(ReadLine());
    Console.WriteLine("\nЗгенерований SQL:");
    Console.WriteLine(sql);
    logger.Log("INFO", "Згенеровано SQL-запит.");
}

static void ViewLogs(AdvancedLogger logger)
{
    Console.WriteLine("1. Весь лог  2. За рівнем  3. Останні N  4. Зберегти у файл");
    Console.Write("Вибір: ");
    switch (Console.ReadLine())
    {
        case "1": Console.WriteLine(logger.GetFullLog()); break;
        case "2":
            Console.Write("Рівень (INFO/ERROR): ");
            Console.WriteLine(logger.GetLogsByLevel(ReadLine()));
            break;
        case "3":
            Console.Write("Кількість: ");
            Console.WriteLine(logger.GetLast(int.Parse(ReadLine())));
            break;
        case "4":
            logger.SaveToFile("system_log.txt");
            Console.WriteLine("Збережено у system_log.txt");
            break;
    }
}

static void RunBenchmark(AdvancedLogger logger)
{
    Console.Write("Кількість ітерацій (наприклад, 10000): ");
    var n = int.Parse(ReadLine());
    var result = TextProcessor.ComparePerformance(n);
    Console.WriteLine(result);
    logger.Log("INFO", $"Бенчмарк виконано ({n} ітерацій).");
}

static void SaveOrLoad(ref StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Зберегти  2. Завантажити");
    Console.Write("Вибір: ");
    if (Console.ReadLine() == "1")
    {
        Console.Write("Файл: ");
        group.SaveToFile(ReadLine());
        logger.Log("INFO", "Дані збережено.");
        Console.WriteLine("Збережено.");
    }
    else
    {
        Console.Write("Файл: ");
        group = StudentGroup.LoadFromFile(ReadLine());
        logger.Log("INFO", "Дані завантажено.");
        Console.WriteLine($"Завантажено. Студентів: {group.GroupSize}");
    }
}

static string ReadLine()
{
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line)) throw new ArgumentException("Поле не може бути порожнім.");
    return line.Trim();
}

static DateTime ReadDate()
{
    if (!DateTime.TryParseExact(ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var d))
        throw new FormatException("Формат: дд.ММ.рррр");
    return d;
}

static StudentStatus ReadStatus() => ReadLine() switch
{
    "1" => StudentStatus.Active,
    "2" => StudentStatus.AcademicLeave,
    "3" => StudentStatus.Expelled,
    "4" => StudentStatus.Graduated,
    _ => throw new ArgumentException("Невірний статус.")
};
