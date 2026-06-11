namespace StudentGroupApp;

public readonly struct GradeRecord : IEquatable<GradeRecord>
{
    public string Subject { get; }
    public double Grade { get; }

    public GradeRecord(string subject, double grade)
    {
        Subject = subject;
        Grade = grade;
    }

    public void Deconstruct(out string subject, out double grade)
    {
        subject = Subject;
        grade = Grade;
    }

    public bool Equals(GradeRecord other) =>
        Subject == other.Subject && Grade == other.Grade;

    public override bool Equals(object? obj) => obj is GradeRecord g && Equals(g);

    public override int GetHashCode() => HashCode.Combine(Subject, Grade);

    public static bool operator ==(GradeRecord a, GradeRecord b) => a.Equals(b);

    public static bool operator !=(GradeRecord a, GradeRecord b) => !a.Equals(b);

    public override string ToString() => $"{Subject}: {Grade:F1}";
}
