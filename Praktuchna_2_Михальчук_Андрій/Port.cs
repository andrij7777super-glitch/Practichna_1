namespace StudentGroupApp;

/// <summary>
/// Симуляція порту вводу/виводу з буфером даних.
/// </summary>
public class Port : ICloneable
{
    /// <summary>
    /// Унікальний номер порту в системі.
    /// </summary>
    public int PortNumber { get; set; }

    /// <summary>
    /// Буфер даних порту (64 байти).
    /// </summary>
    public byte[] DataBuffer { get; } = new byte[64];

    /// <summary>
    /// Чи відкритий порт для обміну даними.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Назва пристрою або ім'я прив'язаного студента.
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Створює глибоку копію порту з окремим буфером даних.
    /// </summary>
    public object Clone()
    {
        var clone = new Port
        {
            PortNumber = PortNumber,
            IsOpen = IsOpen,
            DeviceName = DeviceName
        };

        Array.Copy(DataBuffer, clone.DataBuffer, DataBuffer.Length);
        return clone;
    }

    /// <summary>
    /// Повертає рядкове представлення вмісту буфера.
    /// </summary>
    public string GetBufferAsString()
    {
        var length = Array.FindIndex(DataBuffer, b => b == 0);
        if (length < 0)
        {
            length = DataBuffer.Length;
        }

        return System.Text.Encoding.UTF8.GetString(DataBuffer, 0, length);
    }
}
