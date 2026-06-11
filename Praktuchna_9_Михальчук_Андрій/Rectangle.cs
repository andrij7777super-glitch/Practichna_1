namespace StudentGroupApp;

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea() => Width * Height;

    public override double CalculatePerimeter() => 2 * (Width + Height);

    public override string GetDescription() =>
        $"Прямокутник «{Name}», колір {Color}, {Width:F2}×{Height:F2}, S={CalculateArea():F2}";

    public override void Resize(double factor)
    {
        Width *= factor;
        Height *= factor;
    }
}
