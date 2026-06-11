namespace StudentGroupApp;

/// <summary>
/// Хакатон — командний ІТ-захід.
/// </summary>
public class Hackathon : UniversityEvent
{
    public string TechnologyStack { get; set; } = string.Empty;

    public override void ConductEvent()
    {
        Console.WriteLine($"=== ХАКАТОН: {Title} ===");
        Console.WriteLine($"Дата: {Date:dd.MM.yyyy}");
        Console.WriteLine($"Стек технологій: {TechnologyStack}");
        Console.WriteLine("Команди формуються, розробляють прототип за 24 години та презентують жюрі.");
        Console.WriteLine("Переможці отримують сертифікати та мерч від партнерів ЗПФК.");
    }
}
