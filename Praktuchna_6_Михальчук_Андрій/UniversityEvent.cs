namespace StudentGroupApp;

public abstract class UniversityEvent
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public abstract void ConductEvent();
}
