namespace StudentGroupApp;

public class CS2Game : EsportGame
{
    public override void StartMatch()
    {
        Console.WriteLine($"=== Counter-Strike 2: {Title} ===");
        Console.WriteLine("Команда CT проти T. Раунд 1 — розминка, закупівля зброї, захоплення точки.");
    }
}
