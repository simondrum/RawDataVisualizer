
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace avaloniaCrossPlat;
class HttpService
{
    public async Task<string> GetInfos(string id = "")
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"https://www.vbus.net/api/v5/data/live-system/{id}");
        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(jsonResponse);
        return jsonResponse;
    }
}