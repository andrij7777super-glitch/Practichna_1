namespace StudentGroupApp;

public class ScienceConference : UniversityEvent
{
    public string Topic { get; set; } = string.Empty;

    public override void ConductEvent()
    {
        Console.WriteLine($"=== НАУКОВА КОНФЕРЕНЦІЯ: {Title} ===");
        Console.WriteLine($"Дата: {Date:dd.MM.yyyy}");
        Console.WriteLine($"Тематика: {Topic}");
        Console.WriteLine("Учасники готують доповіді, презентують дослідження та отримують відгуки від викладачів.");
        Console.WriteLine("Найкращі роботи рекомендуються до публікації у збірнику матеріалів ЗПФК.");
    }
}
