using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace StudentGroupApp;
public static class TextProcessor
{
    public static string Reverse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
    public static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }
    public static int CountCharacters(string text, bool ignoreWhitespace = true)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!ignoreWhitespace)
        {
            return text.Length;
        }

        return text.Count(c => !char.IsWhiteSpace(c));
    }
    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return Regex.Replace(text.Trim(), @"\s+", " ");
    }
    public static bool IsPalindrome(string text, bool ignoreCase = true, bool ignoreSpaces = true)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var processed = text;
        if (ignoreSpaces)
        {
            processed = processed.Replace(" ", string.Empty);
        }

        if (ignoreCase)
        {
            processed = processed.ToLowerInvariant();
        }

        return processed.SequenceEqual(processed.Reverse());
    }
    public static string ReplaceMultiple(string text, Dictionary<string, string> replacements)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(replacements);

        var result = text;
        foreach (var pair in replacements)
        {
            result = result.Replace(pair.Key, pair.Value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
    public static List<string> SplitIntoSentences(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        return Regex.Split(text.Trim(), @"(?<=[.!?…])\s+")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToList();
    }
    public static string BuildGroupReport(StudentGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("           ЗВІТ ГРУПИ (TextProcessor.BuildGroupReport)");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.Append("Група: ").AppendLine(group.GroupName);
        sb.Append("Спеціальність: ").AppendLine(group.Specialty);
        sb.Append("Курс: ").AppendLine(group.Course.ToString());
        sb.Append("Студентів: ").AppendLine(group.GroupSize.ToString());
        sb.Append("Середній бал: ").AppendLine(group.AverageGroupGrade.ToString("F2"));
        sb.AppendLine();

        foreach (var student in group.Students)
        {
            sb.AppendLine(student.GetFormattedInfo(detailed: false));
            sb.AppendLine(new string('-', 40));
        }

        sb.Append("Відмінників: ").AppendLine(group.GetExcellentStudents().Count.ToString());
        sb.Append("Боржників (<60): ").AppendLine(group.GetFailingStudents().Count.ToString());
        return sb.ToString();
    }
    public static string ComparePerformance(int iterations)
    {
        if (iterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "Кількість ітерацій має бути більше нуля.");
        }

        const string fragment = "ЗПФК-студент;";
        var stringWatch = Stopwatch.StartNew();
        var concatResult = string.Empty;
        for (var i = 0; i < iterations; i++)
        {
            concatResult += fragment + i;
        }

        stringWatch.Stop();

        var builderWatch = Stopwatch.StartNew();
        var sb = new StringBuilder();
        for (var i = 0; i < iterations; i++)
        {
            sb.Append(fragment).Append(i);
        }

        var builderResult = sb.ToString();
        builderWatch.Stop();

        var report = new StringBuilder();
        report.AppendLine("=== Бенчмарк: string vs StringBuilder ===");
        report.Append("Ітерацій: ").AppendLine(iterations.ToString());
        report.Append("string (+):     ").Append(stringWatch.ElapsedMilliseconds).AppendLine(" мс");
        report.Append("StringBuilder:  ").Append(builderWatch.ElapsedMilliseconds).AppendLine(" мс");
        report.Append("Довжина результату: ").AppendLine(builderResult.Length.ToString());
        report.Append("Висновок: StringBuilder швидший при великій кількості операцій.");
        return report.ToString();
    }
    public static string GenerateSqlQuery(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Опис запиту не може бути порожнім.", nameof(description));
        }

        var text = Normalize(description).ToLowerInvariant();

        if (ContainsAny(text, "вибрати", "виберіть", "показати", "select", "знайти"))
        {
            return BuildSelectQuery(text);
        }

        if (ContainsAny(text, "додати", "insert", "створити"))
        {
            return BuildInsertQuery(description);
        }

        if (ContainsAny(text, "оновити", "update", "змінити"))
        {
            return BuildUpdateQuery(text, description);
        }

        if (ContainsAny(text, "видалити", "delete", "прибрати"))
        {
            return BuildDeleteQuery(text, description);
        }

        return "-- Не вдалося розпізнати тип запиту. Спробуйте: вибрати / додати / оновити / видалити";
    }

    private static string BuildSelectQuery(string text)
    {
        var sb = new StringBuilder("SELECT * FROM Students");

        var gradeMatch = Regex.Match(text, @"(?:бал|оцінк\w*)\s*(?:більше|вище|>)\s*(\d+)");
        if (gradeMatch.Success)
        {
            sb.Append(" WHERE AverageGrade > ").Append(gradeMatch.Groups[1].Value);
        }
        else if (text.Contains("менше") && Regex.Match(text, @"(\d+)").Success)
        {
            var num = Regex.Match(text, @"(\d+)").Groups[1].Value;
            sb.Append(" WHERE AverageGrade < ").Append(num);
        }
        else if (ContainsAny(text, "відмінник", "відмінники"))
        {
            sb.Append(" WHERE AverageGrade >= 90");
        }
        else if (ContainsAny(text, "боржник", "боржники", "заборгован"))
        {
            sb.Append(" WHERE AverageGrade < 60");
        }
        else if (ContainsAny(text, "активн"))
        {
            sb.Append(" WHERE Status = 'Active'");
        }

        if (text.Contains("ім'я") || text.Contains("піб"))
        {
            var nameMatch = Regex.Match(text, @"піб\s+([а-яіїєґ'\-\s]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success)
            {
                sb.Append(sb.ToString().Contains("WHERE", StringComparison.Ordinal) ? " AND" : " WHERE");
                sb.Append(" FullName LIKE '%").Append(nameMatch.Groups[1].Value.Trim()).Append("%'");
            }
        }

        sb.Append(';');
        return sb.ToString();
    }

    private static string BuildInsertQuery(string original)
    {
        var nameMatch = Regex.Match(original, @"(?:студента?|student)\s+([А-ЯІЇЄҐ][а-яіїєґ'\-]+(?:\s+[А-ЯІЇЄҐ][а-яіїєґ'\-]+){1,2})", RegexOptions.IgnoreCase);
        var name = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : "Невідомий Студент";

        return $"INSERT INTO Students (FullName, Status, EnrollmentDate) VALUES ('{name}', 'Active', GETDATE());";
    }

    private static string BuildUpdateQuery(string text, string original)
    {
        var sb = new StringBuilder("UPDATE Students SET ");

        if (ContainsAny(text, "статус"))
        {
            sb.Append("Status = 'Active'");
        }
        else if (ContainsAny(text, "бал", "оцінк"))
        {
            var num = Regex.Match(text, @"(\d+)").Groups[1].Value;
            sb.Append("AverageGrade = ").Append(string.IsNullOrEmpty(num) ? "0" : num);
        }
        else if (ContainsAny(text, "нотатк", "примітк"))
        {
            sb.Append("Notes = 'Оновлено'");
        }
        else
        {
            sb.Append("Notes = 'Оновлено'");
        }

        var nameMatch = Regex.Match(original, @"([А-ЯІЇЄҐ][а-яіїєґ'\-]+(?:\s+[А-ЯІЇЄҐ][а-яіїєґ'\-]+){1,2})", RegexOptions.IgnoreCase);
        if (nameMatch.Success)
        {
            sb.Append(" WHERE FullName LIKE '%").Append(nameMatch.Groups[1].Value.Trim()).Append("%'");
        }

        sb.Append(';');
        return sb.ToString();
    }

    private static string BuildDeleteQuery(string text, string original)
    {
        var sb = new StringBuilder("DELETE FROM Students");

        var nameMatch = Regex.Match(original, @"([А-ЯІЇЄҐ][а-яіїєґ'\-]+(?:\s+[А-ЯІЇЄҐ][а-яіїєґ'\-]+){1,2})", RegexOptions.IgnoreCase);
        if (nameMatch.Success)
        {
            sb.Append(" WHERE FullName LIKE '%").Append(nameMatch.Groups[1].Value.Trim()).Append("%'");
        }
        else if (ContainsAny(text, "відмінник"))
        {
            sb.Append(" WHERE AverageGrade >= 90");
        }
        else if (ContainsAny(text, "боржник"))
        {
            sb.Append(" WHERE AverageGrade < 60");
        }

        sb.Append(';');
        return sb.ToString();
    }

    private static bool ContainsAny(string text, params string[] keywords) =>
        keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
}
