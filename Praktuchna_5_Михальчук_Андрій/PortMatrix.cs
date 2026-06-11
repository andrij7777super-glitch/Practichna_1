using System.Text;

namespace StudentGroupApp;
public class PortMatrix
{
    private readonly Port[,] _matrix = new Port[16, 16];
    private bool _isInitialized;
    public void InitializeMatrix()
    {
        for (var row = 0; row < 16; row++)
        {
            for (var col = 0; col < 16; col++)
            {
                _matrix[row, col] = new Port
                {
                    PortNumber = row * 16 + col + 1,
                    IsOpen = false,
                    DeviceName = $"Порт [{row},{col}]"
                };
            }
        }

        _isInitialized = true;
    }
    public void OpenPort(int row, int col)
    {
        EnsureInitialized();
        ValidateCoordinates(row, col);
        _matrix[row, col].IsOpen = true;
    }
    public void ClosePort(int row, int col)
    {
        EnsureInitialized();
        ValidateCoordinates(row, col);
        _matrix[row, col].IsOpen = false;
    }
    public void WriteToPort(int row, int col, byte[] data)
    {
        EnsureInitialized();
        ValidateCoordinates(row, col);

        var port = _matrix[row, col];
        if (!port.IsOpen)
        {
            throw new InvalidOperationException($"Порт [{row},{col}] закритий. Спочатку відкрийте порт.");
        }

        ArgumentNullException.ThrowIfNull(data);

        Array.Clear(port.DataBuffer, 0, port.DataBuffer.Length);
        var length = Math.Min(data.Length, port.DataBuffer.Length);
        Array.Copy(data, port.DataBuffer, length);
    }
    public byte[] ReadFromPort(int row, int col)
    {
        EnsureInitialized();
        ValidateCoordinates(row, col);

        var port = _matrix[row, col];
        if (!port.IsOpen)
        {
            throw new InvalidOperationException($"Порт [{row},{col}] закритий.");
        }

        var result = new byte[port.DataBuffer.Length];
        Array.Copy(port.DataBuffer, result, port.DataBuffer.Length);
        return result;
    }
    public List<Port> ScanMatrix()
    {
        EnsureInitialized();
        var openPorts = new List<Port>();

        for (var row = 0; row < 16; row++)
        {
            for (var col = 0; col < 16; col++)
            {
                if (_matrix[row, col].IsOpen)
                {
                    openPorts.Add(_matrix[row, col]);
                }
            }
        }

        return openPorts;
    }
    public (int Row, int Col)? FindOpenPortByNumber(int portNumber)
    {
        EnsureInitialized();

        for (var row = 0; row < 16; row++)
        {
            for (var col = 0; col < 16; col++)
            {
                var port = _matrix[row, col];
                if (port.IsOpen && port.PortNumber == portNumber)
                {
                    return (row, col);
                }
            }
        }

        return null;
    }
    public Port GetPort(int row, int col)
    {
        EnsureInitialized();
        ValidateCoordinates(row, col);
        return _matrix[row, col];
    }
    public string GetFormattedMatrixState()
    {
        EnsureInitialized();
        var sb = new StringBuilder();
        sb.AppendLine("╔══════════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║              МАТРИЦЯ ПОРТІВ 16×16 (ЗПФК)                        ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════════════╝");
        sb.AppendLine("Легенда: [O] — відкритий, [ ] — закритий");
        sb.AppendLine();

        sb.Append("     ");
        for (var col = 0; col < 16; col++)
        {
            sb.Append(col.ToString("D2")).Append(' ');
        }

        sb.AppendLine();

        for (var row = 0; row < 16; row++)
        {
            sb.Append(row.ToString("D2")).Append("  ");
            for (var col = 0; col < 16; col++)
            {
                sb.Append(_matrix[row, col].IsOpen ? "[O]" : "[ ]");
                if (col < 15)
                {
                    sb.Append(' ');
                }
            }

            sb.AppendLine();
        }

        var openCount = ScanMatrix().Count;
        sb.AppendLine();
        sb.Append("Відкритих портів: ").Append(openCount).Append(" / 256");
        return sb.ToString();
    }
    public List<PortCellSnapshot> ExportState()
    {
        EnsureInitialized();
        var snapshots = new List<PortCellSnapshot>();

        for (var row = 0; row < 16; row++)
        {
            for (var col = 0; col < 16; col++)
            {
                var port = _matrix[row, col];
                snapshots.Add(new PortCellSnapshot
                {
                    Row = row,
                    Col = col,
                    PortNumber = port.PortNumber,
                    IsOpen = port.IsOpen,
                    DeviceName = port.DeviceName,
                    DataBuffer = (byte[])port.DataBuffer.Clone()
                });
            }
        }

        return snapshots;
    }
    public void ImportState(List<PortCellSnapshot> snapshots)
    {
        InitializeMatrix();

        foreach (var snapshot in snapshots)
        {
            ValidateCoordinates(snapshot.Row, snapshot.Col);
            var port = _matrix[snapshot.Row, snapshot.Col];
            port.PortNumber = snapshot.PortNumber;
            port.IsOpen = snapshot.IsOpen;
            port.DeviceName = snapshot.DeviceName;
            Array.Copy(snapshot.DataBuffer, port.DataBuffer, Math.Min(snapshot.DataBuffer.Length, port.DataBuffer.Length));
        }
    }
    public bool IsInitialized => _isInitialized;

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Матриця портів не ініціалізована. Викличте InitializeMatrix().");
        }
    }

    private static void ValidateCoordinates(int row, int col)
    {
        if (row is < 0 or > 15 || col is < 0 or > 15)
        {
            throw new ArgumentOutOfRangeException($"Координати [{row},{col}] виходять за межі матриці 16×16 (0–15).");
        }
    }
}
public class PortCellSnapshot
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int PortNumber { get; set; }
    public bool IsOpen { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public byte[] DataBuffer { get; set; } = Array.Empty<byte>();
}
