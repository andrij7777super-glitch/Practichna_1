namespace StudentGroupApp;

/// <summary>
/// Кіберспортивний турнір серед студентів.
/// </summary>
public class EsportsTournament : UniversityEvent
{
    public string GameTitle { get; set; } = string.Empty;
    public decimal PrizePool { get; set; }

    public override void ConductEvent()
    {
        Console.WriteLine($"=== КІБЕРСПОРТИВНИЙ ТУРНІР: {Title} ===");
        Console.WriteLine($"Дата: {Date:dd.MM.yyyy}");
        Console.WriteLine($"Гра: {GameTitle}");
        Console.WriteLine($"Призовий фонд: {PrizePool:F2} грн");
        Console.WriteLine("Реєстрація команд, груповий етап, плей-оф та фінал у комп'ютерному класі ЗПФК.");
        Console.WriteLine("Трансляція матчів ведеться на внутрішньому Discord-сервері коледжу.");
    }
}
