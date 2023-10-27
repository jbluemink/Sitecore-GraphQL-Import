using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport.Model;

namespace SitecoreGraphqlmport
{
    internal class GetItem
    {
        /// <summary>
        /// Sample method which calls a GraphQL endpoint.
        /// </summary>
        internal static async Task GetSitecoreItem(CancellationToken cancellationToken, string itemPath)
        {
            string graphqlendpoint = System.Configuration.ConfigurationManager.AppSettings.Get("graphqlEdgePreviewUrl");
            string apikey = System.Configuration.ConfigurationManager.AppSettings.Get("scApikey");

            Console.WriteLine("Searching for Sitecore Item...");

            // Call GraphQL endpoint here, specifying return data type, endpoint, method, query, and variables
            var result = await Request.CallGraphQLAsync<DataItem>(
                new Uri(graphqlendpoint),
                HttpMethod.Post,
                "",
                apikey,
                "query { item(path: \""+itemPath+ "\", language: \"en\") {id, name }}",
                new
                {
                },
                cancellationToken);

            // Examine the GraphQL response to see if any errors were encountered
            if (result.Errors?.Count > 0)
            {
                Console.WriteLine($"GraphQL returned errors:\n{string.Join("\n", result.Errors.Select(x => $"  - {x.Message}"))}");
                return;
            }

            // Use the response data
            if (result.Data.item == null)
            {
                Console.WriteLine($"item not found ,");
            }
            else
            {
                Console.WriteLine($"Found item {result.Data.item?.id} ,");
            }
        }

    }
}

