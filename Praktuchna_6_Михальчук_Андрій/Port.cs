namespace StudentGroupApp;

public class Port : ICloneable
{
    public int PortNumber { get; set; }
    public byte[] DataBuffer { get; } = new byte[64];
    public bool IsOpen { get; set; }
    public string DeviceName { get; set; } = string.Empty;

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
