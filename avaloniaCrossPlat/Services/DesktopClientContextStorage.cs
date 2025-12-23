using avaloniaCrossPlat.Services;
using System;
using System.IO;

namespace avaloniaCrossPlat.Desktop.Services;

public class DesktopClientContextStorage : IClientContextStorage
{
    private readonly string _file =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RawDataVisualizer",
            "client.txt");

    public string? GetClientId()
        => File.Exists(_file) ? File.ReadAllText(_file) : null;

    public void SetClientId(string clientId)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_file)!);
        File.WriteAllText(_file, clientId);
    }

    public void Clear()
    {
        if (File.Exists(_file))
            File.Delete(_file);
    }
}
