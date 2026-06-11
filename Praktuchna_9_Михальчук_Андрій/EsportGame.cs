namespace StudentGroupApp;

public abstract class EsportGame
{
    public string Title { get; set; } = string.Empty;

    public virtual void StartMatch()
    {
        Console.WriteLine($"Матч {Title} розпочато.");
    }
}
