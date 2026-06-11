using System.Text;
using StudentGroupApp;

var group = CreateTestGroup();
var group2 = CreateSecondGroup();
var events = CreateTestEvents();
var logger = new AdvancedLogger();
logger.Log("INFO", "СГМС (накопичувальна ПР №5) запущено.");

while (true)
{
    PrintMegaMenu(group);
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1": AddBasicStudent(group, logger); break;
            case "2": RemoveMemberMenu(group, logger); break;
            case "3": PrintMembersWithPagination(group); break;
            case "4": SearchStudentByRecordBook(group); break;
            case "5": SaveOrLoadJson(ref group, logger); break;
            case "6": AddLabGradeMenu(group); break;
            case "7": InitializePortMatrixMenu(group); break;
            case "8": AssignAndSimulatePort(group); break;
            case "9": ShowPortLog(group); break;
            case "10": GenerateSqlMenu(logger); break;
            case "11": NormalizeNotes(group, logger); break;
            case "12": CheckPalindromes(group); break;
            case "13": ExportCsv(group, logger); break;
            case "14": RunBenchmark(logger); break;
            case "15": CompareStudentsMenu(group); break;
            case "16": ShowBestStudent(group); break;
            case "17": MergeGroupsMenu(group, ref group2); break;
            case "18": DemoVector(); break;
            case "19": DemoGradePointAndExam(); break;
            case "20": AddSpecialStudent(group, logger); break;
            case "21": ShowTotalScholarship(group); break;
            case "22": FilterByTypeMenu(group); break;
            case "23": ConductEventsMenu(events); break;
            case "24": TestHierarchyEnroll(group); break;
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

static void PrintMegaMenu(StudentGroup group)
{
    var sb = new StringBuilder();
    sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
    sb.AppendLine("║   СГМС (ЗПФК) — Накопичувальна Практична робота №5       ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.AppendLine("║  --- Базові операції (ПР №1) ---                         ║");
    sb.AppendLine("║   1. Додати студента (базового)                          ║");
    sb.AppendLine("║   2. Видалити особу (студента/викладача)                 ║");
    sb.AppendLine("║   3. Вивести весь список (з пагінацією)                  ║");
    sb.AppendLine("║   4. Пошук студента за номером залікової                 ║");
    sb.AppendLine("║   5. Зберегти / Завантажити дані (JSON)                  ║");
    sb.AppendLine("║  --- Масиви та Порти (ПР №2) ---                         ║");
    sb.AppendLine("║   6. Додати оцінку за лабораторну роботу                 ║");
    sb.AppendLine("║   7. Ініціалізувати матрицю портів 16×16                 ║");
    sb.AppendLine("║   8. Прив'язати студента до порту та симулювати роботу   ║");
    sb.AppendLine("║   9. Переглянути лог портів                              ║");
    sb.AppendLine("║  --- Робота з текстом (ПР №3) ---                        ║");
    sb.AppendLine("║  10. Генерація SQL-запиту з тексту (Варіант 5)           ║");
    sb.AppendLine("║  11. Нормалізувати нотатки всіх студентів                ║");
    sb.AppendLine("║  12. Перевірити паліндроми в нотатках                    ║");
    sb.AppendLine("║  13. Експорт групи у CSV                                 ║");
    sb.AppendLine("║  14. Порівняти продуктивність string vs StringBuilder    ║");
    sb.AppendLine("║  --- Перевантаження операторів (ПР №4) ---               ║");
    sb.AppendLine("║  15. Порівняти двох студентів (>, <, ==)                 ║");
    sb.AppendLine("║  16. Знайти найкращого студента в групі                  ║");
    sb.AppendLine("║  17. Об'єднати дві групи (оператор +)                    ║");
    sb.AppendLine("║  18. Робота з векторами (Vector)                         ║");
    sb.AppendLine("║  19. Робота з GradePoint та ExamResult (Варіант 10)      ║");
    sb.AppendLine("║  --- Наслідування та Поліморфізм (ПР №5) ---             ║");
    sb.AppendLine("║  20. Додати специфічного студента                        ║");
    sb.AppendLine("║  21. Розрахувати стипендію для всіх (поліморфізм)        ║");
    sb.AppendLine("║  22. Показати учасників за типом (GetMembersByType)      ║");
    sb.AppendLine("║  23. Організувати студентські заходи (Варіант 10)        ║");
    sb.AppendLine("║  24. Тестування ієрархії та викликів base/override       ║");
    sb.AppendLine("║   0. Вийти                                               ║");
    sb.AppendLine("╠══════════════════════════════════════════════════════════╣");
    sb.Append($"║  Група: {group.GroupName,-47}║").AppendLine();
    sb.Append($"║  Учасників: {group.GroupSize,-43}║").AppendLine();
    sb.Append($"║  Середній бал: {group.AverageGroupGrade,-40:F2}║").AppendLine();
    sb.Append($"║  Порти: {(group.PortMatrix?.IsInitialized == true ? "ініціалізовано" : "не ініціалізовано"),-43}║").AppendLine();
    sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
    Console.Write(sb.ToString());
}

static StudentGroup CreateTestGroup()
{
    var g = new StudentGroup
    {
        GroupName = "КН-21",
        Specialty = "Комп'ютерні науки",
        Course = 2
    };

    var regular = new Student(
        "Коваленко Андрій Іванович",
        new DateTime(2005, 4, 12),
        "andriy.kovalenko@student.zpfk.edu.ua",
        "24010001",
        "  Староста   групи  ")
    {
        CourseProgress = 75
    };
    regular.Journal.AddOrUpdateGrade("Програмування", 82);
    regular.GradePoints.Add(new GradePoint(8));
    regular.GradePoints.Add(new GradePoint(8.5));
    regular.AddLabGrade(1, 90);
    regular.AddLabGrade(2, 85);

    var excellent = new ExcellentStudent(
        "Бондаренко Марія Сергіївна",
        new DateTime(2005, 8, 17),
        "maria.bondarenko@student.zpfk.edu.ua",
        "24010002",
        "Відмінниця курсу")
    {
        CourseProgress = 95
    };
    excellent.Journal.AddOrUpdateGrade("Бази даних", 96);
    excellent.GradePoints.Add(new GradePoint(9.5));
    excellent.AddLabGrade(1, 94);

    var foreign = new ForeignStudent(
        "Schmidt Hans Peter",
        new DateTime(2004, 3, 9),
        "hans.schmidt@student.zpfk.edu.ua",
        "24010003",
        "Німеччина",
        "Обмінний студент")
    {
        CourseProgress = 80
    };
    foreign.Journal.AddOrUpdateGrade("Англійська мова", 88);
    foreign.GradePoints.Add(new GradePoint(8.8));
    foreign.AddLabGrade(1, 80);

    var working = new WorkingStudent(
        "Шевченко Олена Петрівна",
        new DateTime(2006, 1, 25),
        "olena.shevchenko@student.zpfk.edu.ua",
        "24010004",
        "IT Solutions Nova",
        "А роза упала на лапу Азора")
    {
        CourseProgress = 65
    };
    working.Journal.AddOrUpdateGrade("Веб-технології", 75);
    working.AddLabGrade(1, 72);

    g.AddMember(regular);
    g.AddMember(excellent);
    g.AddMember(foreign);
    g.AddMember(working);

    g.InitializePortMatrix();
    g.AssignStudentToPort(regular, 0, 0);
    g.AssignStudentToPort(excellent, 0, 1);
    g.AssignStudentToPort(foreign, 1, 0);

    return g;
}

static StudentGroup CreateSecondGroup()
{
    var g = new StudentGroup { GroupName = "КН-22", Specialty = "Комп'ютерні науки", Course = 2 };

    var s = new Student(
        "Мельник Тарас Васильович",
        new DateTime(2004, 11, 3),
        "taras.melnyk@student.zpfk.edu.ua",
        "24020001",
        "Друга група")
    {
        CourseProgress = 55
    };
    s.Journal.AddOrUpdateGrade("Математика", 65);
    s.GradePoints.Add(new GradePoint(6.5));
    g.AddMember(s);
    return g;
}

static List<UniversityEvent> CreateTestEvents() =>
[
    new Hackathon
    {
        Title = "ZPFK CodeFest 2026",
        Date = new DateTime(2026, 3, 15),
        TechnologyStack = "C# / .NET 8, React, PostgreSQL"
    },
    new EsportsTournament
    {
        Title = "ZPFK Cyber Cup Spring 2026",
        Date = new DateTime(2026, 4, 20),
        GameTitle = "Counter-Strike 2",
        PrizePool = 15000m
    },
    new ScienceConference
    {
        Title = "Студентська наукова конференція ЗПФК",
        Date = new DateTime(2026, 5, 10),
        Topic = "Штучний інтелект у освіті"
    }
];

static void AddBasicStudent(StudentGroup group, AdvancedLogger logger)
{
    Console.Write("ПІБ: ");
    var name = ReadLine();
    Console.Write("Залікова (8 цифр): ");
    var rb = ReadLine();
    Console.Write("Email: ");
    var email = ReadLine();
    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dob = ReadDate();
    Console.Write("Прогрес курсу (0-100): ");
    var progress = int.Parse(ReadLine());

    var student = new Student(name, dob, email, rb)
    {
        CourseProgress = progress
    };

    group.AddMember(student);
    logger.Log("INFO", $"Додано базового студента: {name}");
    Console.WriteLine("Студента додано.");
}

static void RemoveMemberMenu(StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. За номером залікової  2. За номером у списку");
    Console.Write("Вибір: ");
    if (Console.ReadLine() == "1")
    {
        Console.Write("Залікова: ");
        if (group.RemoveStudent(ReadLine()))
        {
            logger.Log("INFO", "Учасника видалено за заліковою.");
            Console.WriteLine("Видалено.");
        }
        else
        {
            Console.WriteLine("Не знайдено.");
        }
    }
    else
    {
        PrintMembersWithPagination(group, pause: false);
        Console.Write("Номер у списку: ");
        var index = int.Parse(ReadLine()) - 1;
        if (index >= 0 && index < group.Members.Count)
        {
            var member = group.Members[index];
            group.RemoveMember(member);
            logger.Log("INFO", $"Видалено учасника: {member.FullName}");
            Console.WriteLine("Видалено.");
        }
        else
        {
            Console.WriteLine("Невірний номер.");
        }
    }
}

static void PrintMembersWithPagination(StudentGroup group, bool pause = true)
{
    const int pageSize = 2;
    var members = group.Members;
    if (members.Count == 0)
    {
        Console.WriteLine("Список порожній.");
        return;
    }

    var totalPages = (int)Math.Ceiling(members.Count / (double)pageSize);
    for (var page = 0; page < totalPages; page++)
    {
        Console.WriteLine($"--- Сторінка {page + 1}/{totalPages} ---");
        var slice = members.Skip(page * pageSize).Take(pageSize);
        foreach (var member in slice)
        {
            Console.WriteLine($"[{member.GetType().Name}] {member.FullName}");
            if (member is Student student)
            {
                Console.WriteLine(student.GetFormattedInfo(detailed: true));
            }
            else
            {
                Console.WriteLine(member.GetInfo());
            }

            Console.WriteLine(new string('-', 40));
        }

        if (pause && page < totalPages - 1)
        {
            Console.WriteLine("Натисніть Enter для наступної сторінки...");
            Console.ReadLine();
        }
    }
}

static void SearchStudentByRecordBook(StudentGroup group)
{
    Console.Write("Номер залікової: ");
    var student = group.FindStudent(ReadLine());
    if (student is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    Console.WriteLine(student.GetFormattedInfo(detailed: true));
}

static void SaveOrLoadJson(ref StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Зберегти  2. Завантажити");
    Console.Write("Вибір: ");
    if (Console.ReadLine() == "1")
    {
        Console.Write("Файл (наприклад, group_data.json): ");
        var path = ReadLine();
        group.SaveToFile(path);
        logger.Log("INFO", $"Дані збережено у {path}");
        Console.WriteLine("Збережено.");
    }
    else
    {
        Console.Write("Файл: ");
        group = StudentGroup.LoadFromFile(ReadLine());
        logger.Log("INFO", "Дані завантажено з JSON.");
        Console.WriteLine($"Завантажено. Учасників: {group.GroupSize}");
    }
}

static void AddLabGradeMenu(StudentGroup group)
{
    Console.Write("Залікова студента: ");
    var rb = ReadLine();
    if (group.FindStudent(rb) is not Student student)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    Console.Write("Номер лабораторної (1-10): ");
    var lab = int.Parse(ReadLine());
    Console.Write("Оцінка (0-100): ");
    var grade = byte.Parse(ReadLine());

    student.AddLabGrade(lab, grade);
    Console.WriteLine($"Оцінку {grade} за лаб. #{lab} додано для {student.FullName}.");
    Console.WriteLine($"Середній бал лаб: {student.GetAverageLabGrade():F2}");
}

static void InitializePortMatrixMenu(StudentGroup group)
{
    group.InitializePortMatrix();
    Console.WriteLine("Матрицю портів 16×16 ініціалізовано.");
}

static void AssignAndSimulatePort(StudentGroup group)
{
    Console.WriteLine("1. Прив'язати студента  2. Симулювати лабораторну роботу");
    Console.Write("Вибір: ");
    if (Console.ReadLine() == "1")
    {
        Console.Write("Залікова: ");
        var student = group.FindStudent(ReadLine());
        if (student is null)
        {
            Console.WriteLine("Студента не знайдено.");
            return;
        }

        Console.Write("Рядок (0-15): ");
        var row = int.Parse(ReadLine());
        Console.Write("Стовпець (0-15): ");
        var col = int.Parse(ReadLine());
        group.AssignStudentToPort(student, row, col);
        Console.WriteLine($"Студента {student.FullName} прив'язано до порту [{row},{col}].");
    }
    else
    {
        Console.Write("Залікова: ");
        var rb = ReadLine();
        Console.Write("Номер лаб. (1-10): ");
        var lab = int.Parse(ReadLine());
        Console.Write("Оцінка: ");
        var grade = byte.Parse(ReadLine());
        group.SimulateLabWork(rb, lab, grade);
        Console.WriteLine("Лабораторну роботу симульовано, дані записано в порт.");
    }
}

static void ShowPortLog(StudentGroup group)
{
    Console.WriteLine(group.PortLogger.GetFullLog());
    if (group.PortMatrix?.IsInitialized == true)
    {
        Console.WriteLine();
        Console.WriteLine(group.PortMatrix.GetFormattedMatrixState());
    }
}

static void GenerateSqlMenu(AdvancedLogger logger)
{
    Console.WriteLine("Приклад: вибрати всіх студентів де середній бал більше 90");
    Console.Write("Ваш опис: ");
    var sql = TextProcessor.GenerateSqlQuery(ReadLine());
    Console.WriteLine("\nЗгенерований SQL:");
    Console.WriteLine(sql);
    logger.Log("INFO", "Згенеровано SQL-запит.");
}

static void NormalizeNotes(StudentGroup group, AdvancedLogger logger)
{
    var count = group.NormalizeAllNotes();
    Console.WriteLine($"Нормалізовано нотаток: {count}");
    logger.Log("INFO", $"Нормалізовано {count} нотаток.");
}

static void CheckPalindromes(StudentGroup group)
{
    Console.WriteLine("--- Перевірка паліндромів у нотатках ---");
    foreach (var member in group.Members)
    {
        if (string.IsNullOrWhiteSpace(member.Notes))
        {
            continue;
        }

        var isPal = TextProcessor.IsPalindrome(member.Notes);
        Console.WriteLine($"{member.FullName}: {(isPal ? "ПАЛІНДРОМ" : "не паліндром")}");
    }
}

static void ExportCsv(StudentGroup group, AdvancedLogger logger)
{
    var csv = group.ExportToCsv();
    File.WriteAllText("students.csv", csv, Encoding.UTF8);
    Console.WriteLine("Збережено у students.csv:");
    Console.WriteLine(csv);
    logger.Log("INFO", "Експорт CSV виконано.");
}

static void RunBenchmark(AdvancedLogger logger)
{
    Console.Write("Кількість ітерацій (наприклад, 10000): ");
    var n = int.Parse(ReadLine());
    Console.WriteLine(TextProcessor.ComparePerformance(n));
    logger.Log("INFO", $"Бенчмарк виконано ({n} ітерацій).");
}

static void CompareStudentsMenu(StudentGroup group)
{
    Console.Write("Залікова 1: ");
    var s1 = group.FindStudent(ReadLine());
    Console.Write("Залікова 2: ");
    var s2 = group.FindStudent(ReadLine());

    if (s1 is null || s2 is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    Console.WriteLine($"{s1.FullName}: бал {s1.AverageGrade:F2}, прогрес {s1.CourseProgress}");
    Console.WriteLine($"{s2.FullName}: бал {s2.AverageGrade:F2}, прогрес {s2.CourseProgress}");
    Console.WriteLine($"s1 > s2:  {s1 > s2}");
    Console.WriteLine($"s1 < s2:  {s1 < s2}");
    Console.WriteLine($"s1 == s2: {s1 == s2}");
    Console.WriteLine($"s1 >= s2: {s1 >= s2}");
    Console.WriteLine($"s1 <= s2: {s1 <= s2}");
}

static void ShowBestStudent(StudentGroup group)
{
    var best = group.BestStudent();
    if (best is null)
    {
        Console.WriteLine("Студентів у групі немає.");
        return;
    }

    Console.WriteLine("Найкращий студент:");
    Console.WriteLine(best.GetFormattedInfo(detailed: true));
}

static void MergeGroupsMenu(StudentGroup group, ref StudentGroup group2)
{
    Console.WriteLine($"Група 1: {group.GroupName} ({group.StudentCount} студ.)");
    Console.WriteLine($"Група 2: {group2.GroupName} ({group2.StudentCount} студ.)");
    var merged = group.MergeGroups(group2);
    Console.WriteLine($"Об'єднана група: {merged.GroupName} ({merged.StudentCount} студ.)");
    foreach (var member in merged.Members)
    {
        if (member is Student s)
        {
            Console.WriteLine($"  • {s.FullName} | бал: {s.AverageGrade:F2}");
        }
    }
}

static void DemoVector()
{
    var v1 = new Vector(1, 2, 3);
    var v2 = new Vector(4, 5, 6);
    Console.WriteLine($"v1 = {v1}, довжина = {(double)v1:F2}");
    Console.WriteLine($"v2 = {v2}, довжина = {(double)v2:F2}");
    Console.WriteLine($"v1 + v2 = {v1 + v2}");
    Console.WriteLine($"v1 - v2 = {v1 - v2}");
    Console.WriteLine($"v1 * 2 = {v1 * 2}");
    Console.WriteLine($"v1 > v2: {v1 > v2}");
    Console.WriteLine($"++v1 = {++v1}");
}

static void DemoGradePointAndExam()
{
    GradePoint g1 = 7.5;
    GradePoint g2 = 8.5;
    Console.WriteLine($"g1 = {g1}, g2 = {g2}");
    Console.WriteLine($"g1 + g2 = {g1 + g2}");
    Console.WriteLine($"g2 > g1: {g2 > g1}");
    Console.WriteLine($"g2 >= 8: {(g2 ? "так" : "ні")}");

    var e1 = new ExamResult { Subject = "Програмування", Score = 45, MaxScore = 50 };
    var e2 = new ExamResult { Subject = "Математика", Score = 30, MaxScore = 50 };
    Console.WriteLine($"e1: {e1}");
    Console.WriteLine($"e2: {e2}");
    Console.WriteLine($"e1 + e2: {e1 + e2}");
    Console.WriteLine($"e1 успішний: {(e1 ? "так" : "ні")}");
    Console.WriteLine($"Відсоток e1: {(double)e1:F1}%");
}

static void AddSpecialStudent(StudentGroup group, AdvancedLogger logger)
{
    Console.WriteLine("1. Відмінник  2. Іноземний  3. Працюючий  4. Випускник");
    Console.Write("Тип: ");
    var type = Console.ReadLine();

    Console.Write("ПІБ: ");
    var name = ReadLine();
    Console.Write("Залікова: ");
    var rb = ReadLine();
    Console.Write("Email: ");
    var email = ReadLine();
    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dob = ReadDate();
    Console.Write("Прогрес курсу: ");
    var progress = int.Parse(ReadLine());

    Student member = type switch
    {
        "1" => new ExcellentStudent(name, dob, email, rb),
        "2" => CreateForeign(name, dob, email, rb, progress),
        "3" => CreateWorking(name, dob, email, rb, progress),
        "4" => CreateGraduate(name, dob, email, rb, progress),
        _ => throw new ArgumentException("Невірний тип.")
    };

    if (type == "1")
    {
        member.CourseProgress = progress;
    }

    group.AddMember(member);
    logger.Log("INFO", $"Додано {member.GetType().Name}: {name}");
    Console.WriteLine("Студента додано.");
}

static ForeignStudent CreateForeign(string name, DateTime dob, string email, string rb, int progress)
{
    Console.Write("Країна: ");
    return new ForeignStudent(name, dob, email, rb, ReadLine()) { CourseProgress = progress };
}

static WorkingStudent CreateWorking(string name, DateTime dob, string email, string rb, int progress)
{
    Console.Write("Компанія: ");
    return new WorkingStudent(name, dob, email, rb, ReadLine()) { CourseProgress = progress };
}

static GraduateStudent CreateGraduate(string name, DateTime dob, string email, string rb, int progress)
{
    Console.Write("Тема диплома: ");
    return new GraduateStudent(name, dob, email, rb, ReadLine()) { CourseProgress = progress };
}

static void ShowTotalScholarship(StudentGroup group)
{
    Console.WriteLine($"Загальна стипендія групи «{group.GroupName}»: {group.GetTotalScholarship():F2} грн");
    foreach (var member in group.Members)
    {
        Console.WriteLine($"  • [{member.GetType().Name}] {member.FullName}: {member.CalculateScholarship():F2} грн");
    }
}

static void FilterByTypeMenu(StudentGroup group)
{
    Console.WriteLine("1. Student  2. ExcellentStudent  3. ForeignStudent");
    Console.WriteLine("4. WorkingStudent  5. GraduateStudent");
    Console.Write("Тип: ");
    switch (Console.ReadLine())
    {
        case "1": PrintTypeResult(group.GetMembersByType<Student>(), "Student"); break;
        case "2": PrintTypeResult(group.GetMembersByType<ExcellentStudent>(), "ExcellentStudent"); break;
        case "3": PrintTypeResult(group.GetMembersByType<ForeignStudent>(), "ForeignStudent"); break;
        case "4": PrintTypeResult(group.GetMembersByType<WorkingStudent>(), "WorkingStudent"); break;
        case "5": PrintTypeResult(group.GetMembersByType<GraduateStudent>(), "GraduateStudent"); break;
        default: Console.WriteLine("Невірний тип."); break;
    }
}

static void PrintTypeResult<T>(List<T> members, string typeName) where T : UniversityMember
{
    Console.WriteLine($"Тип {typeName}: знайдено {members.Count}");
    foreach (var m in members)
    {
        Console.WriteLine($"  • {m.FullName} | стипендія: {m.CalculateScholarship():F2} грн");
    }
}

static void ConductEventsMenu(List<UniversityEvent> events)
{
    for (var i = 0; i < events.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {events[i].Title}");
    }

    Console.WriteLine("  0. Провести всі заходи");
    Console.Write("Вибір: ");
    var input = Console.ReadLine();
    if (input == "0")
    {
        foreach (var ev in events)
        {
            ev.ConductEvent();
            Console.WriteLine();
        }

        return;
    }

    if (int.TryParse(input, out var idx) && idx >= 1 && idx <= events.Count)
    {
        events[idx - 1].ConductEvent();
    }
    else
    {
        Console.WriteLine("Невірний вибір.");
    }
}

static void TestHierarchyEnroll(StudentGroup group)
{
    Console.WriteLine("--- Демонстрація base/override та Enroll() ---");
    foreach (var member in group.Members)
    {
        Console.WriteLine($"\nТип: {member.GetType().Name}, ПІБ: {member.FullName}");
        Console.WriteLine("Виклик virtual Enroll() → override у Student викликає base.Enroll():");
        member.Enroll();
        Console.WriteLine($"[{member.GetType().Name}] {member.FullName} зарахований до ЗПФК (дата: {member.EnrollmentDate:dd.MM.yyyy}).");
        Console.WriteLine("GetInfo() (поліморфний виклик override):");
        Console.WriteLine(member.GetInfo());
        Console.WriteLine(new string('-', 40));
    }
}

static string ReadLine()
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
    if (!DateTime.TryParseExact(ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var d))
    {
        throw new FormatException("Формат: дд.ММ.рррр");
    }

    return d;
}
