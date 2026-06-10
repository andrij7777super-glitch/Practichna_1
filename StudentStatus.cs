namespace StudentGroupApp;

/// <summary>
/// Статус студента в навчальному закладі.
/// </summary>
public enum StudentStatus
{
    /// <summary>Активний студент (навчається).</summary>
    Active,

    /// <summary>Студент у академічній відпустці.</summary>
    AcademicLeave,

    /// <summary>Відрахований студент.</summary>
    Expelled,

    /// <summary>Випускник.</summary>
    Graduated
}
