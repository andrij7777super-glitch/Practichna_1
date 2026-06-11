namespace StudentGroupApp;

public class Vector
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector operator +(Vector a, Vector b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector operator -(Vector a, Vector b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector operator *(Vector v, double scalar) =>
        new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static Vector operator *(double scalar, Vector v) => v * scalar;

    public static bool operator ==(Vector a, Vector b) =>
        a.X == b.X && a.Y == b.Y && a.Z == b.Z;

    public static bool operator !=(Vector a, Vector b) => !(a == b);

    public static bool operator >(Vector a, Vector b) => a.Length > b.Length;
    public static bool operator <(Vector a, Vector b) => a.Length < b.Length;

    public static Vector operator ++(Vector v) =>
        new(v.X + 1, v.Y + 1, v.Z + 1);

    public static Vector operator --(Vector v) =>
        new(v.X - 1, v.Y - 1, v.Z - 1);

    public static explicit operator double(Vector v) => v.Length;

    public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";

    public override bool Equals(object? obj) => obj is Vector v && this == v;

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
}
