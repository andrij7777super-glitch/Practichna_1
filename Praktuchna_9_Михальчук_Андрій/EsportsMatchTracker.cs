namespace StudentGroupApp;

public class EsportsMatchTracker
{
    public event Action? MatchStarted;
    public event Action<string, int>? ScoreChanged;
    public event Action? MatchEnded;

    public void StartMatch()
    {
        MatchStarted?.Invoke();
    }

    public void UpdateScore(string team, int score)
    {
        ScoreChanged?.Invoke(team, score);
    }

    public void EndMatch()
    {
        MatchEnded?.Invoke();
    }
}
