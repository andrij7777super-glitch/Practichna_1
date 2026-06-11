namespace StudentGroupApp;

/// <summary>
/// Оцінка у 10-бальній шкалі з перевантаженими операторами.
/// </summary>
public class GradePoint
{
    public double Value { get; set; }

    public GradePoint(double value)
    {
        Value = Validate(value);
    }

    public static GradePoint operator +(GradePoint a, GradePoint b) =>
        new(Math.Min(a.Value + b.Value, 10));

    public static GradePoint operator ++(GradePoint g) =>
        new(Math.Min(g.Value + 1, 10));

    public static GradePoint operator --(GradePoint g) =>
        new(Math.Max(g.Value - 1, 0));

    public static bool operator >(GradePoint a, GradePoint b) => a.Value > b.Value;
    public static bool operator <(GradePoint a, GradePoint b) => a.Value < b.Value;
    public static bool operator >=(GradePoint a, GradePoint b) => a.Value >= b.Value;
    public static bool operator <=(GradePoint a, GradePoint b) => a.Value <= b.Value;

    public static bool operator true(GradePoint g) => g.Value >= 8;
    public static bool operator false(GradePoint g) => g.Value < 8;

    public static implicit operator GradePoint(double d) => new(d);
    public static implicit operator double(GradePoint g) => g.Value;

    public override string ToString() => Value.ToString("F1");

    private static double Validate(double value)
    {
        if (value is < 0 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Оцінка має бути від 0 до 10.");
        }

        return Math.Round(value, 2);
    }
}
