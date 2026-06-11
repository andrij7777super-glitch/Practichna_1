namespace StudentGroupApp;

public abstract class Shape : IResizable, IDrawable, IPrintable
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public virtual double CalculateArea() => 0;

    public virtual double CalculatePerimeter() => 0;

    public abstract string GetDescription();

    public virtual void Resize(double factor)
    {
    }

    public virtual void Draw()
    {
        Console.WriteLine($"[Draw] {GetDescription()}");
    }

    public virtual string GetPrintInfo()
    {
        return GetDescription();
    }
}
