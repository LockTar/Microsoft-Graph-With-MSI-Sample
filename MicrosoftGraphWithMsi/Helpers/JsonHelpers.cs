using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Helpers
{
    public static class JsonHelpers
    {
        public static string FormatJson(object objectToReturnAsJson)
        {
            try
            {
                string prettyJson = JsonSerializer.Serialize(objectToReturnAsJson, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                return prettyJson;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static string FormatJson(string json)
        {
            try
            {
                var value = JsonSerializer.Deserialize<object>(json);
                string prettyJson = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                return prettyJson;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async static Task<string> FormatJsonAsync(Stream jsonStream)
        {
            try
            {
                var value = await JsonSerializer.DeserializeAsync<object>(jsonStream);
                string prettyJson = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                return prettyJson;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    public static class JsonExtensionMethod
    {
        public static string ToFormattedJson(this object objectToReturnAsJson)
        {
            return JsonHelpers.FormatJson(objectToReturnAsJson);
        }

        public static string FormatJson(this string json)
        {
            return JsonHelpers.FormatJson(json);
        }

        public async static Task<string> FormatJsonAsync(this Stream jsonStream)
        {
            return await JsonHelpers.FormatJsonAsync(jsonStream);
        }
    }
}
