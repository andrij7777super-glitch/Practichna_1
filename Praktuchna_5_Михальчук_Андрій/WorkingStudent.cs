using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Працюючий студент, який не отримує академічну стипендію.
/// </summary>
public class WorkingStudent : Student
{
    public string CompanyName { get; set; } = string.Empty;

    public WorkingStudent()
    {
    }

    public WorkingStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber,
        string companyName, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
    {
        CompanyName = companyName;
    }

    public override decimal CalculateScholarship() => 0m;

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine($"Місце роботи: {CompanyName}");
        sb.AppendLine("Категорія: Працюючий студент (стипендія не нараховується)");
        return sb.ToString().TrimEnd();
    }
}
