using System.Text;

namespace StudentGroupApp;

public class ExcellentStudent : Student
{
    public ExcellentStudent()
    {
    }

    public ExcellentStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
    {
    }

    public override decimal CalculateScholarship() => AverageGrade >= 90 ? 2500m : base.CalculateScholarship();

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine("Категорія: Відмінник (підвищена стипендія)");
        return sb.ToString().TrimEnd();
    }
}
