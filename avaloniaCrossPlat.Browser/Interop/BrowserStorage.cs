using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;

namespace avaloniaCrossPlat.Browser.Interop;


public static partial class BrowserStorage
{
    [JSImport("appStorage.set")]
    public static partial void Set(string key, string value);

    [JSImport("appStorage.get")]
    public static partial string? Get(string key);

    [JSImport("appStorage.remove")]
    public static partial void Remove(string key);
}
