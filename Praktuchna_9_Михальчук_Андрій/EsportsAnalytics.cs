namespace StudentGroupApp;

public static class EsportsAnalytics
{
    public static PlayerStats? GetBestKdPlayer(List<PlayerStats> players) =>
        players.OrderByDescending(p => p.KdRatio).FirstOrDefault();

    public static Dictionary<string, List<PlayerStats>> GroupPlayersByWinRate(List<PlayerStats> players) =>
        players.GroupBy(p => p.WinRate > 50 ? ">50%" : "<=50%")
            .ToDictionary(g => g.Key, g => g.ToList());

    public static List<PlayerStats> GetPlayersSortedByPoints(List<PlayerStats> players) =>
        players.OrderByDescending(p => p.TotalPoints).ToList();
}
