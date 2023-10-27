using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport.Model;

namespace SitecoreGraphqlmport
{
    internal class Request
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Calls a specified GraphQL endpoint with the specified query and variables.
        /// </summary>
        internal static async Task<GraphQLResponse<TResponse>> CallGraphQLAsync<TResponse>(Uri endpoint, HttpMethod method, string accessToken, string apiKey, string query, object variables, CancellationToken cancellationToken)
        {
            var content = new StringContent(SerializeGraphQLCall(query, variables), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                Content = content,
                RequestUri = endpoint,
            };
            //add authorization headers if necessary here
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
            }
            if (!string.IsNullOrEmpty(apiKey))
            {
                httpRequestMessage.Headers.Add("sc_apikey", apiKey);
            }
            using (var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
            {
                //if (response.IsSuccessStatusCode)
                if (response?.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false); //cancellationToken supported for .NET 5/6
                    return DeserializeGraphQLCall<TResponse>(responseString);
                }
                else
                {
                    throw new ApplicationException($"Unable to contact '{endpoint}': {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        internal static async Task<Uploaded> CallGraphQLUploadAsync<TResponse>(HttpMethod method, string accessToken, string presignedUploadUrl, string filePath, CancellationToken cancellationToken)
        {
            string fileName = Path.GetFileName(filePath);
            byte[] file_bytes = File.ReadAllBytes(filePath);

            var formContent = new MultipartFormDataContent
            {
                // Send Image 
                {new StreamContent(new MemoryStream(file_bytes)),"imagekey",fileName}
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                Content = formContent,
                RequestUri = new Uri(presignedUploadUrl),
            };
            //add authorization headers if necessary here
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
            }
            using (var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
            {
                //if (response.IsSuccessStatusCode)
                if (response?.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false); //cancellationToken supported for .NET 5/6
                    return DeserializeGraphQLUploadCall(responseString);
                }
                else
                {
                    throw new ApplicationException($"Unable to contact '{presignedUploadUrl}': {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public class GraphQLErrorLocation
        {
            public int Line { get; set; }
            public int Column { get; set; }
        }

        public class GraphQLError
        {
            public string Message { get; set; }
            public List<GraphQLErrorLocation> Locations { get; set; }
            public List<object> Path { get; set; } //either int or string
        }

        public class GraphQLResponse<TResponse>
        {
            public List<GraphQLError> Errors { get; set; }
            public TResponse Data { get; set; }
        }

        /// <summary>
        /// Serializes a query and variables to JSON to be sent to the GraphQL endpoint.
        /// </summary>
        private static string SerializeGraphQLCall(string query, object variables)
        {
            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, new
            {
                query = query,
                variables = variables,
            });
            return sb.ToString();
        }

        private static Uploaded DeserializeGraphQLUploadCall(string response)
        {
            var serializer = new JsonSerializer();
            var stringReader = new StringReader(response);
            var jsonReader = new JsonTextReader(stringReader);
            var result = serializer.Deserialize<Uploaded>(jsonReader);
            return result;
        }

        /// <summary>
        /// Deserializes a GraphQL response.
        /// </summary>
        private static GraphQLResponse<TResponse> DeserializeGraphQLCall<TResponse>(string response)
        {
            var serializer = new JsonSerializer();
            var stringReader = new StringReader(response);
            var jsonReader = new JsonTextReader(stringReader);
            var result = serializer.Deserialize<GraphQLResponse<TResponse>>(jsonReader);
            return result;
        }
    }
}
