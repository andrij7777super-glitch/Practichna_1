using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Випускник (sealed) — кінцевий рівень ієрархії студентів.
/// </summary>
public sealed class GraduateStudent : Student
{
    public string ThesisTopic { get; set; } = string.Empty;

    public GraduateStudent()
    {
        Status = StudentStatus.Graduated;
    }

    public GraduateStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber,
        string thesisTopic, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
    {
        ThesisTopic = thesisTopic;
        Status = StudentStatus.Graduated;
    }

    public override decimal CalculateScholarship() => 3000m;

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine($"Тема дипломної роботи: {ThesisTopic}");
        sb.AppendLine("Категорія: Випускник");
        return sb.ToString().TrimEnd();
    }
}
