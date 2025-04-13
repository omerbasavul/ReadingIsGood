using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ReadingIsGood.BuildingBlocks.Logging;

public static class KibanaHelper
{
    public static async Task CreateKibanaIndexPatternAsync(
        string kibanaUrl,
        string patternId,
        string indexPatternTitle,
        string timeFieldName = "@timestamp",
        int retryDelaySec = 2
    )
    {
        var requestUrl = $"{kibanaUrl.TrimEnd('/')}/api/saved_objects/index-pattern/{patternId}";
        var payload = new
        {
            attributes = new
            {
                title = indexPatternTitle,
                timeFieldName
            }
        };
        var json = JsonConvert.SerializeObject(payload);

        var attempt = 0;

        while (true)
        {
            attempt++;

            try
            {
                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                request.Headers.Add("kbn-xsrf", "true");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Kibana] Created/Updated index pattern '{patternId}' => '{indexPatternTitle}'");
                    return;
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        Console.WriteLine($"[Kibana] Index pattern '{patternId}' already exists or version conflict. Ignoring. Message: {body}");

                        return;
                    }

                    throw new Exception($"Failed to create Kibana index pattern: {response.StatusCode}, {body}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[Kibana] Attempt #{attempt} failed - {httpEx.Message}");

                if (attempt >= 20)
                {
                    return;
                }

                var delaySeconds = retryDelaySec * (int)Math.Pow(2, attempt - 1);
                Console.WriteLine($"[Kibana] Retrying in {delaySeconds} seconds...");
                await Task.Delay(delaySeconds * 1000);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}