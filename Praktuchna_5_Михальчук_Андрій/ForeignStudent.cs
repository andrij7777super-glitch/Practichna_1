using System.Text;

namespace StudentGroupApp;

/// <summary>
/// Іноземний студент із власною логікою стипендії.
/// </summary>
public class ForeignStudent : Student
{
    public string CountryOfOrigin { get; set; } = string.Empty;

    public ForeignStudent()
    {
    }

    public ForeignStudent(string fullName, DateTime dateOfBirth, string personalEmail, string recordBookNumber,
        string countryOfOrigin, string notes = "")
        : base(fullName, dateOfBirth, personalEmail, recordBookNumber, notes)
    {
        CountryOfOrigin = countryOfOrigin;
    }

    public override decimal CalculateScholarship()
    {
        var baseAmount = base.CalculateScholarship();
        return baseAmount == 0 ? 0m : baseAmount + 500m;
    }

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.GetInfo());
        sb.AppendLine($"Країна походження: {CountryOfOrigin}");
        sb.AppendLine("Категорія: Іноземний студент");
        return sb.ToString().TrimEnd();
    }
}
