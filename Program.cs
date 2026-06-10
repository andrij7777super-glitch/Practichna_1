using StudentGroupApp;

const int PageSize = 10;
var group = CreateGroupWithTestData();

while (true)
{
    PrintMainMenu();
    Console.Write("Оберіть пункт меню: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1":
                AddStudent(group);
                break;
            case "2":
                RemoveStudent(group);
                break;
            case "3":
                ShowAllStudentsPaginated(group);
                break;
            case "4":
                SearchStudent(group);
                break;
            case "5":
                EditStudent(group);
                break;
            case "6":
                ShowExcellentAndFailingStudents(group);
                break;
            case "7":
                ShowGroupStatistics(group);
                break;
            case "8":
                SaveGroupToFile(group);
                break;
            case "9":
                group = LoadGroupFromFile();
                break;
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

static void PrintMainMenu()
{
    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║   Система управління студентською групою (ЗПФК)          ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine("║  1. Додати студента                                      ║");
    Console.WriteLine("║  2. Видалити студента                                    ║");
    Console.WriteLine("║  3. Вивести всіх студентів (з пагінацією)                ║");
    Console.WriteLine("║  4. Пошук студента (за ПІБ або номером залікової)        ║");
    Console.WriteLine("║  5. Редагування даних студента                           ║");
    Console.WriteLine("║  6. Вивести відмінників / тих, хто має < 60 балів        ║");
    Console.WriteLine("║  7. Вивести статистику групи                             ║");
    Console.WriteLine("║  8. Зберегти дані групи у JSON файл                      ║");
    Console.WriteLine("║  9. Завантажити дані групи з JSON файлу                  ║");
    Console.WriteLine("║  0. Вийти                                                ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
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
        Notes = "Староста групи, активний учасник олімпіад з програмування."
    };
    student1.Journal.AddOrUpdateGrade("Математика", 95);
    student1.Journal.AddOrUpdateGrade("Програмування", 92);
    student1.Journal.AddOrUpdateGrade("Англійська мова", 88);

    var student2 = new Student
    {
        FullName = "Шевченко Олена Петрівна",
        DateOfBirth = new DateTime(2006, 1, 25),
        RecordBookNumber = "24010002",
        PersonalEmail = "olena.shevchenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.Active,
        Notes = "Добре володіє графічним дизайном."
    };
    student2.Journal.AddOrUpdateGrade("Математика", 78);
    student2.Journal.AddOrUpdateGrade("Програмування", 85);
    student2.Journal.AddOrUpdateGrade("Фізика", 80);

    var student3 = new Student
    {
        FullName = "Мельник Тарас Васильович",
        DateOfBirth = new DateTime(2004, 11, 3),
        RecordBookNumber = "24010003",
        PersonalEmail = "taras.melnyk@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.AcademicLeave,
        Notes = "Тимчасово в академічній відпустці з медичних причин."
    };
    student3.Journal.AddOrUpdateGrade("Математика", 55);
    student3.Journal.AddOrUpdateGrade("Програмування", 58);
    student3.Journal.AddOrUpdateGrade("Історія України", 62);

    var student4 = new Student
    {
        FullName = "Бондаренко Марія Сергіївна",
        DateOfBirth = new DateTime(2005, 8, 17),
        RecordBookNumber = "24010004",
        PersonalEmail = "maria.bondarenko@student.zpfk.edu.ua",
        EnrollmentDate = new DateTime(2023, 9, 1),
        Status = StudentStatus.Active,
        Notes = "Працює над курсовим проєктом з веб-розробки."
    };
    student4.Journal.AddOrUpdateGrade("Математика", 91);
    student4.Journal.AddOrUpdateGrade("Програмування", 94);
    student4.Journal.AddOrUpdateGrade("Бази даних", 96);

    studentGroup.AddStudent(student1);
    studentGroup.AddStudent(student2);
    studentGroup.AddStudent(student3);
    studentGroup.AddStudent(student4);

    return studentGroup;
}

static void AddStudent(StudentGroup group)
{
    Console.WriteLine("--- Додавання нового студента ---");

    Console.Write("ПІБ (Прізвище Ім'я По батькові): ");
    var fullName = ReadRequiredLine();

    Console.Write("Дата народження (дд.ММ.рррр): ");
    var dateOfBirth = ReadDate();

    Console.Write("Номер залікової книжки (8 цифр): ");
    var recordBookNumber = ReadRequiredLine();

    Console.Write("Електронна пошта: ");
    var email = ReadRequiredLine();

    Console.Write("Дата зарахування (дд.ММ.рррр): ");
    var enrollmentDate = ReadDate();

    Console.Write("Статус (1-Активний, 2-Академ.відпустка, 3-Відрахований, 4-Випускник): ");
    var status = ReadStudentStatus();

    Console.Write("Нотатки (необов'язково): ");
    var notes = Console.ReadLine() ?? string.Empty;

    var student = new Student
    {
        FullName = fullName,
        DateOfBirth = dateOfBirth,
        RecordBookNumber = recordBookNumber,
        PersonalEmail = email,
        EnrollmentDate = enrollmentDate,
        Status = status,
        Notes = notes
    };

    Console.Write("Додати оцінку за предмет? (т/н): ");
    if (ReadYesNo())
    {
        Console.Write("Назва предмету: ");
        var subject = ReadRequiredLine();
        Console.Write("Оцінка (0-100): ");
        var grade = ReadDoubleInRange(0, 100);
        student.Journal.AddOrUpdateGrade(subject, grade);
    }

    group.AddStudent(student);
    Console.WriteLine($"Студента {student.FullName} успішно додано до групи.");
}

static void RemoveStudent(StudentGroup group)
{
    Console.WriteLine("--- Видалення студента ---");
    Console.Write("Введіть номер залікової книжки: ");
    var recordBookNumber = ReadRequiredLine();

    if (group.RemoveStudent(recordBookNumber))
    {
        Console.WriteLine("Студента успішно видалено з групи.");
    }
    else
    {
        Console.WriteLine("Студента з таким номером залікової не знайдено.");
    }
}

static void ShowAllStudentsPaginated(StudentGroup group)
{
    var students = group.Students;
    if (students.Count == 0)
    {
        Console.WriteLine("У групі немає студентів.");
        return;
    }

    var totalPages = (int)Math.Ceiling(students.Count / (double)PageSize);
    var currentPage = 1;

    while (true)
    {
        Console.WriteLine($"--- Список студентів (сторінка {currentPage} з {totalPages}) ---");
        Console.WriteLine($"Група: {group.GroupName} | Спеціальність: {group.Specialty} | Курс: {group.Course}");
        Console.WriteLine(new string('-', 70));

        var pageStudents = students
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        foreach (var student in pageStudents)
        {
            Console.WriteLine($"{student.RecordBookNumber} | {student.FullName} | Бал: {student.AverageGrade:F2} | {GetStatusName(student.Status)}");
        }

        Console.WriteLine(new string('-', 70));
        Console.WriteLine("Навігація: [N] — наступна, [P] — попередня, [D] — деталі, [Q] — вихід");
        Console.Write("Ваш вибір: ");
        var choice = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();

        switch (choice)
        {
            case "N" when currentPage < totalPages:
                currentPage++;
                break;
            case "P" when currentPage > 1:
                currentPage--;
                break;
            case "D":
                Console.Write("Введіть номер залікової для детального перегляду: ");
                var recordBook = ReadRequiredLine();
                var found = group.FindStudent(recordBook);
                if (found is not null)
                {
                    found.ShowDetailedInfo();
                }
                else
                {
                    Console.WriteLine("Студента не знайдено.");
                }
                break;
            case "Q":
                return;
            default:
                Console.WriteLine("Невірна команда або неможлива дія.");
                break;
        }
    }
}

static void SearchStudent(StudentGroup group)
{
    Console.WriteLine("--- Пошук студента ---");
    Console.WriteLine("1. За номером залікової книжки");
    Console.WriteLine("2. За фрагментом ПІБ");
    Console.Write("Оберіть тип пошуку: ");
    var searchType = Console.ReadLine();

    switch (searchType)
    {
        case "1":
            Console.Write("Номер залікової: ");
            var recordBook = ReadRequiredLine();
            var byRecord = group.FindStudent(recordBook);
            if (byRecord is not null)
            {
                byRecord.ShowDetailedInfo();
            }
            else
            {
                Console.WriteLine("Студента не знайдено.");
            }
            break;

        case "2":
            Console.Write("Фрагмент ПІБ: ");
            var nameFragment = ReadRequiredLine();
            var foundStudents = group.FindStudentByName(nameFragment);
            if (foundStudents.Count == 0)
            {
                Console.WriteLine("Збігів не знайдено.");
            }
            else
            {
                Console.WriteLine($"Знайдено студентів: {foundStudents.Count}");
                foreach (var student in foundStudents)
                {
                    student.ShowDetailedInfo();
                }
            }
            break;

        default:
            Console.WriteLine("Невірний тип пошуку.");
            break;
    }
}

static void EditStudent(StudentGroup group)
{
    Console.WriteLine("--- Редагування даних студента ---");
    Console.Write("Введіть номер залікової книжки: ");
    var recordBook = ReadRequiredLine();
    var student = group.FindStudent(recordBook);

    if (student is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    student.ShowDetailedInfo();
    Console.WriteLine();
    Console.WriteLine("Що бажаєте змінити?");
    Console.WriteLine("1. Нотатки");
    Console.WriteLine("2. Статус");
    Console.WriteLine("3. Email");
    Console.WriteLine("4. Додати/оновити оцінку в журналі");
    Console.Write("Ваш вибір: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Нові нотатки: ");
            student.Notes = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Нотатки оновлено.");
            break;

        case "2":
            Console.Write("Новий статус (1-Активний, 2-Академ.відпустка, 3-Відрахований, 4-Випускник): ");
            student.Status = ReadStudentStatus();
            Console.WriteLine("Статус оновлено.");
            break;

        case "3":
            Console.Write("Новий email: ");
            student.PersonalEmail = ReadRequiredLine();
            Console.WriteLine("Email оновлено.");
            break;

        case "4":
            Console.Write("Назва предмету: ");
            var subject = ReadRequiredLine();
            Console.Write("Оцінка (0-100): ");
            var grade = ReadDoubleInRange(0, 100);
            student.Journal.AddOrUpdateGrade(subject, grade);
            Console.WriteLine($"Оцінку оновлено. Новий середній бал: {student.AverageGrade:F2}");
            break;

        default:
            Console.WriteLine("Невірний пункт меню.");
            break;
    }
}

static void ShowExcellentAndFailingStudents(StudentGroup group)
{
    Console.WriteLine("--- Відмінники та студенти з балом < 60 ---");

    var excellent = group.GetExcellentStudents();
    Console.WriteLine($"\nВідмінники ({excellent.Count}):");
    if (excellent.Count == 0)
    {
        Console.WriteLine("  Відмінників у групі немає.");
    }
    else
    {
        foreach (var student in excellent)
        {
            Console.WriteLine($"  • {student.FullName} — {student.AverageGrade:F2} балів");
        }
    }

    var failing = group.GetFailingStudents();
    Console.WriteLine($"\nСтуденти з балом < 60 ({failing.Count}):");
    if (failing.Count == 0)
    {
        Console.WriteLine("  Таких студентів немає.");
    }
    else
    {
        foreach (var student in failing)
        {
            Console.WriteLine($"  • {student.FullName} — {student.AverageGrade:F2} балів");
        }
    }
}

static void ShowGroupStatistics(StudentGroup group)
{
    Console.WriteLine("--- Статистика групи ---");
    Console.WriteLine($"Назва групи:           {group.GroupName}");
    Console.WriteLine($"Спеціальність:         {group.Specialty}");
    Console.WriteLine($"Курс:                  {group.Course}");
    Console.WriteLine($"Кількість студентів:   {group.GroupSize}");
    Console.WriteLine($"Середній бал групи:    {group.AverageGroupGrade:F2}");
    Console.WriteLine($"Відмінників:           {group.GetExcellentStudents().Count} ({group.GetExcellentPercentage():F2}%)");
    Console.WriteLine($"З балом < 60:          {group.GetFailingStudents().Count}");

    Console.WriteLine("\nРозподіл за статусами:");
    foreach (StudentStatus status in Enum.GetValues(typeof(StudentStatus)))
    {
        var count = group.GetStudentsByStatus(status).Count;
        if (count > 0)
        {
            Console.WriteLine($"  • {GetStatusName(status)}: {count}");
        }
    }
}

static void SaveGroupToFile(StudentGroup group)
{
    Console.Write("Введіть шлях до файлу (наприклад, group_data.json): ");
    var path = ReadRequiredLine();
    group.SaveToFile(path);
    Console.WriteLine($"Дані групи збережено у файл: {path}");
}

static StudentGroup LoadGroupFromFile()
{
    Console.Write("Введіть шлях до JSON файлу: ");
    var path = ReadRequiredLine();
    var loadedGroup = StudentGroup.LoadFromFile(path);
    Console.WriteLine($"Дані успішно завантажено. Студентів у групі: {loadedGroup.GroupSize}");
    return loadedGroup;
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

static double ReadDoubleInRange(double min, double max)
{
    var input = ReadRequiredLine();
    if (!double.TryParse(input, out var value) || value < min || value > max)
    {
        throw new FormatException($"Введіть число в діапазоні від {min} до {max}.");
    }

    return value;
}

static bool ReadYesNo()
{
    var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
    return answer is "т" or "так" or "y" or "yes";
}

static StudentStatus ReadStudentStatus()
{
    var input = ReadRequiredLine();
    return input switch
    {
        "1" => StudentStatus.Active,
        "2" => StudentStatus.AcademicLeave,
        "3" => StudentStatus.Expelled,
        "4" => StudentStatus.Graduated,
        _ => throw new ArgumentException("Невірний статус. Оберіть число від 1 до 4.")
    };
}

static string GetStatusName(StudentStatus status) => status switch
{
    StudentStatus.Active => "Активний",
    StudentStatus.AcademicLeave => "Академічна відпустка",
    StudentStatus.Expelled => "Відрахований",
    StudentStatus.Graduated => "Випускник",
    _ => status.ToString()
};
