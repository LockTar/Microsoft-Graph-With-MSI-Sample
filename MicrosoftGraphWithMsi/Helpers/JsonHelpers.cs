using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Helpers
{
    public static class JsonHelpers
    {
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
            // Use newtonsoft here because subcollections of Microsoft Graph objects give errors. 
            // Sub collection properties have newtonsoft ignore property. So this will work.
            return Newtonsoft.Json.JsonConvert.SerializeObject(objectToReturnAsJson).FormatJson();
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
