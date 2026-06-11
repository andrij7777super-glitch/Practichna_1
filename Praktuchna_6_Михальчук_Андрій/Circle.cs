namespace StudentGroupApp;

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea() => Math.PI * Radius * Radius;

    public override double CalculatePerimeter() => 2 * Math.PI * Radius;

    public override string GetDescription() =>
        $"Коло «{Name}», колір {Color}, R={Radius:F2}, S={CalculateArea():F2}";

    public override void Resize(double factor)
    {
        Radius *= factor;
    }
}
