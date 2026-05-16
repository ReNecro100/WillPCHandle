using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

class GigaChatInteractions
{
    private const string TokenUrl = "https://ngw.devices.sberbank.ru:9443";
    private const string ResponseUrl = "https://gigachat.devices.sberbank.ru";
    private string secretToken = "MDE5YTc4MTUtYjEwNS03ZDg2LThmMGQtNzYxMjgzYmFhMGRkOmU5N2NmZDI5LWEwYjQtNDMxZC1iYTI0LTQzYTYxODgzMjU2ZQ==";

    public string GetGigaChatToken(string basicAuthToken)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
        };

        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri(TokenUrl);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v2/oauth");

        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("RqUID", "7bdd1a35-c097-4d77-96c1-71438ee10c8c");
        request.Headers.Add("Authorization", $"Basic {basicAuthToken}");

        request.Content = new StringContent(
            "scope=GIGACHAT_API_PERS",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        using var response = client.Send(request);
        string resultText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        using var jsonDoc = JsonDocument.Parse(resultText);

        if (jsonDoc.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
        {
            return tokenElement.GetString() ?? throw new Exception("Token is null");
        }

        throw new Exception("access_token not found");
    }
    public string GetGigaChatResponse()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
        };

        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri(ResponseUrl);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/chat/completions");

        request.Headers.Add("Content-Type", "application/json");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer " + GetGigaChatToken(secretToken));

        // Ваши данные из --data-raw
        string jsonData = @"{
            ""model"": ""GigaChat"",
            ""messages"": [
                {
                    ""role"": ""user"",
                    ""content"": ""Привет! Как дела?""
                }
            ],
            ""stream"": false,
            ""repetition_penalty"": 1
        }";

        request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        using var response = client.Send(request);
        string resultText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        using var jsonDoc = JsonDocument.Parse(resultText);

        if (jsonDoc.RootElement.TryGetProperty("choices[0].message.content", out JsonElement tokenElement))
        {
            return tokenElement.GetString() ?? throw new Exception("Token is null");
        }
        throw new Exception("choices[0].message.content");
    }
}