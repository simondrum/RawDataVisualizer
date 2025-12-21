
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace avaloniaCrossPlat;
class HttpService
{
    public async Task<string> GetInfos(string url = "")
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(jsonResponse);
        return jsonResponse;
    }
}