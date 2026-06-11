namespace StudentGroupApp;

public class Triangle : Shape
{
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }

    public override double CalculateArea()
    {
        var s = CalculatePerimeter() / 2;
        return Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
    }

    public override double CalculatePerimeter() => SideA + SideB + SideC;

    public override string GetDescription() =>
        $"Трикутник «{Name}», колір {Color}, сторони {SideA:F2}/{SideB:F2}/{SideC:F2}, S={CalculateArea():F2}";

    public override void Resize(double factor)
    {
        SideA *= factor;
        SideB *= factor;
        SideC *= factor;
    }
}
