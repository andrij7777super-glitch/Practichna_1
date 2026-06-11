using System.Diagnostics;

namespace StudentGroupApp;

public static class PerformanceTest
{
    private const int Count = 1_000_000;
    private const string SearchKey = "00500000";

    public static void Run()
    {
        var records = new StudentRecord[Count];
        var students = new Student[Count];

        var fillRecords = MeasureFillRecords(records);
        var fillStudents = MeasureFillStudents(students);

        var searchRecords = MeasureSearchRecords(records);
        var searchStudents = MeasureSearchStudents(students);

        var sortRecords = MeasureSortRecords(records);
        var sortStudents = MeasureSortStudents(students);

        Console.WriteLine("--- Порівняння StudentRecord (struct) vs Student (class) ---");
        Console.WriteLine($"{"Операція",-20} {"StudentRecord",-18} {"Student",-18}");
        Console.WriteLine(new string('-', 58));
        PrintRow("Заповнення", fillRecords, fillStudents);
        PrintRow("Лінійний пошук", searchRecords, searchStudents);
        PrintRow("Сортування", sortRecords, sortStudents);
    }

    private static long MeasureFillRecords(StudentRecord[] arr)
    {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < Count; i++)
        {
            arr[i] = new StudentRecord($"Student{i}", i.ToString("D8"), i % 100);
        }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long MeasureFillStudents(Student[] arr)
    {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < Count; i++)
        {
            arr[i] = new Student($"Student{i}", new DateTime(2000, 1, 1), $"s{i}@test.com", i.ToString("D8"));
            arr[i].UpdateAverageGrade(i % 100);
        }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long MeasureSearchRecords(StudentRecord[] arr)
    {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].RecordBookNumber == SearchKey)
            {
                break;
            }
        }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long MeasureSearchStudents(Student[] arr)
    {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].RecordBookNumber == SearchKey)
            {
                break;
            }
        }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long MeasureSortRecords(StudentRecord[] arr)
    {
        var sw = Stopwatch.StartNew();
        Array.Sort(arr, (a, b) => a.AverageGrade.CompareTo(b.AverageGrade));
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static long MeasureSortStudents(Student[] arr)
    {
        var sw = Stopwatch.StartNew();
        Array.Sort(arr, (a, b) => a.AverageGrade.CompareTo(b.AverageGrade));
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    private static void PrintRow(string operation, long structMs, long classMs)
    {
        Console.WriteLine($"{operation,-20} {structMs} мс{"",-12} {classMs} мс");
    }
}
