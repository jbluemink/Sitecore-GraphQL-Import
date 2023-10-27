using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport.Model;

namespace SitecoreGraphqlmport
{
    internal class Media
    {

        /// <summary>
        /// Sample method which calls a GraphQL endpoint.
        /// </summary>
        internal static async Task RequestUploadMedia(CancellationToken cancellationToken, string itemPath, string file)
        {
            string graphqlendpoint = System.Configuration.ConfigurationManager.AppSettings.Get("graphqlAuthoringUrl");
            string accessToken = System.Configuration.ConfigurationManager.AppSettings.Get("accessToken");

            Console.WriteLine("mutation uploadMedia");

            // Call GraphQL endpoint here, specifying return data type, endpoint, method, query, and variables
            var result = await Request.CallGraphQLAsync<UploadMedia>(
                new Uri(graphqlendpoint),
                HttpMethod.Post,
                accessToken,
                "",
                "mutation { uploadMedia(input: { itemPath: \"" + itemPath + "\" }) { presignedUploadUrl }}",
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
            Console.WriteLine($"Found url: {result.Data.uploadMedia.presignedUploadUrl}");

            var result2 = await Request.CallGraphQLUploadAsync<Uploaded>(
                           HttpMethod.Post,
                           accessToken,
                           result.Data.uploadMedia.presignedUploadUrl,
                           file,
                           cancellationToken);

            // Examine the response to see if any errors were encountered
            if (!string.IsNullOrEmpty(result2.Message))
            {
                Console.WriteLine($"upload returned errors:{result2.Message}");
                return;
            }

            Console.WriteLine($"media uploaded id=: {result2.Id}");

        }
    }
}

