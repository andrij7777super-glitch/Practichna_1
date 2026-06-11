using StudentGroupApp;

var group = CreateTestGroup();
var group2 = CreateSecondGroup();

while (true)
{
    PrintMenu(group);
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1": ManageStudents(group); break;
            case "2": CompareStudents(group); break;
            case "3": MergeGroupsMenu(group, ref group2); break;
            case "4": IndexerSearch(group); break;
            case "5": DemoVector(); break;
            case "6": DemoGradePoint(); break;
            case "7": DemoExamResult(); break;
            case "8": ShowBestStudent(group); break;
            case "0":
                Console.WriteLine("До побачення!");
                return;
            default:
                Console.WriteLine("Невірний пункт меню.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Натисніть будь-яку клавішу...");
    Console.ReadKey(true);
    Console.Clear();
}

static void PrintMenu(StudentGroup group)
{
    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║   СГМС (ЗПФК) — Практична робота №4: Оператори          ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine("║  1. Управління студентами (Додати/Видалити/Вивести)     ║");
    Console.WriteLine("║  2. Порівняти двох студентів (>, <, ==)                  ║");
    Console.WriteLine("║  3. Об'єднати дві групи (оператор +)                     ║");
    Console.WriteLine("║  4. Пошук студента за індексатором group[\"номер\"]       ║");
    Console.WriteLine("║  5. Продемонструвати роботу з Vector                     ║");
    Console.WriteLine("║  6. Продемонструвати роботу з GradePoint                 ║");
    Console.WriteLine("║  7. Робота з ExamResult (Варіант 10)                     ║");
    Console.WriteLine("║  8. Знайти найкращого студента в групі                   ║");
    Console.WriteLine("║  0. Вийти                                                ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine($"║  Група: {group.GroupName,-47}║");
    Console.WriteLine($"║  Студентів: {group.GroupSize,-43}║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
}

static StudentGroup CreateTestGroup()
{
    var g = new StudentGroup { GroupName = "КН-21", Specialty = "Комп'ютерні науки", Course = 2 };

    var s1 = new Student
    {
        FullName = "Коваленко Андрій Іванович",
        DateOfBirth = new DateTime(2005, 4, 12),
        RecordBookNumber = "24010001",
        PersonalEmail = "andriy.kovalenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        CourseProgress = 75
    };
    s1.Journal.AddOrUpdateGrade("Програмування", 92);
    s1.GradePoints.Add(new GradePoint(9));
    s1.GradePoints.Add(new GradePoint(8.5));

    var s2 = new Student
    {
        FullName = "Шевченко Олена Петрівна",
        DateOfBirth = new DateTime(2006, 1, 25),
        RecordBookNumber = "24010002",
        PersonalEmail = "olena.shevchenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        CourseProgress = 60
    };
    s2.Journal.AddOrUpdateGrade("Програмування", 78);
    s2.GradePoints.Add(new GradePoint(7));

    var s3 = new Student
    {
        FullName = "Бондаренко Марія Сергіївна",
        DateOfBirth = new DateTime(2005, 8, 17),
        RecordBookNumber = "24010004",
        PersonalEmail = "maria.bondarenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        CourseProgress = 90
    };
    s3.Journal.AddOrUpdateGrade("Бази даних", 96);
    s3.GradePoints.Add(new GradePoint(9.5));

    g.AddStudent(s1);
    g.AddStudent(s2);
    g.AddStudent(s3);
    return g;
}

static StudentGroup CreateSecondGroup()
{
    var g = new StudentGroup { GroupName = "КН-22", Specialty = "Комп'ютерні науки", Course = 2 };

    var s = new Student
    {
        FullName = "Мельник Тарас Васильович",
        DateOfBirth = new DateTime(2004, 11, 3),
        RecordBookNumber = "24020001",
        PersonalEmail = "taras.melnyk@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        CourseProgress = 55
    };
    s.Journal.AddOrUpdateGrade("Математика", 65);
    g.AddStudent(s);
    return g;
}

static void ManageStudents(StudentGroup group)
{
    Console.WriteLine("1. Додати  2. Видалити  3. Вивести всіх");
    Console.Write("Вибір: ");
    switch (Console.ReadLine())
    {
        case "1":
            Console.Write("ПІБ: ");
            var name = ReadLine();
            Console.Write("Залікова: ");
            var rb = ReadLine();
            Console.Write("Email: ");
            var email = ReadLine();
            Console.Write("Дата народження (дд.ММ.рррр): ");
            var dob = ReadDate();
            Console.Write("Прогрес курсу (0-100): ");
            var progress = int.Parse(ReadLine());

            var student = new Student
            {
                FullName = name,
                RecordBookNumber = rb,
                PersonalEmail = email,
                DateOfBirth = dob,
                EnrollmentDate = DateTime.Today,
                CourseProgress = progress
            };
            group.AddStudent(student);
            Console.WriteLine("Студента додано.");
            break;
        case "2":
            Console.Write("Залікова: ");
            Console.WriteLine(group.RemoveStudent(ReadLine()) ? "Видалено." : "Не знайдено.");
            break;
        case "3":
            foreach (var s in group.Students)
            {
                Console.WriteLine(s.GetFormattedInfo(detailed: true));
                Console.WriteLine(new string('-', 40));
            }
            break;
    }
}

static void CompareStudents(StudentGroup group)
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

static void MergeGroupsMenu(StudentGroup group, ref StudentGroup group2)
{
    Console.WriteLine($"Група 1: {group.GroupName} ({group.GroupSize} студ.)");
    Console.WriteLine($"Група 2: {group2.GroupName} ({group2.GroupSize} студ.)");
    var merged = group.MergeGroups(group2);
    Console.WriteLine($"Об'єднана група: {merged.GroupName} ({merged.GroupSize} студ.)");
    foreach (var s in merged.Students)
    {
        Console.WriteLine($"  • {s.FullName} | бал: {s.AverageGrade:F2}");
    }
}

static void IndexerSearch(StudentGroup group)
{
    Console.Write("Номер залікової: ");
    var student = group[ReadLine()];
    if (student is null)
    {
        Console.WriteLine("Студента не знайдено.");
    }
    else
    {
        Console.WriteLine(student.GetFormattedInfo(detailed: true));
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
    Console.WriteLine($"--v1 = {--v1}");
}

static void DemoGradePoint()
{
    GradePoint g1 = 7.5;
    GradePoint g2 = 8.5;

    Console.WriteLine($"g1 = {g1}, g2 = {g2}");
    Console.WriteLine($"g1 + g2 = {g1 + g2}");
    Console.WriteLine($"++g1 = {++g1}");
    Console.WriteLine($"g2 > g1: {g2 > g1}");
    Console.WriteLine($"g1 >= 8 (true/false): {(g1 ? "так" : "ні")}");
    Console.WriteLine($"g2 >= 8 (true/false): {(g2 ? "так" : "ні")}");

    double value = g2;
    Console.WriteLine($"Неявне приведення до double: {value}");
}

static void DemoExamResult()
{
    var e1 = new ExamResult { Subject = "Програмування", Score = 45, MaxScore = 50 };
    var e2 = new ExamResult { Subject = "Математика", Score = 30, MaxScore = 50 };

    Console.WriteLine($"e1: {e1}");
    Console.WriteLine($"e2: {e2}");
    Console.WriteLine($"e1 + e2: {e1 + e2}");
    Console.WriteLine($"e1 успішний (true/false): {(e1 ? "так" : "ні")}");
    Console.WriteLine($"Відсоток e1: {(double)e1:F1}%");
    Console.WriteLine($"Відсоток e2: {(double)e2:F1}%");
}

static void ShowBestStudent(StudentGroup group)
{
    var best = group.BestStudent();
    if (best is null)
    {
        Console.WriteLine("Група порожня.");
        return;
    }

    Console.WriteLine("Найкращий студент групи:");
    Console.WriteLine(best.GetFormattedInfo(detailed: true));
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
