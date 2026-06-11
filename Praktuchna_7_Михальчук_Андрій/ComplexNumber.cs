namespace StudentGroupApp;

public readonly struct ComplexNumber : IEquatable<ComplexNumber>
{
    public double Real { get; }
    public double Imaginary { get; }

    public ComplexNumber(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }

    public void Deconstruct(out double real, out double imaginary)
    {
        real = Real;
        imaginary = Imaginary;
    }

    public bool Equals(ComplexNumber other) =>
        Real == other.Real && Imaginary == other.Imaginary;

    public override bool Equals(object? obj) => obj is ComplexNumber c && Equals(c);

    public override int GetHashCode() => HashCode.Combine(Real, Imaginary);

    public static bool operator ==(ComplexNumber a, ComplexNumber b) => a.Equals(b);

    public static bool operator !=(ComplexNumber a, ComplexNumber b) => !a.Equals(b);

    public override string ToString() => $"{Real:F2} + {Imaginary:F2}i";
}
