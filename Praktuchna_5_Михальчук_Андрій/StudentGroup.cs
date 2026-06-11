namespace StudentGroupApp;

/// <summary>
/// Навчальна група з поліморфною колекцією членів університету.
/// </summary>
public class StudentGroup
{
    private readonly List<UniversityMember> _members = new();

    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int Course { get; set; }

    public int MemberCount => _members.Count;

    public IReadOnlyList<UniversityMember> Members => _members.AsReadOnly();

    public void AddMember(UniversityMember member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (member is Student student &&
            _members.OfType<Student>().Any(s => s.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException(
                $"Студент із номером залікової {student.RecordBookNumber} вже існує в групі.");
        }

        _members.Add(member);
    }

    public List<T> GetMembersByType<T>() where T : UniversityMember =>
        _members.OfType<T>().ToList();

    public decimal GetTotalScholarship() =>
        _members.Sum(m => m.CalculateScholarship());

    public void PrintAllMembers()
    {
        if (_members.Count == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        for (var i = 0; i < _members.Count; i++)
        {
            Console.WriteLine($"--- Учасник #{i + 1} ---");
            Console.WriteLine(_members[i].GetInfo());
            Console.WriteLine();
        }
    }
}
