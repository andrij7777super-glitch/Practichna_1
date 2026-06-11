namespace StudentGroupApp;

public readonly struct StudentRecord : IEquatable<StudentRecord>
{
    public string FullName { get; }
    public string RecordBookNumber { get; }
    public double AverageGrade { get; }

    public StudentRecord(string fullName, string recordBookNumber, double averageGrade)
    {
        FullName = fullName;
        RecordBookNumber = recordBookNumber;
        AverageGrade = averageGrade;
    }

    public void Deconstruct(out string fullName, out string recordBookNumber, out double averageGrade)
    {
        fullName = FullName;
        recordBookNumber = RecordBookNumber;
        averageGrade = AverageGrade;
    }

    public bool Equals(StudentRecord other) =>
        FullName == other.FullName &&
        RecordBookNumber == other.RecordBookNumber &&
        AverageGrade == other.AverageGrade;

    public override bool Equals(object? obj) => obj is StudentRecord s && Equals(s);

    public override int GetHashCode() => HashCode.Combine(FullName, RecordBookNumber, AverageGrade);

    public static bool operator ==(StudentRecord a, StudentRecord b) => a.Equals(b);

    public static bool operator !=(StudentRecord a, StudentRecord b) => !a.Equals(b);

    public override string ToString() =>
        $"{FullName} [{RecordBookNumber}] — {AverageGrade:F2}";
}
