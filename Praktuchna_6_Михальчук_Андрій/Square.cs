namespace StudentGroupApp;

public class Square : Rectangle
{
    public double Side
    {
        get => Width;
        set
        {
            Width = value;
            Height = value;
        }
    }

    public override string GetDescription() =>
        $"Квадрат «{Name}», колір {Color}, a={Width:F2}, S={CalculateArea():F2}";

    public override void Resize(double factor)
    {
        Side *= factor;
    }
}
