using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace avaloniaCrossPlat.Services;

public interface IClientContextStorage
{
    string? GetClientId();
    void SetClientId(string clientId);
    void Clear();
}
