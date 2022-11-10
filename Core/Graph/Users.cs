using Microsoft.Graph;
using Core.Helpers;
using System.Net.Http.Headers;

namespace Core.Graph
{
    public static class Users
    {
        private const string RETRY_AFTER = "Retry-After";

        public static async Task DisplayLoggedInUserInfoAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput = true)
        {
            User user = await graphClient.Me
                            .Request()
                            .GetAsync();

            Console.WriteLine("Logged in user:");
            PrintUserInformation(user, writeJsonObjectsToOutput);
        }

        public static async Task DisplayUserInfoAsync(GraphServiceClient graphClient, string userId, bool writeJsonObjectsToOutput = true)
        {
            const int MaxRetry = 5; // So number of call are (MaxRetry + 1)

            User user = await graphClient.Users[userId]
                            .Request()
                            .WithMaxRetry(MaxRetry)
                            .WithShouldRetry((delay, attempt, httpResponse) =>
                            {
                                Console.WriteLine($"Request returned status code {httpResponse.StatusCode}");

                                // Add more status codes here or change your if statement...
                                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                    return false;

                                double delayInSeconds = CalculateDelay(httpResponse, attempt, delay);

                                if (attempt == 0)
                                    Console.WriteLine($"Request failed, let's retry after a delay of {delayInSeconds} seconds");
                                else if (attempt == MaxRetry)
                                    Console.WriteLine($"This was the last retry attempt {attempt}");
                                else
                                    Console.WriteLine($"This was retry attempt {attempt}, let's retry after a delay of {delayInSeconds} seconds");

                                return true;
                            })
                            .GetAsync();

            Console.WriteLine("User information:");
            PrintUserInformation(user, writeJsonObjectsToOutput);
        }

        /// <summary>
        /// This is reverse engineerd from:
        /// https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/dev/src/Microsoft.Graph.Core/Requests/Middleware/RetryHandler.cs#L164
        /// </summary>
        /// <param name="response"></param>
        /// <param name="retry_count"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private static double CalculateDelay(HttpResponseMessage response, int retry_count, int delay)
        {
            HttpHeaders headers = response.Headers;
            double delayInSeconds = delay;
            if (headers.TryGetValues(RETRY_AFTER, out IEnumerable<string> values))
            {
                string retry_after = values.First();
                if (int.TryParse(retry_after, out int delay_seconds))
                {
                    delayInSeconds = delay_seconds;
                }
            }
            else
            {
                var m_pow = Math.Pow(2, retry_count);
                delayInSeconds = m_pow * delay;
            }

            const int MAX_DELAY = 180; // From github code https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/2e43863e349b4b3ebe2e166c26e3afcc4a974365/src/Microsoft.Graph.Core/Requests/Middleware/Options/RetryHandlerOption.cs#L18
            delayInSeconds = Math.Min(delayInSeconds, MAX_DELAY);

            return delayInSeconds;
        }



        private static void PrintUserInformation(User user, bool writeJsonObjectsToOutput)
        {
            Console.WriteLine($"Displayname: {user.DisplayName}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("User in JSON:");
                string json = user.ToFormattedJson();
                Console.WriteLine(json);
            }
        }
    }
}
