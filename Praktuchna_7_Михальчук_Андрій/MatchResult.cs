namespace StudentGroupApp;

public readonly struct MatchResult : IEquatable<MatchResult>
{
    public int Team1Score { get; }
    public int Team2Score { get; }
    public int DurationMinutes { get; }
    public bool Team1Won { get; }

    public MatchResult(int team1Score, int team2Score, int durationMinutes, bool team1Won)
    {
        Team1Score = team1Score;
        Team2Score = team2Score;
        DurationMinutes = durationMinutes;
        Team1Won = team1Won;
    }

    public void Deconstruct(out int team1Score, out int team2Score, out int durationMinutes, out bool team1Won)
    {
        team1Score = Team1Score;
        team2Score = Team2Score;
        durationMinutes = DurationMinutes;
        team1Won = Team1Won;
    }

    public bool Equals(MatchResult other) =>
        Team1Score == other.Team1Score &&
        Team2Score == other.Team2Score &&
        DurationMinutes == other.DurationMinutes &&
        Team1Won == other.Team1Won;

    public override bool Equals(object? obj) => obj is MatchResult m && Equals(m);

    public override int GetHashCode() =>
        HashCode.Combine(Team1Score, Team2Score, DurationMinutes, Team1Won);

    public static bool operator ==(MatchResult a, MatchResult b) => a.Equals(b);

    public static bool operator !=(MatchResult a, MatchResult b) => !a.Equals(b);

    public override string ToString() =>
        $"{Team1Score}:{Team2Score}, {DurationMinutes} хв, переможець: {(Team1Won ? "Команда 1" : "Команда 2")}";
}
