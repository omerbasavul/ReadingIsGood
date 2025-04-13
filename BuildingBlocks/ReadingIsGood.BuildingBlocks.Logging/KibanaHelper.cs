using System.Text;
using Newtonsoft.Json;

namespace ReadingIsGood.BuildingBlocks.Logging;

public static class KibanaHelper
{
    public static async Task CreateKibanaIndexPatternAsync(
        string kibanaUrl,
        string patternId,
        string indexPatternTitle,
        string timeFieldName = "@timestamp")
    {
        var requestUrl = $"{kibanaUrl.TrimEnd('/')}/api/saved_objects/index-pattern/{patternId}";

        var payload = new
        {
            attributes = new
            {
                title = indexPatternTitle, timeFieldName
            }
        };
        var json = JsonConvert.SerializeObject(payload);

        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("kbn-xsrf", "true");

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create Kibana index pattern: {response.StatusCode}, {body}");
        }
    }
}