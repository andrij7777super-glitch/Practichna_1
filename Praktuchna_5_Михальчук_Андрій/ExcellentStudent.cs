using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Студент-відмінник із підвищеною стипендією.
/// </summary>
public class ExcellentStudent : Student
{
    public ExcellentStudent()
    {
    }

    public ExcellentStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
    {
    }

    public override decimal CalculateScholarship()
    {
        if (AverageGrade >= 90)
        {
            return 2500m;
        }

        return base.CalculateScholarship();
    }

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine("Категорія: Відмінник (підвищена стипендія)");
        return sb.ToString().TrimEnd();
    }
}
