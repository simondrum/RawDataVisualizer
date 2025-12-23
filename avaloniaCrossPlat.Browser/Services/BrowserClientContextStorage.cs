using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using avaloniaCrossPlat.Services;
using avaloniaCrossPlat.Browser.Interop;

namespace avaloniaCrossPlat.Browser.Services;

public class BrowserClientContextStorage : IClientContextStorage
{
    private const string Key = "rawdatavisualizer.clientId";

    public string? GetClientId()
        => BrowserStorage.Get(Key);

    public void SetClientId(string clientId)
        => BrowserStorage.Set(Key, clientId);

    public void Clear()
        => BrowserStorage.Remove(Key);
}
