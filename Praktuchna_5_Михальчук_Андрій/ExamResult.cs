namespace StudentGroupApp;

/// <summary>
/// Результат іспиту (варіант 10 з попередньої роботи).
/// </summary>
public class ExamResult
{
    public string Subject { get; set; } = string.Empty;
    public double Score { get; set; }
    public double MaxScore { get; set; }

    public static ExamResult operator +(ExamResult a, ExamResult b) =>
        new()
        {
            Subject = $"{a.Subject}+{b.Subject}",
            Score = a.Score + b.Score,
            MaxScore = a.MaxScore + b.MaxScore
        };

    public static bool operator true(ExamResult r) => r.MaxScore > 0 && r.Score / r.MaxScore >= 0.6;
    public static bool operator false(ExamResult r) => r.MaxScore <= 0 || r.Score / r.MaxScore < 0.6;

    public static explicit operator double(ExamResult r)
    {
        if (r.MaxScore <= 0)
        {
            return 0;
        }

        return Math.Round(r.Score / r.MaxScore * 100, 2);
    }

    public override string ToString() =>
        $"{Subject}: {Score}/{MaxScore} ({(double)this:F1}%)";
}
