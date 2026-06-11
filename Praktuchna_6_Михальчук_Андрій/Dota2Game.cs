namespace StudentGroupApp;

public class Dota2Game : EsportGame
{
    public override void StartMatch()
    {
        Console.WriteLine($"=== Dota 2: {Title} ===");
        Console.WriteLine("Radiant проти Dire. Герої виходять на лінії, фармять золото та б'ються за Ancient.");
    }
}
