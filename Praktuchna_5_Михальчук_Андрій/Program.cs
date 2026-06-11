using StudentGroupApp;

var group = CreateTestGroup();
var events = CreateTestEvents();

while (true)
{
    PrintMenu(group);
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1": AddRegularStudent(group); break;
            case "2": AddSpecialStudent(group); break;
            case "3": group.PrintAllMembers(); break;
            case "4": ShowTotalScholarship(group); break;
            case "5": FilterByType(group); break;
            case "6": ConductEvents(events); break;
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
    Console.WriteLine("║   СГМС (ЗПФК) — Практична робота №5: Наслідування       ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine("║  1. Додати звичайного студента                           ║");
    Console.WriteLine("║  2. Додати відмінника / іноземного / працюючого /        ║");
    Console.WriteLine("║     випускника                                           ║");
    Console.WriteLine("║  3. Вивести всіх членів університету (поліморфізм)       ║");
    Console.WriteLine("║  4. Розрахувати загальну суму стипендій                  ║");
    Console.WriteLine("║  5. Відфільтрувати учасників за типом                    ║");
    Console.WriteLine("║  6. Організувати студентські заходи (Варіант 10)         ║");
    Console.WriteLine("║  0. Вийти                                                ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine($"║  Група: {group.GroupName,-47}║");
    Console.WriteLine($"║  Учасників: {group.MemberCount,-43}║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
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
        "Звичайний студент групи")
    {
        CourseProgress = 70
    };
    regular.Journal.AddOrUpdateGrade("Програмування", 82);
    regular.GradePoints.Add(new GradePoint(8));

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

    var working = new WorkingStudent(
        "Шевченко Олена Петрівна",
        new DateTime(2006, 1, 25),
        "olena.shevchenko@student.zpfk.edu.ua",
        "24010004",
        "IT Solutions Nova",
        "Поєднує навчання з роботою")
    {
        CourseProgress = 65
    };
    working.Journal.AddOrUpdateGrade("Веб-технології", 75);

    var graduate = new GraduateStudent(
        "Мельник Тарас Васильович",
        new DateTime(2003, 11, 3),
        "taras.melnyk@student.zpfk.edu.ua",
        "24010005",
        "Розробка системи обліку студентів на .NET",
        "Випускник 2025")
    {
        CourseProgress = 100
    };
    graduate.Journal.AddOrUpdateGrade("Дипломний проєкт", 94);

    g.AddMember(regular);
    g.AddMember(excellent);
    g.AddMember(foreign);
    g.AddMember(working);
    g.AddMember(graduate);

    return g;
}

static List<UniversityEvent> CreateTestEvents()
{
    return
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
}

static void AddRegularStudent(StudentGroup group)
{
    Console.Write("ПІБ: ");
    var name = ReadLine();
    Console.Write("Залікова (8 цифр): ");
    var recordBook = ReadLine();
    Console.Write("Email: ");
    var email = ReadLine();
    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dob = ReadDate();
    Console.Write("Прогрес курсу (0-100): ");
    var progress = int.Parse(ReadLine());

    var student = new Student(name, dob, email, recordBook)
    {
        CourseProgress = progress
    };

    group.AddMember(student);
    Console.WriteLine("Звичайного студента додано до групи.");
}

static void AddSpecialStudent(StudentGroup group)
{
    Console.WriteLine("1. Відмінник  2. Іноземний  3. Працюючий  4. Випускник");
    Console.Write("Тип: ");
    var type = Console.ReadLine();

    Console.Write("ПІБ: ");
    var name = ReadLine();
    Console.Write("Залікова (8 цифр): ");
    var recordBook = ReadLine();
    Console.Write("Email: ");
    var email = ReadLine();
    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dob = ReadDate();
    Console.Write("Прогрес курсу (0-100): ");
    var progress = int.Parse(ReadLine());

    UniversityMember member = type switch
    {
        "1" => new ExcellentStudent(name, dob, email, recordBook),
        "2" => CreateForeignStudent(name, dob, email, recordBook, progress),
        "3" => CreateWorkingStudent(name, dob, email, recordBook, progress),
        "4" => CreateGraduateStudent(name, dob, email, recordBook, progress),
        _ => throw new ArgumentException("Невірний тип студента.")
    };

    if (member is Student s && type is "1" or "2" or "3" or "4")
    {
        s.CourseProgress = progress;
    }

    group.AddMember(member);
    Console.WriteLine("Студента додано до групи.");
}

static ForeignStudent CreateForeignStudent(string name, DateTime dob, string email, string recordBook, int progress)
{
    Console.Write("Країна походження: ");
    var country = ReadLine();
    var student = new ForeignStudent(name, dob, email, recordBook, country)
    {
        CourseProgress = progress
    };
    return student;
}

static WorkingStudent CreateWorkingStudent(string name, DateTime dob, string email, string recordBook, int progress)
{
    Console.Write("Назва компанії: ");
    var company = ReadLine();
    var student = new WorkingStudent(name, dob, email, recordBook, company)
    {
        CourseProgress = progress
    };
    return student;
}

static GraduateStudent CreateGraduateStudent(string name, DateTime dob, string email, string recordBook, int progress)
{
    Console.Write("Тема дипломної роботи: ");
    var thesis = ReadLine();
    var student = new GraduateStudent(name, dob, email, recordBook, thesis)
    {
        CourseProgress = progress
    };
    return student;
}

static void ShowTotalScholarship(StudentGroup group)
{
    var total = group.GetTotalScholarship();
    Console.WriteLine($"Загальна сума стипендій для групи «{group.GroupName}»: {total:F2} грн");
    Console.WriteLine();
    foreach (var member in group.Members)
    {
        Console.WriteLine($"  • {member.FullName}: {member.CalculateScholarship():F2} грн");
    }
}

static void FilterByType(StudentGroup group)
{
    Console.WriteLine("1. Student  2. ExcellentStudent  3. ForeignStudent");
    Console.WriteLine("4. WorkingStudent  5. GraduateStudent");
    Console.Write("Тип: ");
    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            PrintFiltered(group.GetMembersByType<Student>(), "Student");
            break;
        case "2":
            PrintFiltered(group.GetMembersByType<ExcellentStudent>(), "ExcellentStudent");
            break;
        case "3":
            PrintFiltered(group.GetMembersByType<ForeignStudent>(), "ForeignStudent");
            break;
        case "4":
            PrintFiltered(group.GetMembersByType<WorkingStudent>(), "WorkingStudent");
            break;
        case "5":
            PrintFiltered(group.GetMembersByType<GraduateStudent>(), "GraduateStudent");
            break;
        default:
            Console.WriteLine("Невірний тип.");
            break;
    }
}

static void PrintFiltered<T>(List<T> members, string typeName) where T : UniversityMember
{
    Console.WriteLine($"Знайдено {members.Count} учасників типу {typeName}:");
    foreach (var member in members)
    {
        Console.WriteLine($"  • {member.FullName} | стипендія: {member.CalculateScholarship():F2} грн");
    }
}

static void ConductEvents(List<UniversityEvent> events)
{
    Console.WriteLine("Оберіть захід:");
    for (var i = 0; i < events.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {events[i].Title} ({events[i].Date:dd.MM.yyyy})");
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

    if (int.TryParse(input, out var index) && index >= 1 && index <= events.Count)
    {
        events[index - 1].ConductEvent();
    }
    else
    {
        Console.WriteLine("Невірний вибір.");
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
