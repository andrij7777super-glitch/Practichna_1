namespace StudentGroupApp;

public class PlayerStats
{
    public string Nickname { get; set; } = string.Empty;
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public double WinRate { get; set; }
    public double KdRatio => Deaths == 0 ? Kills : (double)Kills / Deaths;
    public int TotalPoints => Kills * 2 + (int)(WinRate * 10);
}
