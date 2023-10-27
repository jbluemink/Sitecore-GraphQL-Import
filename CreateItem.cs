using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport.Model;

namespace SitecoreGraphqlmport
{
    internal class CreateItem
    {
        /// <summary>
        /// Sample method which calls a GraphQL endpoint.
        /// </summary>
        internal static async Task CreateSampleItem(CancellationToken cancellationToken, string itemname, Guid parentID)
        {
            string graphqlendpoint = System.Configuration.ConfigurationManager.AppSettings.Get("graphqlAuthoringUrl");
            string accessToken = System.Configuration.ConfigurationManager.AppSettings.Get("accessToken");

            Console.WriteLine("Try to Create item");

            // Call GraphQL endpoint here, specifying return data type, endpoint, method, query, and variables
            var result = await Request.CallGraphQLAsync<CreateSampleItem>(
                new Uri(graphqlendpoint),
                HttpMethod.Post,
                accessToken,
                "",
                "mutation {\r\n  createItem(\r\n    input: {\r\n      name: \""+itemname+"\"\r\n      templateId: \"{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}\"\r\n      parent: \""+parentID.ToString()+"\"\r\n      language: \"en\"\r\n      fields: [\r\n        { name: \"title\", value: \"Welcome to Sitecore\" }\r\n        { name: \"text\", value: \"Welcome to Sitecore\" }\r\n      ]\r\n    }\r\n  ) {\r\n    item {\r\n      itemId\r\n    }\r\n  }\r\n}",
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
            Console.WriteLine($"Item created with Id: {result.Data.createItem.item.itemId} ");
        }

    }
}

