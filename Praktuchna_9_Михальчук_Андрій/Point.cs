namespace StudentGroupApp;

public readonly struct Point : IEquatable<Point>
{
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out double x, out double y)
    {
        x = X;
        y = Y;
    }

    public bool Equals(Point other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Point p && Equals(p);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Point a, Point b) => a.Equals(b);

    public static bool operator !=(Point a, Point b) => !a.Equals(b);

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
