using System.Text;

namespace StudentGroupApp;

public abstract class UniversityMember : Person
{
    protected UniversityMember()
    {
    }

    protected UniversityMember(string fullName, DateTime dateOfBirth, string personalEmail, string notes)
        : base(fullName, dateOfBirth, personalEmail, notes)
    {
    }

    public DateTime? EnrollmentDate { get; protected set; }

    public abstract decimal CalculateScholarship();

    public virtual void Enroll()
    {
        EnrollmentDate = DateTime.Today;
    }

    public override string GetInfo()
    {
        var sb = new StringBuilder();
        sb.Append(base.GetInfo());
        if (EnrollmentDate.HasValue)
        {
            sb.AppendLine();
            sb.Append($"Дата зарахування: {EnrollmentDate:dd.MM.yyyy}");
        }

        return sb.ToString();
    }
}
