using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport.Model;

namespace SitecoreGraphqlmport
{
    internal class GetSites
    {

        /// <summary>
        /// Sample method which calls a GraphQL endpoint.
        /// </summary>
        internal static async Task GetSitecoreSites(CancellationToken cancellationToken)
        {
            string graphqlendpoint = System.Configuration.ConfigurationManager.AppSettings.Get("graphqlAuthoringUrl");
            string accessToken = System.Configuration.ConfigurationManager.AppSettings.Get("accessToken");

            Console.WriteLine("Searching for Sitecore Sites...");

            // Call GraphQL endpoint here, specifying return data type, endpoint, method, query, and variables
            var result = await Request.CallGraphQLAsync<Sites>(
                new Uri(graphqlendpoint),
                HttpMethod.Post,
                accessToken,
                "",
                "query { sites { name, language }}",
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
            Console.WriteLine($"Found {result.Data.sites.Count} site(s),");
            foreach (var site in result.Data.sites)
            {
                Console.WriteLine($"{site.name} with main language {site.language},");
            }
        }

    }
}

