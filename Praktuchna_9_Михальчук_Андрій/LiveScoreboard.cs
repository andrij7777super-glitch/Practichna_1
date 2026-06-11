namespace StudentGroupApp;

public class LiveScoreboard
{
    public LiveScoreboard(EsportsMatchTracker tracker)
    {
        tracker.MatchStarted += () => Console.WriteLine("=== Матч розпочато ===");
        tracker.ScoreChanged += (team, score) => Console.WriteLine($"[Scoreboard] {team}: {score}");
        tracker.MatchEnded += () => Console.WriteLine("=== Матч завершено ===");
    }
}
