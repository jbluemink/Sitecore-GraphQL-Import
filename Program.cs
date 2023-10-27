using System;
using System.Threading;
using System.Threading.Tasks;
using SitecoreGraphqlmport;

namespace Client
{
    internal class Program
    {
        /// <summary>
        /// Program entry point with wire-up for Ctrl-C handler and exception handling.
        /// </summary>
        private static async Task Main(string[] args)
        {
            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    ConsoleCancelEventHandler handler = (o, e) =>
                    {
                        Console.WriteLine("Cancelling...");
                        cts.CancelAfter(0);
                        e.Cancel = true;
                    };
                    Console.CancelKeyPress += handler;
                    try
                    {
                        await GetItem.GetSitecoreItem(cts.Token, "/sitecore/Content/Home");
                        await GetSites.GetSitecoreSites(cts.Token);
                        Guid home = new Guid("{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}");
                        await CreateItem.CreateSampleItem(cts.Token, "test1",home);
                        await Media.RequestUploadMedia(cts.Token, "Project/jbltenant/jbl/newfolder/new2/newmedia1", "testimage.jpg");
                    }
                    finally
                    {
                        Console.CancelKeyPress -= handler;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancelled query");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
  
}